using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;  // Référence à l'AudioSource qui jouera les sons
    public AudioClip[] soundClips;   // Tableau des sons disponibles
    public float minInterval = 2f;   // Intervalle minimum entre deux sons
    public float maxInterval = 5f;   // Intervalle maximum entre deux sons

    private bool isWaiting = false;  // Indique si on attend la fin du son avant d'en rejouer un

    void Update()
    {
        // Si l'AudioSource n'est pas en train de jouer et qu'on n'est pas en attente
        if (!audioSource.isPlaying && !isWaiting)
        {
            PlayRandomSound(); // Joue un son aléatoire
        }
    }

    // Joue un son aléatoire du tableau soundClips
    void PlayRandomSound()
    {
        if (soundClips.Length == 0) return;  // Vérifie s'il y a des sons dans le tableau

        int randomIndex = Random.Range(0, soundClips.Length); // Choisit un son au hasard
        audioSource.clip = soundClips[randomIndex]; // Assigne le son
        audioSource.Play(); // Joue le son

        isWaiting = true; // On marque qu'on attend la fin du son

        // Programme le prochain son après la durée du clip + un délai aléatoire
        Invoke(nameof(ResetWaiting), audioSource.clip.length + Random.Range(minInterval, maxInterval));
    }

    // Permet de rejouer un son après la fin du précédent
    void ResetWaiting()
    {
        isWaiting = false; // On peut jouer un nouveau son
    }
}
