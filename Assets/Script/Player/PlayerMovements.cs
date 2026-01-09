using Mirror;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController characterController;
    public Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;     
    public float mouseSensitivity;

    [SerializeField] public Camera playerCamera;

    [Header("Ground Check")]
    public Transform groundCheck;    
    public float groundDistance = 0.1f; 
    public LayerMask groundMask;    
    public bool isGrounded;

    [Header("Jump Settings")]
    public float jumpHeight = 3.5f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2.5f; // Augmente la gravité à la descente
    public float coyoteTime = 0.2f; // Temps après lequel on peut encore sauter après avoir quitté le sol
    public float coyoteTimeCounter = 0f;

    [Header("Audio Jump Sound and Source")]
    public AudioClip jumpAudioClip;
    public AudioSource jumpAudioSource;

    [Header("Audio Walk Sound and Source")]
    public AudioClip footStepsClip;
    public AudioSource footSteps;

    [Header("Breakers")]
    public bool canMove = true;

    [Header("Variables")]
    public Vector3 playerMovement;
    private float PlayerMovementLerpAcceleration = 10f;
    private Vector3 kbInputs;
    private float mouseX;
    private float mouseY;

    [Header("Camera Shooting Recoil Settings")]
    public float currentRecoilY;
    public float currentRecoilX;
    public float PlayerXRotation;
    public float CameraYRotation;
    public float recoilSmoothTime = 0.1f; // Temps de lissage
    private float recoilVelocityX, recoilVelocityY; // Vélocité pour SmoothDamp


    void Start()
    {
        //Application.targetFrameRate = 60;

        if (isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            playerMovement = Vector3.zero;
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
        }

    }

    

    void Update()
    {
        if (isLocalPlayer)
        {
            PlayerKeyboardInput();
            PlayerMouseInput();

            ApplyPlayerMovement(canMove);
            ManagePlayerJump(canMove);
            MouseLook(canMove);


            if (characterController.velocity.x != 0f || characterController.velocity.z != 0f)
            {
                CmdPlayFootSteps();
            }
            else
            {
                CmdResetFootSteps();
            }

            if(Input.GetKeyDown(KeyCode.P))
            {
                animator.SetTrigger("Special");
            }
        }
    }

    public void EDPlayerMovements(bool status)
    {
        canMove = status;
    }

    public void PlayerKeyboardInput()
    {
        kbInputs.x = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
        kbInputs.z = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            CmdPlayWalkingAnimation(true);
        }
        else
        {
            CmdPlayWalkingAnimation(false);
        }

        //kbInputs.x = Input.GetAxisRaw("Horizontal");
        //kbInputs.z = Input.GetAxisRaw("Vertical");
    }


    [Command]
    public void CmdPlayWalkingAnimation(bool isWalking)
    {
        RpcPlayWalkingAnimation(isWalking);
    }

    [ClientRpc]
    public void RpcPlayWalkingAnimation(bool isWalking)
    {
        animator.SetBool("isWalking", isWalking);
    }

    public void PlayerMouseInput()
    {
        mouseX = Input.GetAxis("Mouse X") * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime;
    }

    public void ApplyPlayerMovement(bool breaker)
    {
        if (!breaker) { return; }
        
        Vector3 dir = (transform.right * kbInputs.x + transform.forward * kbInputs.z) * moveSpeed;
        playerMovement = Vector3.Lerp(playerMovement, dir, PlayerMovementLerpAcceleration * Time.deltaTime);

        characterController.Move(playerMovement * Time.deltaTime);
    }

   

    public void ManagePlayerJump(bool breaker)
    {
        if (!breaker) { return; }

        bool wasGrounded = isGrounded;
        isGrounded = (characterController.collisionFlags & CollisionFlags.Below) != 0;

        // Appliquer un "coyote time" pour un saut plus permissif
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reset du coyote time quand on touche le sol
        }
        else if (coyoteTimeCounter > 0)
        {
            coyoteTimeCounter -= Time.deltaTime; // Diminue le coyote time une fois en l'air
        }

        // Détection du saut
        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            animator.SetTrigger("Jumping");
            playerMovement.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            CmdPlayJumpSound();
            coyoteTimeCounter = 0; // Empêche un double saut abusif
        }
            
        // Appliquer la gravité avec un effet plus réaliste
        if (playerMovement.y > 0)
        {
            // Appliquer une gravité plus forte lors de la descente
            playerMovement.y += gravity * Time.deltaTime;
        }
        else
        {
            playerMovement.y += gravity * fallMultiplier * Time.deltaTime;
        }


        //else
        //{
        //    // Gravité normale en montée
        //    playerMovement.y += gravity * Time.deltaTime;
        //}

        // Appliquer le mouvement
        characterController.Move(playerMovement * Time.deltaTime);

        if ((characterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerMovement.y = 0;
            print("touched the ceiling");
        }
    }
    private void MouseLook(bool breaker)
    {
        if(!breaker) { return; }

        currentRecoilX = Mathf.SmoothDamp(currentRecoilX, 0, ref recoilVelocityX, recoilSmoothTime);
        currentRecoilY = Mathf.SmoothDamp(currentRecoilY, 0, ref recoilVelocityY, recoilSmoothTime);

        PlayerXRotation -= (mouseY * mouseSensitivity * 10) + currentRecoilY;
        PlayerXRotation = Mathf.Clamp(PlayerXRotation, -90f, 90f);

        CameraYRotation += (mouseX * mouseSensitivity * 10) + currentRecoilX;

        transform.rotation = Quaternion.Euler(0, CameraYRotation, 0);
        playerCamera.transform.localRotation = Quaternion.Euler(PlayerXRotation, 0, 0);
        
    }

    // Network

    [Command]
    void CmdPlayFootSteps()
    {
        RpcPlayFootSteps();
    }

    [ClientRpc]
    void RpcPlayFootSteps()
    {
        if (!footSteps.isPlaying) // Joue uniquement si aucun autre son ne joue
        {
            footSteps.PlayOneShot(footStepsClip);
        }
    }

    [Command]
    void CmdResetFootSteps()
    {
        RpcResetFootSteps();
    }

    [ClientRpc]
    void RpcResetFootSteps()
    {
        footSteps.Stop();
    }

    [Command]
    void CmdPlayJumpSound()
    {
        RpcPlayJumpSound();
    }

    [ClientRpc]
    void RpcPlayJumpSound()
    {
        jumpAudioSource.PlayOneShot(jumpAudioClip);
    }
}
