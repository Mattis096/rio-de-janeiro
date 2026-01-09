using Mirror.BouncyCastle.Tsp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    [Header("References")]
    public Toggle FullScreenToggle;
    public TMP_Dropdown ResolutionDropdown;
    public TMP_Dropdown QualityDropdown;


    private void Start()
    {
        Screen.SetResolution(1920, 1080, false); // false = mode fenêtré non redimensionnable

        FullScreenToggle.isOn = Screen.fullScreen;

        // Ajouter un listener pour détecter les changements
        FullScreenToggle.onValueChanged.AddListener(SetFullscreen);
        ResolutionDropdown.onValueChanged.AddListener(ChangeResolution);

        QualityDropdown.ClearOptions();
        QualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));

        // Sélectionner la qualité actuelle
        QualityDropdown.value = QualitySettings.GetQualityLevel();
        QualityDropdown.RefreshShownValue();

        // Ajouter l'événement pour changer la qualité
        QualityDropdown.onValueChanged.AddListener(ChangeQuality);

    }

    void ChangeResolution(int index)
    {
        switch (index)
        {
            case 0: // FHD
                Screen.SetResolution(3840, 2160, Screen.fullScreen);
                break;
            case 1: // 4K
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 2: // 720p
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                break;
        }
    }

    void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    void ChangeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
    }

    public void OpenMainMenu()
    {
        SceneHistoryManager.Instance.LoadPreviousScene();
    }

}
