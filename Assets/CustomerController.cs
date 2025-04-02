using UnityEngine;
using UnityEngine.UI; // For Text component

public class CustomerController : MonoBehaviour
{
    public float speed = 3f;
    public Vector2 targetPosition; // The predetermined counter spot.

    // Reference to the text bubble child object.
    public GameObject textBubble;
    private Text bubbleText;  // Assumes a Text component is attached to textBubble or its child.

    void Awake()
    {
        if (textBubble != null)
        {
            bubbleText = textBubble.GetComponentInChildren<Text>();
            if (bubbleText != null)
            {
                bubbleText.text = "..."; // Display initial three dots.
            }
        }
    }

    void Update()
    {
        // Move toward the target counter position.
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        // Check if the customer has arrived at the counter.
        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            ArrivedAtCounter();
        }
    }

    // Set the target counter position from the spawner.
    public void SetTargetCounterPosition(Vector2 pos)
    {
        targetPosition = pos;
    }

    void ArrivedAtCounter()
    {
        // Update text bubble to indicate arrival or display order.
        // if (bubbleText != null)
        // {
        //     bubbleText.text = "Order"; // Change this to display the customer's order as needed.
        // }
        // Optionally, disable further movement or add additional logic.
        enabled = false;
    }
}
