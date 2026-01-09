using JetBrains.Annotations;
using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using System.Collections;

public enum Team
{
    Blue,
    Red
}


public class PlayerData : NetworkBehaviour
{
    public Team team;

    [Header("Player Ref")]
    public GameObject player;
    public Animator animator;
    

    [Header("Player Elements")]
    public TextMeshProUGUI playerNameText;

    [Header("Player Game Infos")]
    public int KillCounter;
    public int DeathCounter;

    [Header("Dynamic References")]
    private PlayerUI PlayerUI;

    [Header("Variables Sync")]
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerNameSynced = "";
    [SyncVar(hook = nameof(OnHealthChanged))]
    private float playerHealthSynced = 100;

    [Header("Ragdoll Manager")]
    public Rigidbody CamRigidBody;
    public GameObject camPos;
    public Rigidbody[] _ragdollRigidbodies;
    public GameObject[] _ragdollOriginalPos;

    public bool canTakeDamage = true;

    public void RegisterOriginalPos()
    {
        GameObject rb = new GameObject();
        rb.transform.position = CamRigidBody.transform.position;
        camPos = rb;

        _ragdollOriginalPos = new GameObject[_ragdollRigidbodies.Length];
        for(int i = 0; i <  _ragdollRigidbodies.Length; i++)
        {
            GameObject go = new GameObject();
            go.transform.localPosition = _ragdollRigidbodies[i].transform.localPosition;
            go.transform.localRotation = _ragdollRigidbodies[i].transform.localRotation;
            _ragdollOriginalPos[i] = go;
        }
    }

    public void ResetRigidBodyPos()
    {
        DisableRagdoll();
        for(int i = 0; i < _ragdollOriginalPos.Length; i++)
        {
            _ragdollRigidbodies[i].transform.localPosition = _ragdollOriginalPos[i].transform.localPosition;
            _ragdollRigidbodies[i].transform.localRotation= _ragdollOriginalPos[i].transform.localRotation;
        }
        //CamRigidBody.transform.position = camPos.transform.position;
    }

    private void Awake()
    {
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        RegisterOriginalPos();
        DisableRagdoll();
    }

    

    public void DisableRagdoll()
    {
        foreach(var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
        //CamRigidBody.isKinematic = true;
    }

    public void EnableRagdoll()
    {
        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }
        //CamRigidBody.isKinematic = false;
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            PlayerUI = GameObject.FindGameObjectWithTag("playerInfos").GetComponent<PlayerUI>();
            PlayerUI.localDebugger(PlayerUI.GetInstanceID().ToString());
            ResetPlayerKD();
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            PlayerUI.UpdatePlayerKD(KillCounter, DeathCounter);
        
            //if(Input.GetKey(KeyCode.H))
            //{
            //    EnableRagdoll();
            //}
            //if (Input.GetKey(KeyCode.K))
            //{
            //    ResetRigidBodyPos();
            //}
        }
    }

    [Command]
    public void DebugKillPlayer()
    {
        RpcOnPlayerDeath("myself");
    }

    public override void OnStartLocalPlayer()
    {
        string name = "player" + Random.Range(1000, 9999);
        CmdSetupPlayer(PlayerOnLoad.Instance.playerName);

        if(isLocalPlayer)
        {
            playerNameText.gameObject.SetActive(false);
        }
    }

    public void ResetPlayerKD()
    {
        KillCounter = 0;
        DeathCounter = 0;
    }


    [Command]
    public void CmdSetupPlayer(string name)
    {
        playerNameSynced = name;
    }

    public void OnPlayerNameChanged(string _Old,  string _New)
    {
        playerNameText.text = _New;
    }


    [Server]
    public bool TakeDamage(float amount, string from)
    {
        if(canTakeDamage)
        {
            playerHealthSynced -= amount;
            Debug.Log("Now, phs = " + playerHealthSynced);
        }

        if (playerHealthSynced <= 0)
        {
            canTakeDamage = false;
            playerHealthSynced = 100;
            RpcOnPlayerDeath(from);
            StartCoroutine(WaitForRespawn());
            return true;
        }
        return false;
    }

    public Camera playerCam;

    public IEnumerator WaitForRespawn()
    {
        animator.enabled = false;
        EnableRagdoll();
        
        yield return new WaitForSeconds(3f);
        canTakeDamage = true;
        CmdRespawnPlayer();
    }

    [Client]
    public void CmdRespawnPlayer()
    {
        RpcRespawnPlayer();
    }

    [ClientRpc]
    public void RpcRespawnPlayer()
    {
        ResetRigidBodyPos();
        animator.enabled = true;

        if(isLocalPlayer)
        {
            player.transform.position = new Vector3(-2, -5.5f, 0.2f);
        }

    }

    [ClientRpc]
    public void RpcOnPlayerDeath(string from)
    {
        // Actions a effectuer chez moi
        if(isLocalPlayer)
        {
            DeathCounter += 1;
            
            Debug.Log("im deadm");

            // Desactiver les degats
            // Animation camera mort joueur
            // Teleporter le joueur

        
            
            return;
        }
        animator.enabled = false;
        EnableRagdoll();

        KillFeedManager.instance.CmdAddKillMessage(from, playerNameSynced);
        

    }

    
    void OnHealthChanged(float _Old, float _New)
    {
        if (isLocalPlayer)
        {
            PlayerUI.localDebugger(_New.ToString());
            PlayerUI.UpdateLifebar(playerHealthSynced);



            Debug.Log($"OnHealthChanged {playerNameSynced} : {_Old} -> {_New}");
        }
    }
}
