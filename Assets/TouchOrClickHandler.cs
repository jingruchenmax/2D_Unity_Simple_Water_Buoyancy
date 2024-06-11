using UnityEngine;

public class TouchOrClickHandler : MonoBehaviour
{
    public bool isPressed { get; private set; }
    public float pressedTime { get; private set; }

    private bool wasPressedLastFrame;

    void Update()
    {
        isPressed = false;

        // Check for mouse button click or touch input
        if (Input.GetMouseButton(0))
        {
            isPressed = true;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isPressed = true;
        }

        // Update pressedTime
        if (isPressed)
        {
            if (!wasPressedLastFrame)
            {
                pressedTime = 0f; // Reset the pressed time if it's the first frame the button is pressed
            }
            else
            {
                pressedTime += Time.deltaTime; // Accumulate the time the button is held
            }
        }

        // Store the state for the next frame
        wasPressedLastFrame = isPressed;
    }
}
