using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [Header("References")]
    public Image LoadingImg;

    [Header("Values")]
    public Color LoaderColor;
    public float timeToLoad = 10f;
    public bool done = false;

    [Header("Variables")]
    public bool isStarted = false;
    public float elapsedTime = 0f;

    public void Start()
    {
        if(LoaderColor != null)
            LoadingImg.color = LoaderColor;
        LoadingImg.fillAmount = 0;
    }

    public void Update()
    {
        Load(isStarted);
    }

    private void Load(bool canStart)
    {
        if (canStart == false) { return; }
        print("here");
        elapsedTime += Time.deltaTime;
        LoadingImg.fillAmount = Mathf.Clamp(elapsedTime, 0, timeToLoad) / timeToLoad;
        if (elapsedTime >= timeToLoad)
        {
            done = true;
            ResetLoader();
        }
    }

    public void StartLoading()
    {
        isStarted = true;
    }

    public void ResetLoader()
    {
        isStarted = false;
        elapsedTime = 0f;
        LoadingImg.fillAmount = 0;
    }

}
