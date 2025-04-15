using UnityEngine;
using UnityEngine.UI;         // for Button

using UnityEngine.SceneManagement; // for loading scenes
using UnityEngine.EventSystems;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class MainMenuManager : MonoBehaviour
{
    [Header("Music")]
    [Tooltip("Reference to an AudioSource in the scene")]
    public AudioSource musicSource;

    [Tooltip("List of possible background music tracks")]
    public AudioClip[] musicClips;

    [Header("UI Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;

    void Awake()
    {
    }

    void Start()
    {
        // Pick a random track from the array and play it
        // PlayRandomMusic();
        Debug.Log("Starting Main Menu...");
    }

    /// <summary>
    /// Plays a random music track from the MusicClips array on the MusicSource.
    /// </summary>
    private void PlayRandomMusic()
    {
        if (musicSource == null || musicClips == null || musicClips.Length == 0)
        {
            Debug.LogWarning("MainMenuManager: Missing AudioSource or no music clips assigned.");
            return;
        }

        int randomIndex = Random.Range(0, musicClips.Length);
        musicSource.clip = musicClips[randomIndex];
        musicSource.loop = true;    // Usually loop background music
        musicSource.Play();
    }

    /// <summary>
    /// Called when the Play button is clicked.
    /// Loads the gameplay scene or transitions to your main game.
    /// </summary>
    public void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked! Loading Game scene...");
        EventSystem.current.SetSelectedGameObject(null);
        // Replace "GameScene" with your actual game scene name/index:
        // SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// Called when the Settings button is clicked.
    /// You could load a separate Settings scene, or show a settings panel.
    /// </summary>
    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked! Loading Settings scene...");
        EventSystem.current.SetSelectedGameObject(null);
        // Example: load a Settings scene or open a UI panel
        // SceneManager.LoadScene("SettingsScene");
        // OR
        // settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Called when the Exit button is clicked.
    /// Exits the application. Does nothing in the editor unless in play mode.
    /// </summary>
    public void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked! Quitting...");
        EventSystem.current.SetSelectedGameObject(null);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
