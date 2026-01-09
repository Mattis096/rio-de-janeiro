using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
public class FPSCounter : MonoBehaviour
{
    public TMP_Text fpsText;
    public TMP_Text pingText;
    private float deltaTime = 0.0f;

    private void Start()
    {
        Application.targetFrameRate = 300;
    }
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();
        double v = NetworkTime.rtt * 1000;
        pingText.text = Mathf.Round((float)v).ToString();
    }
}

