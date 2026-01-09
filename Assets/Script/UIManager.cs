using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.PlayerLoop;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using TMPro;
using UnityEngine.Rendering;

public class UIManager : NetworkBehaviour
{
    public static GameObject LocalPlayer;

    [Header("Escape Menu")]
    public GameObject QuitButton;
    public GameObject SettingsButton;

    [Header("Settings Menu")]
    public GameObject SettingsPanel;

    [Header("Shop Menu")]
    public GameObject ShopMenu;

    [Header("Player UI")]
    public GameObject HealthBar;
    public GameObject PlayerAmmo;
    public GameObject PlayerSight;
    public GameObject Timer;
    public GameObject Score;
    public GameObject Minimap;

    public bool isSettingsButtonOpen = false;
    public bool isQuitButtonOpen = false;
    public bool isHealthBarOpen = false;
    public bool isPlayerAmmoOpen = false;
    public bool isPlayerSightOpen = false;
    public bool isTimerOpen = false;
    public bool isScoreboardOpen = false;
    public bool isShopMenuOpen = false;
    public bool isMiniMapOpen = false;
    public bool isSettingsOpen = false;

    [SerializeField] private bool isElementOpen = false;

    public KeyCode ShopKeyCode;
    public KeyCode EscapeKeyCode;

    void Start()
    {
        EDGameSceneUI(true);
    }

    private void Update()
    {      
        ManageEscape(EscapeKeyCode);
        ManageShop(ShopKeyCode);
    }

    

    public void ManageEscape(KeyCode keycode)
    {
        if (Input.GetKeyDown(keycode) && (!isElementOpen || isQuitButtonOpen))
        {
            isElementOpen = !isElementOpen;
            QuitButton.SetActive(!isQuitButtonOpen);
            SettingsButton.SetActive(!isSettingsButtonOpen);
            EDPlayerInteractions(isQuitButtonOpen);
            ShowCursor(!isQuitButtonOpen);
            isQuitButtonOpen = !isQuitButtonOpen;
            isSettingsButtonOpen = !isSettingsButtonOpen;
        }
    }
    public void ManageShop(KeyCode keycode)
    {
        if(Input.GetKeyDown(keycode) && (!isElementOpen || isShopMenuOpen))
        {
            isElementOpen = !isElementOpen;
            ShopMenu.SetActive(!isShopMenuOpen);
            EDPlayerInteractions(isShopMenuOpen);
            ShowCursor(!isShopMenuOpen);
            isShopMenuOpen = !isShopMenuOpen;
        }
    }
    public void EDPlayerInteractions(bool status)
    {
        LocalPlayer.GetComponent<PlayerMovement>().EDPlayerMovements(status);
        LocalPlayer.GetComponent<PlayerInteractions>().EDPlayerShoot(status);
    }
    private void EDGameSceneUI(bool status)
    {
        PlayerAmmo.SetActive(status);
        HealthBar.SetActive(status);
        Timer.SetActive(status);
        Score.SetActive(status);
        PlayerSight.SetActive(status);
        Minimap.SetActive(status);
    }
    public void ShowCursor(bool status)
    {
        Cursor.visible = status; // Rend le curseur visible
        if(status)
        {
            Cursor.lockState = CursorLockMode.None; // Déverrouille la souris
        }else
        {
            Cursor.lockState = CursorLockMode.Locked; // Déverrouille la souris
        }
    }

    public void OpenSettings()
    {
        //SceneHistoryManager.Instance.LoadScene("SettingsScene");
        SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsPanel.SetActive(false);
    }

    // Shop Event

    public void ChangePlayerLoadout(string name)
    {
        LocalPlayer.GetComponent<PlayerInteractions>().CmdChangeActiveLoadout(name);
    }
}
