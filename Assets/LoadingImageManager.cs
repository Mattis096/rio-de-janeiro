using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoadingImageManager : MonoBehaviour
{

    public Sprite[] loadingImages;
    [SerializeField]public UnityEngine.UI.Image currentLoadingScreen;

    [Range(0, 10)]public uint index = 0;

    void Start()
    {
        currentLoadingScreen.sprite = loadingImages[0];
    }

    void Update()
    {
        if(loadingImages[index] != null)
        {
            currentLoadingScreen.sprite = loadingImages[index];
        }
    }


}
