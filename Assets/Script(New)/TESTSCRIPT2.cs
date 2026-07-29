using UnityEngine;
using UnityEngine.InputSystem;

public class TESTSCRIPT2 : MonoBehaviour
{
    void Update()
    {
        if (Touchscreen.current == null)
            return;

        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            Debug.Log("Touch Began");

        if (Touchscreen.current.primaryTouch.press.isPressed)
            Debug.Log("Touch Held");

        if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            Debug.Log("Touch Ended");
    }
}