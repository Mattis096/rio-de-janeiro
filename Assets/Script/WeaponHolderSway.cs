using Mirror.Examples.Benchmark;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHolderSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float smooth;
    public float swayMultiplier;
    public float idleSwayMultiplier;
    public float idleFreq;

    // Player Input --> PlayerMouseInput()
    private float mouseX;
    private float mouseY;

    // Player Sway --> CalculateSway()
    private Quaternion sway;


    //public Quaternion idleSway;
    //public Quaternion idleSway2;

    private float elapsedTime = 0f;

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        PlayerMouseInput();

        // Player Weapons Sway
        CalculateSway();
        ApplySway();

    }

    private void PlayerMouseInput()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    private void CalculateSway()
    {
        //idleSway = Quaternion.AngleAxis(-Mathf.Sin(elapsedTime * idleFreq) * idleSwayMultiplier, Vector3.right);
        //idleSway2 = Quaternion.AngleAxis(-Mathf.Cos(elapsedTime * idleFreq) * idleSwayMultiplier, Vector3.up);

        Quaternion rotX = Quaternion.AngleAxis(-mouseX * swayMultiplier, Vector3.up);
        Quaternion rotY = Quaternion.AngleAxis(-mouseY * swayMultiplier, Vector3.right);
        Quaternion targetSway = rotX* rotY;

        sway = Quaternion.Slerp(transform.localRotation, targetSway, smooth * Time.deltaTime);
    }

    private void ApplySway()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, sway, smooth * Time.deltaTime);
    }


}
