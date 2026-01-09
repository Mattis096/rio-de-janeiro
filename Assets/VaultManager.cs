using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VaultManager : NetworkBehaviour
{
    Camera billboardCam = null;

    public GameObject VaultDoor;
    public bool isOpen = false;
    Quaternion target;
    public float timePressed = 0;
    public bool canPress = false;
    public Image loadingVault;
    public float timeToOpen = 3f;
    public GameObject elems;

    public Team team;
    public Image loadingBar;
    public TMP_Text loadingText;

    [SyncVar(hook = nameof(OnVaultProgressChanged))]
    private float vaultProgress = 0f; // SyncVar pour synchroniser la progression

    private void Start()
    {
        target = Quaternion.Euler(VaultDoor.transform.localRotation.eulerAngles.x, VaultDoor.transform.localRotation.eulerAngles.y, 179);
        loadingVault.fillAmount = 0;
        loadingBar.fillAmount = 0;
    }

    private void Update()
    {
        if (canPress)
        {
            PlayerPressing();
        }

        if (timePressed > timeToOpen)
        {
            CmdOpenVault();
        }
    }

    private void LateUpdate()
    {
        if (billboardCam != null)
        {
            elems.transform.LookAt(billboardCam.transform.position);
            elems.transform.Rotate(0, 180, 0);
        }
    }

    public void OpenVault()
    {
        CmdOpenVault();
    }

    [Command(requiresAuthority = false)]
    void CmdOpenVault()
    {
        RpcOpenVault();
    }

    [ClientRpc]
    void RpcOpenVault()
    {
        VaultDoor.transform.localRotation = Quaternion.Lerp(VaultDoor.transform.localRotation, target, Time.deltaTime);
        isOpen = true;
        elems.SetActive(false);
    }

    public GameObject uiElement; // Élément UI à afficher

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LocalPlayer") && other.GetComponent<PlayerData>().team != team)
        {
            billboardCam = other.GetComponent<PlayerMovement>().playerCamera;
            uiElement.SetActive(true);
            loadingVault.gameObject.SetActive(true);
            canPress = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        uiElement.SetActive(false);
        loadingVault.gameObject.SetActive(false);
        canPress = false;
    }

    public void PlayerPressing()
    {
        if (Input.GetKey(KeyCode.T))
        {
            timePressed += Time.deltaTime;
            float progress = Mathf.Clamp(timePressed, 0, timeToOpen);

            CmdUpdateVaultProg(progress); // Envoi au serveur

            loadingVault.fillAmount = progress / timeToOpen;
        }
    }

    [Command(requiresAuthority = false)]
    void CmdUpdateVaultProg(float progress)
    {
        vaultProgress = progress; // Mise à jour sur le serveur → Synchronisation automatique sur tous les clients
    }

    void OnVaultProgressChanged(float oldProgress, float newProgress)
    {
        loadingBar.fillAmount = newProgress / timeToOpen; // Met à jour l'UI sur tous les clients
        loadingText.text = ((int)(newProgress/timeToOpen * 100)).ToString() + "%";
    }
}
