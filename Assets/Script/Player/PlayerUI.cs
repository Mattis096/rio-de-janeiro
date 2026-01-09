using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class PlayerUI : MonoBehaviour
{

    [Header("ref")]
    public LoadingManager LoadingManager;

    public TextMeshProUGUI txt;

    public TextMeshProUGUI currentWeaponAmmo;

    public TMP_Text Minimap_Kills;
    public TMP_Text Minimap_Deaths;

    public Image PrimaryWp_Img;
    public Image SecondaryWp_Img;

    public Image playerHealthBar;
    public int MaxHealth = 100;

    public void localDebugger(string data)
    {
        txt.text = data;
    }

    public void setPlayerAmmo(int current, int max)
    {
        currentWeaponAmmo.text = current + "/" + max;
    }

    public void UpdateLifebar(float health)
    {
        playerHealthBar.fillAmount = Mathf.Clamp(health, 0, MaxHealth) / MaxHealth;
    }

    public void UpdatePlayerKD(int kills, int deaths)
    {
        Minimap_Kills.text = kills.ToString();
        Minimap_Deaths.text = deaths.ToString();
    }
}
