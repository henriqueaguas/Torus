using UnityEngine;

public class RadioSongController : MonoBehaviour
{
    public void Play()
    {
        AudioSource audioSource = this.GetComponent<AudioSource>();

        audioSource.enabled = true;
        audioSource.Play();
    }
}
