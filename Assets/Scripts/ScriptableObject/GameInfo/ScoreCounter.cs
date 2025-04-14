using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    [Header("Data & UI")]
    [Tooltip("Assign your CustomerData SO here")]
    public CustomerData customerData;
    [Tooltip("Assign the TextMeshProUGUI for showing score")]
    public TextMeshProUGUI scoreText;

    void OnEnable()
    {
        if (customerData != null)
            customerData.OnScoreChanged += HandleScoreChanged;
    }

    void OnDisable()
    {
        if (customerData != null)
            customerData.OnScoreChanged -= HandleScoreChanged;
    }

    void Start()
    {
        // Initialize UI from existing SO value
        UpdateScoreUI();
    }

    // Called whenever customerData.score changes
    void HandleScoreChanged(int newScore)
    {
        UpdateScoreUI();
    }

    /// <summary>
    /// Call this when a customer completes an order.
    /// </summary>
    public void AddScore(int amount)
    {
        if (customerData != null)
            customerData.AddScore(amount);
        else
            Debug.LogWarning("ScoreCounter: CustomerData is not assigned!");
    }

    /// <summary>
    /// Call this when a customer's order fails.
    /// </summary>
    public void DeductScore(int amount)
    {
        if (customerData != null)
            customerData.DeductScore(amount);
        else
            Debug.LogWarning("ScoreCounter: CustomerData is not assigned!");
    }

    /// <summary>
    /// Resets the score in the SO (and UI).
    /// </summary>
    public void ResetScore()
    {
        if (customerData != null)
            customerData.ResetScore();
        else
            Debug.LogWarning("ScoreCounter: CustomerData is not assigned!");
    }

    /// <summary>
    /// Updates the on‑screen text from the SO.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText == null)
        {
            Debug.LogWarning("ScoreCounter: scoreText UI element is not assigned!");
            return;
        }

        if (customerData != null)
        {
            scoreText.text = $"Score: {customerData.score}";
        }
        else
        {
            scoreText.text = "Score: 0";
        }
    }
}
