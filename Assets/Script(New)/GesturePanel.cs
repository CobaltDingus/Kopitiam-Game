using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GesturePanel : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    private enum RotationDirection
    {
        None,
        Clockwise,
        CounterClockwise
    }

    [Header("Drawing Area")]
    [SerializeField] private RectTransform drawArea;

    [Header("Circle Detection Stuff")]
    [SerializeField] private float minRadius = 50f;
    [SerializeField] private float maxRadius = 300f;
    [SerializeField] private float angleThreshold = 360f;

    [Tooltip("How many degrees the player must move before a rotation direction is chosen.")]
    [SerializeField] private float directionDetectionAngle = 10f;

    [Header("Stir Limits")]
    [SerializeField] private int maxCirclesAllowed = 4;

    [Header("Visual Feedback")]
    [SerializeField] private RectTransform fingerIndicator;

    // [Header("Mixing Cup Object")]
    // [SerializeField] private MixingCup mixingCup;

    [Header("Events")]
    public UnityEvent onCircleCompleted;
    public UnityEvent onMaxCirclesReached;
    public UnityEvent onDetectorUnlocked;

    private Camera mainCam;

    private bool isDrawing = false;

    // Center of the drawing area in screen coordinates.
    private Vector2 rotationCenter;

    private float totalAngle = 0f;
    private float prevAngle = 0f;

    private bool hasPrevAngle = false;

    private RotationDirection currentDirection =
        RotationDirection.None;

    // Used before we decide whether the player is moving
    // clockwise or counter-clockwise.
    private float directionDetectionProgress = 0f;

    private int circleCount = 0;
    private bool isLocked = false;

    public Vector2 CurrentFingerScreenPosition { get; private set; }

    public int CurrentCircleCount => circleCount;

    public bool IsLocked => isLocked;


    private void Awake()
    {
        mainCam = Camera.main;

        if (drawArea == null)
            drawArea = GetComponent<RectTransform>();

        if (fingerIndicator != null)
            fingerIndicator.gameObject.SetActive(false);

        UpdateRotationCenter();
        // DisableDetector();
    }


    private void UpdateRotationCenter()
    {
        if (drawArea != null)
        {
            // Screen Space - Camera canvas.
            rotationCenter =
                RectTransformUtility.WorldToScreenPoint(
                    mainCam,
                    drawArea.position
                );
        }
        else
        {
            rotationCenter = Vector2.zero;
        }
    }


    private bool IsInsideDrawArea(Vector2 screenPosition)
    {
        if (drawArea == null)
            return true;

        return RectTransformUtility.RectangleContainsScreenPoint(
            drawArea,
            screenPosition,
            mainCam
        );
    }


    // ============================================================
    // Pointer Events
    // ============================================================

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLocked || !enabled)
            return;

        Vector2 screenPos = eventData.position;

        if (!IsInsideDrawArea(screenPos))
            return;

        StartDrawing(screenPos);
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!isDrawing || isLocked || !enabled)
            return;

        Vector2 screenPos = eventData.position;

        CurrentFingerScreenPosition = screenPos;

        // if (!IsInsideDrawArea(screenPos))
        // {
        //     EndDrawing();
        //     return;
        // }

        UpdateDrawing(screenPos);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDrawing)
        {
            EndDrawing();
        }
    }


    // ============================================================
    // Enable / Disable
    // ============================================================

    public void DisableDetector()
    {
        isLocked = true;

        EndDrawing();

        SetVisibility(false);

        onMaxCirclesReached?.Invoke();

        Debug.Log(
            $"Circle detector turned off. " +
            $"{circleCount}/{maxCirclesAllowed} circles."
        );
    }


    public void UnlockAndEnableDetector()
    {
        isLocked = false;

        circleCount = 0;

        UpdateRotationCenter();

        ResetGestureProgress();

        SetVisibility(true);

        onDetectorUnlocked?.Invoke();

        Debug.Log(
            "Circle detector turned on and reset!"
        );
    }


    private void SetVisibility(bool isVisible)
    {
        if (fingerIndicator != null)
            fingerIndicator.gameObject.SetActive(isVisible);
    }


    // ============================================================
    // Gesture Logic
    // ============================================================

    private void StartDrawing(Vector2 screenPos)
    {
        isDrawing = true;

        // Make sure the center is up to date.
        UpdateRotationCenter();

        CurrentFingerScreenPosition = screenPos;

        ResetGestureProgress();

        if (fingerIndicator != null)
        {
            fingerIndicator.gameObject.SetActive(true);

            PositionFingerIndicator(screenPos);
        }
    }


    private void UpdateDrawing(Vector2 currentPos)
    {
        if (fingerIndicator != null)
        {
            PositionFingerIndicator(currentPos);
        }

        // Calculate distance from the center of the drawing area.
        Vector2 delta =
            currentPos - rotationCenter;

        float radius = delta.magnitude;

        // Ignore movement that is too close or too far.
        if (radius < minRadius ||
            radius > maxRadius)
        {
            return;
        }

        float angle =
            Mathf.Atan2(delta.y, delta.x) *
            Mathf.Rad2Deg;

        // First valid point.
        if (!hasPrevAngle)
        {
            prevAngle = angle;
            hasPrevAngle = true;
            return;
        }

        // Calculate angular movement.
        float diff =
            Mathf.DeltaAngle(prevAngle, angle);

        // Ignore extremely tiny movement.
        if (Mathf.Abs(diff) <= 0.1f)
            return;


        // ========================================================
        // Determine rotation direction
        // ========================================================

        if (currentDirection == RotationDirection.None)
        {
            // Accumulate movement until we have enough information
            // to determine the intended direction.
            directionDetectionProgress += diff;

            if (Mathf.Abs(directionDetectionProgress)
                >= directionDetectionAngle)
            {
                if (directionDetectionProgress > 0)
                {
                    currentDirection =
                        RotationDirection.CounterClockwise;
                }
                else
                {
                    currentDirection =
                        RotationDirection.Clockwise;
                }

                // Only start counting toward the circle after
                // direction has been established.
                totalAngle = 0f;

                Debug.Log(
                    "Rotation direction detected: " +
                    currentDirection
                );
            }

            prevAngle = angle;
            return;
        }


        // ========================================================
        // Detect backtracking
        // ========================================================

        bool isBacktracking =
            (currentDirection ==
                RotationDirection.CounterClockwise &&
             diff < 0) ||

            (currentDirection ==
                RotationDirection.Clockwise &&
             diff > 0);

        if (isBacktracking)
        {
            // Instead of resetting completely, subtract the
            // backwards movement.
            totalAngle -= Mathf.Abs(diff);

            if (totalAngle < 0f)
                totalAngle = 0f;

            prevAngle = angle;
            return;
        }


        // ========================================================
        // Normal rotation
        // ========================================================

        totalAngle += Mathf.Abs(diff);

        if (totalAngle >= angleThreshold)
        {
            CompleteCircle();
            return;
        }

        prevAngle = angle;
    }


    private void CompleteCircle()
    {
        circleCount++;

        Debug.Log(
            "Circle Complete! Circle: " +
            circleCount
        );

        onCircleCompleted?.Invoke();

        if (circleCount >= maxCirclesAllowed)
        {
            DisableDetector();
        }
        else
        {
            Debug.Log(
                "Circle completed. Ready for another."
            );

            ResetGestureProgress();
        }
    }


    private void EndDrawing()
    {
        isDrawing = false;

        ResetGestureProgress();

        if (fingerIndicator != null)
            fingerIndicator.gameObject.SetActive(false);
    }


    private void ResetGestureProgress()
    {
        totalAngle = 0f;

        prevAngle = 0f;

        hasPrevAngle = false;

        currentDirection =
            RotationDirection.None;

        directionDetectionProgress = 0f;
    }


    // ============================================================
    // UI Positioning
    // ============================================================

    private void PositionFingerIndicator(Vector2 screenPosition)
    {
        if (fingerIndicator == null)
            return;

        RectTransform canvasRect =
            fingerIndicator.GetComponentInParent<Canvas>()
                .GetComponent<RectTransform>();

        if (canvasRect == null)
            return;

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            mainCam,
            out localPoint
        );

        fingerIndicator.localPosition = localPoint;
    }


    // ============================================================
    // Editor Visualization
    // ============================================================

    private void OnDrawGizmosSelected()
    {
        if (drawArea != null)
        {
            // Draw the UI rectangle in the Scene view.
            Gizmos.color = Color.green;

            Vector3[] corners = new Vector3[4];
            drawArea.GetWorldCorners(corners);

            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
        }
    }
}