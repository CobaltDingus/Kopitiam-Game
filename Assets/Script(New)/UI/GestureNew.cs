using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class GestureNew : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private enum RotationDirection
    {
        None,
        Clockwise,
        CounterClockwise
    }

    [Header("Drawing Area")]
    [SerializeField] private BoxCollider2D drawArea;

    [Header("Circle Detection Stuff")]
    [SerializeField] private float minRadius = 0.5f;
    [SerializeField] private float maxRadius = 10f;
    [SerializeField] private float angleThreshold = 360f;

    [Tooltip("How many degrees the player must move before a rotation direction is chosen.")]
    [SerializeField] private float directionDetectionAngle = 10f;

    [Header("Stir Limits")]
    [SerializeField] private int maxCirclesAllowed = 4;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject fingerIndicator;

    [Header("Events")]
    public UnityEvent onCircleCompleted;
    public UnityEvent onMaxCirclesReached;
    public UnityEvent onDetectorUnlocked;

    private Camera mainCam;
    private Renderer[] objectRenderers;

    private bool isDrawing = false;

    // Center of the drawing area.
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

    public Vector2 CurrentFingerWorldPosition { get; private set; }

    public int CurrentCircleCount => circleCount;

    public bool IsLocked => isLocked;

    private void Awake()
    {
        mainCam = Camera.main;

        if (drawArea == null)
            drawArea = GetComponent<BoxCollider2D>();

        objectRenderers = GetComponentsInChildren<Renderer>();

        if (fingerIndicator != null)
            fingerIndicator.SetActive(false);

        UpdateRotationCenter();
    }

    private void UpdateRotationCenter()
    {
        if (drawArea != null)
        {
            rotationCenter = drawArea.bounds.center;
        }
        else
        {
            rotationCenter = transform.position;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLocked || !enabled)
            return;

        Vector2 worldPos = ScreenToWorld2D(eventData.position);

        if (drawArea != null &&
            !drawArea.OverlapPoint(worldPos))
        {
            return;
        }

        StartDrawing(worldPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDrawing || isLocked || !enabled)
            return;

        Vector2 worldPos =
            ScreenToWorld2D(eventData.position);

        CurrentFingerWorldPosition = worldPos;

        if (drawArea != null &&
            !drawArea.OverlapPoint(worldPos))
        {
            EndDrawing();
            return;
        }

        UpdateDrawing(worldPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDrawing)
        {
            EndDrawing();
        }
    }

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
        if (objectRenderers == null)
            return;

        foreach (Renderer rend in objectRenderers)
        {
            if (isVisible &&
                fingerIndicator != null &&
                rend.gameObject == fingerIndicator)
            {
                continue;
            }

            rend.enabled = isVisible;
        }
    }

    // ============================================================
    // Gesture Logic
    // ============================================================

    private void StartDrawing(Vector2 worldPos)
    {
        isDrawing = true;

        // Make sure the center is up to date.
        UpdateRotationCenter();

        CurrentFingerWorldPosition = worldPos;

        ResetGestureProgress();

        if (fingerIndicator != null)
        {
            fingerIndicator.SetActive(true);

            fingerIndicator.transform.position =
                new Vector3(
                    worldPos.x,
                    worldPos.y,
                    0f
                );
        }
    }

    private void UpdateDrawing(Vector2 currentPos)
    {
        if (fingerIndicator != null)
        {
            fingerIndicator.transform.position =
                new Vector3(
                    currentPos.x,
                    currentPos.y,
                    0f
                );
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
            fingerIndicator.SetActive(false);
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

    private Vector2 ScreenToWorld2D(Vector2 screenPos)
    {
        if (mainCam == null)
            return Vector2.zero;

        Vector3 world3 =
            mainCam.ScreenToWorldPoint(
                new Vector3(
                    screenPos.x,
                    screenPos.y,
                    -mainCam.transform.position.z
                )
            );

        return new Vector2(
            world3.x,
            world3.y
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (drawArea != null)
        {
            // Drawing area
            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(
                drawArea.bounds.center,
                drawArea.bounds.size
            );

            // Rotation center
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                drawArea.bounds.center,
                0.1f
            );
        }
    }
}