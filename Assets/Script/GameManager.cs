using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private const string PlayerPrefix = "Player";
    public static Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    
    public static string p1_name;
    public static string p2_name;

    public TextMeshProUGUI timer;
    public float CountDown = 0f;
    public bool isPaused = false;
    public bool isCalled = false;

    public static TextMeshProUGUI Player1;
    public static TextMeshProUGUI Player2;
    private static int P1_Score = 0;
    private static int P2_Score = 0;

    public GameObject winningObj;
    public TextMeshProUGUI winningText;

    private static bool isGameRunning = false;

    void Start()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreen);

        isPaused = true;
    }


    private void Update()
    {
        if (isGameRunning == false && players.Count == 2) 
        {
            StartGame();
        }

        UpdateCountDown();
    }

    public void UpdateCountDown()
    {
        if(CountDown > 0 && isPaused == false)
        {
            CountDown -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(CountDown / 60); // Calcul des minutes
            int seconds = Mathf.FloorToInt(CountDown % 60); // Calcul des secondes
            timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
        }
        else
        {
            timer.text = string.Format("00:00");
            isPaused = true;
            CountDown = 0f;
            isCalled = false;
        }
    }

    public static void UpdateScore(int sp1, int sp2)
    {
        P1_Score = sp1;
        P2_Score = sp2;
        Player1.text = string.Format($"{P1_Score}");
        Player2.text = string.Format($"{P2_Score}");
    }

    public void StartCountdown(float time)
    {
        if (!isCalled)
        {
            CountDown = time;
            isPaused = false;
            isCalled = true;
        }
    }

    
    //public void InstantiateWeapon()
    //{
    //    print("from gm");
    //    UIManager.LocalPlayer.GetComponent<PlayerEvents>().CmdSpawnPrefab(Vector3.zero);
    //    //UIManager.LocalPlayer.GetComponent<PlayerEvents>().CmdSpawnPrefab(Vector3.zero);
    //}

    public void StopCountDown()
    {
        CountDown = 1f;
        isPaused = true;
        isCalled = false;
    }

    public void PauseCountDown()
    {
        isPaused = true;
    }

    public IEnumerator WaitAbit(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public void StartGame()
    {
        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        print("Echauffement");
        StartCountdown(10);
        yield return new WaitForSeconds(10);

        print("La partie commence !");

        if (P1_Score >= 100)
        {
            
            yield break; // on arrête ici la coroutine
        }
        else if (P2_Score >= 100)
        {
            EndGame(p2_name);
            yield break; // on arrête ici la coroutine
        }

        StartCountdown(180);
        yield return new WaitForSeconds(180);


        //winningText.text = "{PLAYER} won!";
        //winningObj.SetActive(true);

    }

    public void EndGame(string winner)
    {
        CMD_EndGame(winner);
    }

    [Command]
    public void CMD_EndGame(string winner)
    {
        RPCEndGame(winner);
    }

    [ClientRpc]
    public void RPCEndGame(string winner)
    {
        winningText.text = $"{p1_name} won!";
        winningObj.SetActive(true);
    }

    public static void RegisterPlayer(string netID, GameObject player)
    {
        string playerID = PlayerPrefix + netID;
        //if (PlayerOnLoad.Instance.playerName != "")
        //{
        //    playerID = PlayerOnLoad.Instance.playerName;
        //}

        players.Add(playerID, player);
        player.transform.name = playerID;
        if(players.Count == 1)
        {
            p2_name = playerID;
            player.GetComponent<PlayerData>().team = Team.Red;
        }
        else
        {
            p1_name = playerID;
            player.GetComponent<PlayerData>().team = Team.Blue;
        }


    }

    public static void UnregisterPlayer(string netID)
    {
        players.Remove(netID);
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}
