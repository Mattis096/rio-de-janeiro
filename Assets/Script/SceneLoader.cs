using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen; // Écran de chargement à afficher
    public UnityEngine.UI.Slider progressBar; // Barre de progression (optionnelle)

    private void Start()
    {
        // Si la scène commence à charger, on affiche l'écran de chargement
        loadingScreen.SetActive(false);
    }

    // Fonction pour charger la scène en arrière-plan
    public void LoadMultiplayerScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        loadingScreen.SetActive(true); // Affiche l'écran de chargement

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false; // Empêche l'activation automatique de la scène

        while (!operation.isDone)
        {
            // Afficher le pourcentage de chargement
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            // Si la scène est presque chargée, on active la scène
            if (operation.progress >= 0.9f)
            {
                // Le réseau doit être prêt avant de passer à la scène finale
                if (NetworkClient.isConnected)
                {
                    operation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
