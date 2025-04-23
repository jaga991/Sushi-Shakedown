using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using UnityEngine.SceneManagement; // for loading scenes

public class OverLayManager : DebuggableMonoBehaviour, IPointerClickHandler
{
    public CustomerData customerData;
    private CustomerData Pause_CustomerData_Local;
    public static event Action<bool> OnUIBlockToggle;


    private void BlockUI(bool isBlocked)
    {
        OnUIBlockToggle?.Invoke(isBlocked);
    }


    // Screens
    public GameObject Game_Screen;
    public GameObject Pause_Screen;
    public GameObject Info_Screen;
    public GameObject PreDay_Screen;

    public GameObject Failure_Screen;
    public CustomerAudioManager cm;

    public GameObject Final_Day_Screen;

    // UI Toggles (Settings)
    [SerializeField] private ToggleSwitch GameMode_Toggle;
    [SerializeField] private ToggleSwitch DifficultyMode_Toggle;

    // InfoUI elements
    public TextMeshProUGUI Info_DayText;
    public TextMeshProUGUI Info_ScoreText;

    public TextMeshProUGUI Info_StashText;
    public TextMeshProUGUI Info_RansomText;
    public TextMeshProUGUI Info_NewTotalStashText;
    public TextMeshProUGUI Info_TotalServedText;
    public TextMeshProUGUI Info_HappyText;


    public event System.Action OnInfoClosed;



    // PreDayUI elements
    public TextMeshProUGUI PreDay_DayText;
    public TextMeshProUGUI PreDay_ScoreText;

    public TextMeshProUGUI PreDay_HiddenStashText;
    public event System.Action OnPreDayClosed;


    public TextMeshProUGUI Failure_Title;
    public TextMeshProUGUI Failure_Description;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        cm = GameObject.Find("CustomerAudioManager").GetComponent<CustomerAudioManager>();
        // Initialize the UI to show the game screen by default
        DefaultView();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // customerData.OnGameModeChanged += HandleModeChanged;
    }

    protected override void UpdateLogStatus()
    {
        isDebugEnabled = logSettings.OverLayManagerLogs;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        // customerData.OnGameModeChanged -= HandleModeChanged;
    }

    public void PauseButtonClick()
    {

        CreateLocalCopy();
        RefreshUI();
        HideAllScreens();

        cm.PlayButtonClickSound();
        Pause_Screen.SetActive(true);
        Time.timeScale = 0f; // Pause the game

        cm.TransitionToDimmedSnapshot();  // ADD HERE
        BlockUI(true); // Block UI interactions

    }
    private void CreateLocalCopy()
    {
        // Destroy any previous local copy
        if (Pause_CustomerData_Local != null)
        {
            Destroy(Pause_CustomerData_Local);
        }
        Pause_CustomerData_Local = Instantiate(customerData);
        Log($"Local copy created: {Pause_CustomerData_Local.gameMode}");

    }
    private void RefreshUI()
    {
        bool isWaves = Pause_CustomerData_Local.gameMode == GameMode.Waves;

        GameMode_Toggle.CurrentValue = isWaves;

        bool isEasy = Pause_CustomerData_Local.difficulty == Difficulty.Easy;
        DifficultyMode_Toggle.CurrentValue = isEasy;
    }

    public void Accept()
    {
        // Copy selected
        cm.PlayButtonClickSound();
        customerData.SetGameMode(Pause_CustomerData_Local.gameMode);
        customerData.SetDifficulty(Pause_CustomerData_Local.difficulty);
        DefaultView();
        // CloseSettings();
        Destroy(Pause_CustomerData_Local);
        Pause_CustomerData_Local = null;
        Time.timeScale = 1f; // Resume the game speed
        cm.TransitionToGameplaySnapshot();

    }

    /// <summary>
    /// Called by the UI "Decline" button.
    /// Discards any changes from the local copy.
    /// </summary>
    public void Decline()
    {
        cm.PlayButtonClickSound();
        Log("Settings Declined. Changes discarded.");
        DefaultView();
        // Resume game speed when returning to game
        Destroy(Pause_CustomerData_Local);
        Pause_CustomerData_Local = null;

        Time.timeScale = 1f; // Resume the game speed
        cm.TransitionToGameplaySnapshot();

    }

    public void ToggleModeButton(int value)
    {
        cm.PlayButtonClickSound();
        Pause_CustomerData_Local.SetGameMode((GameMode)value);
        RefreshUI();
    }

    public void ToggleDifficultyButton(int value)
    {
        cm.PlayButtonClickSound();
        Log("Tn called with value: " + (Difficulty)value);
        Pause_CustomerData_Local.SetDifficulty((Difficulty)value);
    }

    private void DefaultView()

    {
        HideAllScreens();
        Game_Screen.SetActive(true);
        BlockUI(false); // Unblock UI interactions
    }
    public void ShowInfoUI(int day, int score, int totalServed, int happy, int angry, int TotalCoins, int Ransom)
    {
        BlockUI(true); // Block UI interactions
        cm.TransitionToDimmedSnapshot();
        HideAllScreens();
        Info_Screen.SetActive(true);
        Debug.Log($"[Overlay] ShowInfoUI: day={day}, score={score}, served={totalServed}, happy={happy}, angry={angry} , TotalCoins={TotalCoins}");
        Info_DayText.text = $"Day: {day} Completed!";
        Info_ScoreText.text = $"{score}";
        Info_StashText.text = $"Hidden Stash: {TotalCoins + Ransom - score} COINS";
        Info_RansomText.text = $"Ransom: {Ransom} COINS";
        Info_NewTotalStashText.text = $"New Stash: {TotalCoins} COINS";

        Info_TotalServedText.text = $"Customers Served: {totalServed}";
        Info_HappyText.text = $"Happy Customers: {happy}";
        // Info_CustomerCoinsText.text = $"Total Coins: {TotalCoins}";
    }

    public void CloseInfoUI()
    {

        cm.TransitionToGameplaySnapshot();
        Debug.Log("[Settings]  CloseInfoUI called");
        DefaultView();
        OnInfoClosed?.Invoke();
    }
    public void ShowPreDayUI(int day)
    {
        BlockUI(true); // Block UI interactions
        cm.TransitionToDimmedSnapshot();
        // Debug.Log($"[Overlay] ShowPreDayUI: starting day {day}");
        PreDay_DayText.text = $"Day {day} Starting";
        PreDay_ScoreText.text = $"Make {customerData.GetRansom(day)} COINS OR ELSE!";
        PreDay_HiddenStashText.text = $"Hidden Stash: {customerData.getCustomerCoins()} COINS";
        HideAllScreens();
        PreDay_Screen.SetActive(true);
    }
    public void ClosePreDayUI()
    {

        cm.TransitionToGameplaySnapshot();
        Debug.Log("[Settings]  ClosePreDayUI called");
        DefaultView();
        OnPreDayClosed?.Invoke();
    }
    public void ShowFinalDayUI()
    {
        BlockUI(true); // Block UI interactions
        cm.TransitionToDimmedSnapshot();
        HideAllScreens();
        Final_Day_Screen.SetActive(true);
        // Debug.Log($"[Overlay] ShowFinalDayUI: starting day {customerData.Day}");
    }

    public void FinalDayScreen_ExitButtonClick()
    {
        cm.PlayButtonClickSound();
        Debug.Log("[Overlay] Exit Game");
        Application.Quit();
        // (in the Editor this won’t do anything, but in a build it will quit)
    }

    public void FinalDayScreen_RestartButtonClick()
    {
        cm.TransitionToGameplaySnapshot();
        cm.PlayButtonClickSound();
        Debug.Log("[Overlay] Restart Game");
        customerData.ResetEverything();
        GameSceneManager.instance.BackToMainMenu();
    }


    public void ShowFailureUI(int day, int EarnedCoins, int YakuzaDeduction)
    {
        BlockUI(true); // Block UI interactions
        cm.TransitionToDimmedSnapshot();
        HideAllScreens();
        Failure_Screen.SetActive(true);
        Failure_Title.text = $"Day {day} Failed!";
        Failure_Description.text = $"You were short {YakuzaDeduction - EarnedCoins} COINS TODAY !";
    }

    public void Failure_QuitButtonClick()
    {
        cm.PlayButtonClickSound();
        Debug.Log("[Overlay] Exit Game");
        Application.Quit();
        // (in the Editor this won’t do anything, but in a build it will quit)
    }

    public void Failure_RestartButtonClick()
    {
        cm.TransitionToGameplaySnapshot();
        cm.PlayButtonClickSound();
        Debug.Log("[Overlay] Restart Game");
        customerData.ResetEverything();
        GameSceneManager.instance.BackToMainMenu();
    }

    // Need to find out if scripts are refrenceing this 
    public void CloseFailureUI()
    {

        Debug.Log("[Settings]  CloseFailureUI called");
        DefaultView();
    }


    public void OnPointerClick(PointerEventData eventData)

    {
        Debug.Log("[Settings]  Settings UI received click");
    }

    public void HideAllScreens()
    {
        Game_Screen.SetActive(false);
        Pause_Screen.SetActive(false);
        Info_Screen.SetActive(false);
        PreDay_Screen.SetActive(false);
        Final_Day_Screen.SetActive(false);
        Failure_Screen.SetActive(false);
    }
}