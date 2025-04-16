using System.Security;
using UnityEngine;
using UnityEngine.Playables;

public class OverLayManager : MonoBehaviour
{
    public GameObject GameUI;
    public GameObject PauseUI;

    public CustomerData customerData;
    private CustomerData localCustomerData;
    [SerializeField] private ToggleSwitch gameModeToggle;

    public
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the UI to show the game screen by default
        DefaultView();
        CreateLocalCopy();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PauseButtonClick()
    {
        CreateLocalCopy();
        RefreshUI();
        GameUI.SetActive(false);
        PauseUI.SetActive(true);

    }
    private void CreateLocalCopy()
    {
        // Destroy any previous local copy
        if (localCustomerData != null)
        {
            Destroy(localCustomerData);
        }
        localCustomerData = Instantiate(customerData);
        Debug.Log($"Local copy created: {localCustomerData.gameMode}");
    }
    private void RefreshUI()
    {
        bool isWaves = localCustomerData.gameMode == GameMode.Waves;

        gameModeToggle.CurrentValue = isWaves;
    }

    public void Accept()
    {
        // Copy selected settings (here, just gameMode) from local to global.
        customerData.SetGameMode(localCustomerData.gameMode);
        DefaultView();
        // CloseSettings();
        Destroy(localCustomerData);
        localCustomerData = null;
    }

    /// <summary>
    /// Called by the UI "Decline" button.
    /// Discards any changes from the local copy.
    /// </summary>
    public void Decline()
    {
        Debug.Log("Settings Declined. Changes discarded.");
        DefaultView();
        // Resume game speed when returning to game
        Destroy(localCustomerData);
        localCustomerData = null;
    }

    public void ToggleModeButton(int value)
    {
        localCustomerData.SetGameMode((GameMode)value);
        RefreshUI();
    }

    void DefaultView()
    {
        GameUI.SetActive(true);
        PauseUI.SetActive(false);
    }
}
