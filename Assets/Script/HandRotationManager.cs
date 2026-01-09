using UnityEngine;

public class HandRotationManager : MonoBehaviour
{
    public Transform cameraRotation;
    // Update is called once per frame
    void Update()
    {
        transform.rotation = cameraRotation.rotation;
    }
}
