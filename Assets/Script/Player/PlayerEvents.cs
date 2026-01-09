using Mirror;
using UnityEngine;

public class PlayerEvents : NetworkBehaviour
{
    //public PlayerData playerData;

    //public GameObject currentPrefab;
    //private GameObject lastSpawnedPrefab;

    //public string GetPlayerName()
    //{
    //    return playerData.playerNameSynced;
    //}


    //[Command]
    //public void CmdSpawnPrefab(Vector3 pos)
    //{
    //    print("cmd call");


    //    if (currentPrefab == null) { print("null prefab"); return; }

    //    GameObject go = Instantiate(currentPrefab, pos, Quaternion.identity);
    //    NetworkServer.Spawn(go);
    //    lastSpawnedPrefab = go;
    //    UpdatePosition(lastSpawnedPrefab, new Vector3(10, 10, 10));
    //}

    //public void UpdatePosition(GameObject go, Vector3 pos)
    //{
    //    go.transform.position = pos;
    //}

}
