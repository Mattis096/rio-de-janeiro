using UnityEngine;
using System.Collections;

public class SpeakerManager : MonoBehaviour
{
    public AudioSource audioSource;  // Référence à l'AudioSource
    public AudioClip[] announcements;  // Liste des annonces du speaker
    public float minInterval = 15f;  // Intervalle minimum entre annonces
    public float maxInterval = 30f;  // Intervalle maximum entre annonces
    public float volume = 15f; // Volume de l'annonce

    private bool isSpeaking = false; // Vérifie si une annonce est en cours

    void Start()
    {
        audioSource.volume = volume;  // Définit le volume de base
        StartCoroutine(PlayAnnouncements());// Démarre la routine pour jouer des annonces aléatoires
    }

    IEnumerator PlayAnnouncements()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            if (!isSpeaking) // Vérifie si une annonce est en cours
            {
                PlayRandomAnnouncement();// Joue une annonce aléatoire
            }
        }
    }

    void PlayRandomAnnouncement()
    {
        if (announcements.Length == 0) return;  // Vérifie si y'a des sons

        int randomIndex = Random.Range(0, announcements.Length); // Son au hasard
        audioSource.clip = announcements[randomIndex]; // Assigne le son
        audioSource.Play(); // Joue l'annonce
        isSpeaking = true;

        StartCoroutine(WaitForAnnouncementEnd()); // Attend la fin
    }

    IEnumerator WaitForAnnouncementEnd()
    {
        yield return new WaitForSeconds(audioSource.clip.length);// Attend la fin du clip audio
        isSpeaking = false;
    }

    public void AnnounceEvent(AudioClip specialAnnouncement)
    {
        if (!isSpeaking) // Évite d'interrompre un son en cours
        {
            audioSource.clip = specialAnnouncement;
            audioSource.Play();
            isSpeaking = true;
            StartCoroutine(WaitForAnnouncementEnd());
        }
    }
}
