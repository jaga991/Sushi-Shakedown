using UnityEngine;
// alias Unity’s SceneManager so we can keep our class name
using UnityEngine.SceneManagement;

public class GameSceneManager : Singleton<GameSceneManager>
{
    // Optional: if you need to do anything special on awake
    public override void Awake()
    {
        base.Awake();

        if (Display.displays.Length > 1)
        {
            Debug.Log("Activating Display 2");
            Display.displays[1].Activate();
        }
        else
        {
            Debug.LogWarning("Display 2 not available");
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name} – assigning cameras to Display 2");

        foreach (Camera cam in Camera.allCameras)
        {
            cam.targetDisplay = 1; // 0 = Display 1, 1 = Display 2
        }
    }
    public void PrintActiveScreens()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            Debug.Log($"Active scene: {scene.name}");
        }
    }

    /// <summary>
    /// Call from your MainMenu “Start Game” button
    /// </summary>
    public void StartGame()
    {
        // 1) Load Customers as the base scene (unloads MainMenu)
        SceneManager.LoadScene("Customers", LoadSceneMode.Single);

        // 2) Immediately add KitchenScene on top
        SceneManager.LoadScene("KitchenScene", LoadSceneMode.Additive);


    }

    /// <summary>
    /// Call when you want to go to your Upgrades screen
    /// </summary>
    public void LoadUpgrades()
    {
        SceneManager.LoadScene("Upgrades", LoadSceneMode.Single);
    }

    /// <summary>
    /// Return to the Main Menu (you might use this on a “Back” button)
    /// </summary>
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        // Reset target display to main display (1) when returning to main menu
        Display.displays[0].Activate();
    }

    /// <summary>
    /// Optional: clean exit
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
