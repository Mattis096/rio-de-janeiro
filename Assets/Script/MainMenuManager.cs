using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Cinemachine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using UnityEngine.UIElements;


public class MainMenuManager : NetworkBehaviour
{
    private Texture2D blackTexture;
    private float alpha = 1f;
    private float fadeSpeed = 1.5f;
    private bool fadingIn = true;
    public CinemachineVirtualCamera MainMenuCam, CustomPersoView;
    public UnityEngine.UI.Button MenuBackButton;
    public GameObject PlayerPrefab;
    public GameObject PlayerBody;
    private Animator PrefabAnimator;
    public SkinnedMeshRenderer SkinnedMeshRenderer;

    public TMP_InputField PlayerInputField;
    public NetworkManager manager;
    public string PlayerInputUsrname;

    public UnityEngine.UI.Image Loading;

    private void Start()
    {
        //DontDestroyOnLoad(this);

        blackTexture = new Texture2D(1, 1);
        blackTexture.SetPixel(0, 0, Color.black);
        blackTexture.Apply();
        StartCoroutine(FadeIn());
        MainMenuCam.Priority = 10;
        CustomPersoView.Priority = 0;
        MenuBackButton.gameObject.SetActive(false);
        PrefabAnimator = PlayerPrefab.GetComponent<Animator>();
    }

    private void OnGUI()
    {
        if (alpha > 0)
        {
            GUI.color = new Color(0, 0, 0, alpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
        }
    }

    private IEnumerator FadeIn()
    {
        while (alpha > 0)
        {
            alpha -= Time.deltaTime / fadeSpeed;
            yield return null;
        }
        alpha = 0;
    }

    private IEnumerator FadeOut(string scene)
    {
        while (alpha < 1)
        {
            alpha += Time.deltaTime / fadeSpeed;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }

    
    public void StartHostButton()
    {

        // Show 
        Loading.gameObject.SetActive(true);

        OnNameChanged();
        //SceneTransitionLoader.Instance.StartLoading();
        
        manager.StartHost();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Loading.gameObject.SetActive(false);
    }

    public void StartClientButton()
    {
        OnNameChanged();
        manager.StartClient();
    }
    public void OpenSettingsMenu()
    {
        SceneHistoryManager.Instance.LoadScene("SettingsScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnNameChanged()
    {
        Debug.Log(PlayerInputField.text);
        PlayerOnLoad.Instance.SetPlayerName(PlayerInputField.text);
    }

    // old
    public void ApplyModifButton(NetworkManager nm)
    {
        PlayerPrefab.gameObject.name = "NEW CUSTOM PREFAB";
        nm.playerPrefab = PlayerPrefab;
    }
    public void SwitchMainMenuView()
    {
        MainMenuCam.Priority = 10;
        CustomPersoView.Priority = 0;
        MenuBackButton.gameObject.SetActive(false);
        PrefabAnimator.CrossFade("sitting", 0.3f);
    }
    public void SwitchPlayerCustomView()
    {
        MainMenuCam.Priority = 0;
        CustomPersoView.Priority = 10;
        MenuBackButton.gameObject.SetActive(true);
        PrefabAnimator.CrossFade("idle", 0.1f);
    }

}
