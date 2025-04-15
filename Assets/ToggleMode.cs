using UnityEngine;

public class ToggleMode : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public CustomerData customerData;
    public ToggleSwitch gameModeToggle;


    void Start()
    {
        bool isWaves = customerData.gameMode == GameMode.Waves;
        gameModeToggle.SetStateSilently(isWaves);
    }

    public void ToggleModeButton(int value)
    {

        customerData.SetGameMode((GameMode)value);
    }

    public void Refresh()
    {
        bool isWaves = customerData.gameMode == GameMode.Waves;
        gameModeToggle.ToggleByGroupManager(isWaves);
    }
}
