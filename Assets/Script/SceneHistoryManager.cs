using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHistoryManager : MonoBehaviour
{
    private static SceneHistoryManager instance;
    private Stack<string> sceneHistory = new Stack<string>();

    public static SceneHistoryManager Instance
    {
        get
        {
            // Si l'instance n'existe pas, on la crée
            if (instance == null)
            {
                instance = Object.FindFirstObjectByType<SceneHistoryManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        // Si une instance existe déjà, on la détruit (on n'en garde qu'une seule)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Garde ce GameObject persistant entre les scènes
        }
        else
        {
            Destroy(gameObject); // Si l'instance existe déjà, on détruit ce GameObject
        }
        //sceneHistory.Push(SceneManager.GetActiveScene().name);
    }

    

    public void LoadScene(string sceneName)
    {
        sceneHistory.Push(SceneManager.GetActiveScene().name); // Sauvegarde la scène actuelle
        Debug.Log(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadPreviousScene()
    {
        if (sceneHistory.Count > 0)
        {
            string previousScene = sceneHistory.Pop(); // Récupère la dernière scène
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.Log("Aucune scène précédente enregistrée.");
        }
    }
}
