using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class KillFeedManager : NetworkBehaviour
{
    [Header("Static Reference")]
    public static KillFeedManager instance;

    [Header("References")]
    public GameObject KillFeedMessagePrefab;
    public GameObject KillFeed;

    [Header("Variables")]

    [SyncVar] private string killFeedContent = "";

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [Command(requiresAuthority = false)]
    public void CmdAddKillMessage(string killer, string victim)
    {
        RpcUpdateKillFeed(killer, victim);
    }

    [ClientRpc]
    private void RpcUpdateKillFeed(string killer, string victim)
    {
        GameObject kf = Instantiate(KillFeedMessagePrefab, KillFeed.transform);
        kf.GetComponent<KillFeedMessage>().SetMessage(killer, victim);
    }

}
