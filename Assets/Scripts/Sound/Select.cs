using UnityEngine;

public class CharacterSelectSound : MonoBehaviour
{
    public AudioSource audioSource;       // az AudioSource ami lejátssza a hangot
    public AudioClip[] selectionSounds;   // 3 hang, sorrendben a karakterekhez

    private int currentIndex = 0;         // melyik karakter van most kiválasztva

    // Jobbra gombhoz
    public void Next()
    {
        currentIndex = (currentIndex + 1) % selectionSounds.Length;
        PlaySelectionSound();
    }

    // Balra gombhoz
    public void Previous()
    {
        currentIndex = (currentIndex - 1 + selectionSounds.Length) % selectionSounds.Length;
        PlaySelectionSound();
    }

    private void PlaySelectionSound()
    {
        if (selectionSounds.Length == 0) return;

        audioSource.PlayOneShot(selectionSounds[currentIndex]);
        Debug.Log("Character index: " + currentIndex);
    }
}