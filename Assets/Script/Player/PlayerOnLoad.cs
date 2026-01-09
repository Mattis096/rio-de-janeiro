using UnityEngine;

public class PlayerOnLoad : MonoBehaviour
{
    public static PlayerOnLoad Instance;
    public string playerName = "Joueur";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

}
