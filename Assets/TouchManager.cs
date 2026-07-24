using UnityEngine;
// using UnityEngine.InputSystem.EnhancedTouch;
// Alias to avoid conflict with legacy Touch types
// using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch; 

public class TouchManager : MonoBehaviour
{
    private void OnEnable()
    {
        // Required initialization if you use EnhancedTouch API
        // EnhancedTouchSupport.Enable(); 
    }

    private void OnDisable()
    {
        // EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        CheckLegacyTouch();
        // CheckNewInputTouch();
    }

    // 1. LEGACY INPUT MANAGER DETECTION
    void CheckLegacyTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch legacyTouch = Input.GetTouch(0);
            
            if (legacyTouch.phase == TouchPhase.Began)
            {
                Debug.Log($"[Legacy] Touch detected at: {legacyTouch.position}");
            }
        }
    }
}
