// using UnityEngine;
// // alias Unity’s SceneManager so we can keep our class name
// using UnityEngine.SceneManagement;
// // This is jsut for KitchenSceneManager.cs
// public class CustomerAudioManager : MonoBehaviour
// {
//     public AudioSource BackgroundMusicSource;
//     public AudioSource SFXSource;
//     public AudioClip ButtonClickClip;

//     public AudioClip CoinsClip;

//     private AudioClip[] musicClips;

//     public void PlayButtonClickSound()
//     {
//         Debug.Log("Button click sound played.");
//         if (SFXSource != null && ButtonClickClip != null)
//         {
//             SFXSource.PlayOneShot(ButtonClickClip);
//             Debug.Log("Button click sound played.");

//         }
//     }

//     void Start()
//     {
//         PlayRandomMusic();
//     }


//     void Awake()
//     {
//         // Load all clips in that Resources folder
//         musicClips = Resources.LoadAll<AudioClip>(
//             "Audio/Guru/MainMenu/BackgroundTracks"
//         );

//         if (musicClips == null || musicClips.Length == 0)
//             Debug.LogWarning("MainMenuManager: No background tracks found in Resources!");
//     }

//     private void PlayRandomMusic()
//     {
//         if (BackgroundMusicSource == null || musicClips == null || musicClips.Length == 0)
//             return;

//         // pick one clip at random
//         int idx = Random.Range(0, musicClips.Length);
//         AudioClip chosen = musicClips[idx];
//         BackgroundMusicSource.clip = chosen;
//         BackgroundMusicSource.loop = true;
//         BackgroundMusicSource.Play();

//         Debug.Log($"Now playing: {chosen.name}");

//         // unload all other clips to free memory
//         for (int i = 0; i < musicClips.Length; i++)
//         {
//             if (i == idx) continue;
//             Resources.UnloadAsset(musicClips[i]);
//         }

//         // keep only the chosen clip in the array
//         musicClips = new AudioClip[] { chosen };
//     }

//     // need to call this when mode changes ? 

//     public void ChangeBackgroundMusic()
//     {
//         if (BackgroundMusicSource == null || musicClips == null || musicClips.Length == 0)
//             return;

//         // Load fresh list of all background music clips
//         AudioClip[] allClips = Resources.LoadAll<AudioClip>("Audio/Guru/MainMenu/BackgroundTracks");

//         if (allClips == null || allClips.Length == 0)
//         {
//             Debug.LogWarning("ChangeBackgroundMusic: No music clips found!");
//             return;
//         }

//         // Pick a new random track different from the current one
//         AudioClip current = BackgroundMusicSource.clip;
//         AudioClip newClip = current;

//         int attempts = 0;
//         while (newClip == current && attempts < 10)
//         {
//             newClip = allClips[Random.Range(0, allClips.Length)];
//             attempts++;
//         }

//         BackgroundMusicSource.clip = newClip;
//         BackgroundMusicSource.loop = true;
//         BackgroundMusicSource.Play();

//         Debug.Log($"Changed background music to: {newClip.name}");

//         // Keep only the new clip in memory
//         foreach (AudioClip clip in allClips)
//         {
//             if (clip != newClip)
//                 Resources.UnloadAsset(clip);
//         }

//         musicClips = new AudioClip[] { newClip };
//     }


//     public void PlayCoinsSound()
//     {
//         Debug.Log("Coins sound played.");
//         if (SFXSource != null && CoinsClip != null)
//         {
//             SFXSource.PlayOneShot(CoinsClip);
//             Debug.Log("Coins sound played.");

//         }
//     }

// }


using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class CustomerAudioManager : MonoBehaviour
{
    public AudioSource BackgroundMusicSource;
    public AudioSource SFXSource;
    public AudioClip ButtonClickClip;
    public AudioClip CoinsClip;

    // keep the full pool in memory!
    private AudioClip[] musicClips;
    public AudioMixerSnapshot gameplaySnapshot;
    public AudioMixerSnapshot pauseSnapshot;
    private Coroutine musicRoutine;

    void Awake()
    {
        // load all clips once
        musicClips = Resources.LoadAll<AudioClip>(
            "Audio/Guru/MainMenu/BackgroundTracks"
        );

        if (musicClips == null || musicClips.Length == 0)
            Debug.LogWarning("CustomerAudioManager: No background tracks found!");
    }

    void Start()
    {
        // kick off the looping-through-the-playlist coroutine
        if (musicClips.Length > 0)
            musicRoutine = StartCoroutine(PlayMusicSequence());

    }

    /// <summary>
    /// Plays a random track, waits for its length, then repeats forever.
    /// </summary>
    private IEnumerator PlayMusicSequence()
    {
        // make sure we don’t loop the same clip forever
        BackgroundMusicSource.loop = false;

        while (true)
        {
            // pick a random clip
            var next = musicClips[Random.Range(0, musicClips.Length)];
            BackgroundMusicSource.clip = next;
            BackgroundMusicSource.Play();
            Debug.Log($"Now playing: {next.name}");

            // wait exactly the clip’s length
            yield return new WaitForSeconds(next.length);
        }
    }

    public void TransitionToDimmedSnapshot()
    {
        pauseSnapshot.TransitionTo(0.5f);  // 0.5 seconds transition
    }

    public void TransitionToGameplaySnapshot()
    {
        gameplaySnapshot.TransitionTo(0.5f);
    }

    /// <summary>
    /// Immediately swap to a new random clip (e.g. on mode change).
    /// </summary>
    public void ChangeBackgroundMusic()
    {
        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(PlayMusicSequence());
    }

    public void PlayButtonClickSound()
    {
        if (SFXSource != null && ButtonClickClip != null)
            SFXSource.PlayOneShot(ButtonClickClip);
    }

    public void PlayCoinsSound()
    {
        if (SFXSource != null && CoinsClip != null)
            SFXSource.PlayOneShot(CoinsClip);
    }
}
