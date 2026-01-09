using Mirror;
using Mirror.Examples.Basic;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    public GameObject TPS_VIEW;
    public GameObject FPS_VIEW;
    public PlayerUI PlayerUI;
    public GameObject PlayerEyes;
    public LoadingManager Loader;

    [Header("Local to Disable")]
    public GameObject PlayerShowedName;

    public void Start()
    {
        PlayerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
        Loader = PlayerUI.LoadingManager;
        GetComponent<PlayerInteractions>().playerUI = PlayerUI;
        Application.targetFrameRate = 300;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();

        GameManager.RegisterPlayer(netID, this.gameObject);
        if(isLocalPlayer)
        {
            UIManager.LocalPlayer = this.gameObject;
        }

        InitPlayerViews();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        GameManager.UnregisterPlayer(netID);

        CmdDestroyPlayerInstance();
        
        //Destroy(this);

    }

    [Command]
    void CmdDestroyPlayerInstance()
    {
        RpcDestroyPlayerInstance();
    }

    [ClientRpc]
    void RpcDestroyPlayerInstance()
    {
        Destroy(this);
    }

    public void InitPlayerViews()
    {
        gameObject.tag = "Player";

        if (isLocalPlayer)
        {
            PlayerShowedName.SetActive(false);
            TPS_VIEW.SetActive(false);
            FPS_VIEW.SetActive(true);
            gameObject.tag = "LocalPlayer";
        }
    }
}
