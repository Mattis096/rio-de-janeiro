using UnityEngine;
using TMPro;



public class KillFeedMessage : MonoBehaviour
{
    public TMP_Text killerName;
    public TMP_Text victimName;

    public float duration = 4f;
    public float delta;

    
    public void SetMessage(string killer, string victim)
    {
        killerName.text = killer;
        victimName.text = victim;
    }

    private void Update()
    {
        delta += Time.deltaTime;

       

        if (delta > duration)
        {
            Destroy(gameObject);
        }

    }


}
