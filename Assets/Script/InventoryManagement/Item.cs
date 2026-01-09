using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Item : NetworkBehaviour
{
    //[Command]
    //public void SPI(Item item, Vector3 pos, Quaternion rot)
    //{
    //    this.SpawnItem( item,  pos,  rot);
    //}


    [Server]
    public static void SpawnItem(Item item, Vector3 pos, Quaternion rot)
    {
        GameObject go = Instantiate(item.gameObject, pos, rot);
        NetworkServer.Spawn(go);
    }

    [Command]
    public void MoveItem(Vector3 pos)
    {
        transform.position = pos;
        RpcUpdateItemPosition(pos);
    }

    [ClientRpc]
    void RpcUpdateItemPosition(Vector3 pos)
    {
        transform.position = pos;
    }

}
