using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    // The overall score
    public int score { get; private set; } = 0;

    // Optionally track customer counts
    public int normalCustomersCount { get; private set; } = 0;
    public int angryCustomersCount { get; private set; } = 0;

    public TextMeshProUGUI scoreText;

    void Start()
    {
        UpdateScoreUI();
    }

    /// <summary>
    /// Call this method when a customer completes an order successfully.
    /// </summary>
    /// <param name="amount">The amount to add to the score.</param>
    public void AddScore(int amount)
    {
        score += amount;
        normalCustomersCount++;
        UpdateScoreUI();
    }

    /// <summary>
    /// Call this method when a customer's order fails.
    /// </summary>
    /// <param name="amount">The amount to deduct from the score.</param>
    public void DeductScore(int amount)
    {
        score -= amount;
        angryCustomersCount++;
        UpdateScoreUI();
    }

    /// <summary>
    /// Resets the score and customer counts.
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        normalCustomersCount = 0;
        angryCustomersCount = 0;
        UpdateScoreUI();
    }

    /// <summary>
    /// Updates the score text on the UI.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
        else
        {
            Debug.Log("ScoreText UI element is not assigned!");
        }
    }
}
