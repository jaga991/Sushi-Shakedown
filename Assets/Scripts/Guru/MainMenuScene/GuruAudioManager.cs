using UnityEngine;
// alias Unity’s SceneManager so we can keep our class name
using UnityEngine.SceneManagement;
// This is jsut for KitchenSceneManager.cs
public class GuruAudioManager : MonoBehaviour
{
    public AudioSource BackgroundMusicSource;
    public AudioSource SFXSource;
    public AudioClip ButtonClickClip;

    public void PlayButtonClickSound()
    {
        Debug.Log("Button click sound played.");
        if (SFXSource != null && ButtonClickClip != null)
        {
            SFXSource.PlayOneShot(ButtonClickClip);
            Debug.Log("Button click sound played.");

        }
    }

}
