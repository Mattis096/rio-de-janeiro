using UnityEngine;
using Mirror;

public class CameraViewManager : NetworkBehaviour
{

    public GameObject[] gameObjectsToDisable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if(isLocalPlayer)
        {
            for (int i = 0; i < gameObjectsToDisable.Length; i++)
            {
                gameObjectsToDisable[i].SetActive(false);
            }
        }

        
    }

    
}
