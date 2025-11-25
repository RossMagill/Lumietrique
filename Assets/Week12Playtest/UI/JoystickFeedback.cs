using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // REQUIRED: This namespace is for the new system

public class JoystickFeedback : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform handleRect; 
    
    [Header("Settings")]
    public float maxRadius = 100f;

    void Update()
    {
        Vector2 input = Vector2.zero;

        // Check if a gamepad is actually connected to avoid errors
        if (Gamepad.current != null)
        {
            // Read the left stick directly from the current gamepad
            input = Gamepad.current.leftStick.ReadValue();
        }
        
        // Note: If you are testing with WASD on keyboard, add this:
        /*
        if (Keyboard.current != null && input == Vector2.zero)
        {
             if (Keyboard.current.wKey.isPressed) input.y += 1;
             if (Keyboard.current.sKey.isPressed) input.y -= 1;
             if (Keyboard.current.aKey.isPressed) input.x -= 1;
             if (Keyboard.current.dKey.isPressed) input.x += 1;
        }
        */

        // The rest of the logic is the same as before
        if (input.magnitude > 1f)
        {
            input.Normalize();
        }

        handleRect.anchoredPosition = input * maxRadius;
    }
}