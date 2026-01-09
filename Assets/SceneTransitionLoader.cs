using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionLoader : MonoBehaviour
{
    public Sprite[] loadingImages;
    public Image loadingBar;

    private static SceneTransitionLoader instance;

    public static SceneTransitionLoader Instance
    {
        get
        {
            // Si l'instance n'a pas encore été créée, on la crée
            if (instance == null)
            {
                instance = Object.FindFirstObjectByType<SceneTransitionLoader>();

                // Si aucune instance n'a été trouvée, en créer une nouvelle
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("SceneTransitionLoader");
                    instance = singletonObject.AddComponent<SceneTransitionLoader>();
                }
            }

            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        loadingBar.fillAmount = 0;
        foreach (var image in loadingImages)
        {
            image.GetComponent<SpriteRenderer>().enabled = false;
        }


    }

    public void StartLoading()
    {
        GetComponent<Image>().sprite = loadingImages[Random.Range(0, 5) % loadingImages.Length];
    }
}
