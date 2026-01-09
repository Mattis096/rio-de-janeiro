using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : NetworkBehaviour
{
    public GameObject loadingScreen;  // Le Canvas contenant l'écran de chargement
    public Text loadingText;          // Le texte de chargement (optionnel)
    public Slider loadingBar;         // La barre de progression (optionnelle)

    private NetworkManager networkManager;

    private void Awake()
    {
        // On récupère le NetworkManager
        networkManager = NetworkManager.singleton;
    }

    private void OnEnable()
    {
        // S'abonner aux événements natifs de Mirror
        //networkManager.OnStartHost();
        //networkManager.OnStartClient();
    }




    private void OnDisable()
    {
        // Aucun désabonnement nécessaire car on n'a pas créé d'événements
    }

    

    // Quand le client démarre la connexion
    public override void OnStartClient()
    {
        base.OnStartClient();
        ShowLoadingScreen("Connexion au serveur...");
    }

    

    // Affiche l'écran de chargement
    private void ShowLoadingScreen(string message)
    {
        loadingScreen.SetActive(true);
        loadingText.text = message;
        loadingBar.value = 0f;  // Remet la barre à 0
    }

    // Masque l'écran de chargement
    private void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }
}
