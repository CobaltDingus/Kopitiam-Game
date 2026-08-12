using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Gesture : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private enum RotationDirection { None, Clockwise, CounterClockwise }

    [Header("Drawing Area")]
    [SerializeField] private BoxCollider2D drawArea;

    [Header("Circle Detection Stuff")]
    [SerializeField] private float minRadius = 0.5f;
    [SerializeField] private float maxRadius = 10f;
    [SerializeField] private float angleThreshold = 360f;

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
    private Vector2 startPos;
    private float totalAngle = 0f;
    private float prevAngle = 0f;
    private bool hasPrevAngle = false;
    private RotationDirection currentDirection = RotationDirection.None;

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

        // Cache all Renderers (SpriteRenderers, CanvasRenderers) on this object and its children
        objectRenderers = GetComponentsInChildren<Renderer>();

        if (fingerIndicator != null)
            fingerIndicator.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isLocked || !enabled) return;

        Vector2 worldPos = ScreenToWorld2D(eventData.position);
        if (drawArea != null && !drawArea.OverlapPoint(worldPos)) return;

        StartDrawing(worldPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDrawing || isLocked || !enabled) return;

        Vector2 worldPos = ScreenToWorld2D(eventData.position);
        CurrentFingerWorldPosition = worldPos;

        if (drawArea != null && !drawArea.OverlapPoint(worldPos))
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
        Debug.Log($"Circle detector turned offf.{circleCount}/{maxCirclesAllowed} circles.");
    }


    public void UnlockAndEnableDetector()
    {
        isLocked = false;
        circleCount = 0;
        ResetGestureProgress();
        SetVisibility(true);
        onDetectorUnlocked?.Invoke();
        Debug.Log("Circle detector turned ona and reset!");
    }

    private void SetVisibility(bool isVisible)
    {
        if (objectRenderers == null) return;

        foreach (Renderer rend in objectRenderers)
        {
            // Do not enable fingerIndicator when making object visible unless actively drawing
            if (isVisible && fingerIndicator != null && rend.gameObject == fingerIndicator)
                continue;

            rend.enabled = isVisible;
        }
    }

    //Gesture Logic

    private void StartDrawing(Vector2 worldPos)
    {
        isDrawing = true;
        startPos = worldPos;
        CurrentFingerWorldPosition = worldPos;

        ResetGestureProgress();

        if (fingerIndicator != null)
        {
            fingerIndicator.SetActive(true);
            fingerIndicator.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
        }
    }

    private void UpdateDrawing(Vector2 currentPos)
    {
        if (fingerIndicator != null)
            fingerIndicator.transform.position = new Vector3(currentPos.x, currentPos.y, 0f);

        Vector2 delta = currentPos - startPos;
        float radius = delta.magnitude;

        if (radius < minRadius || radius > maxRadius)
            return;

        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        if (hasPrevAngle)
        {
            float diff = Mathf.DeltaAngle(prevAngle, angle);

            if (Mathf.Abs(diff) > 0.1f)
            {
                if (currentDirection == RotationDirection.None)
                {
                    currentDirection = diff > 0 ? RotationDirection.CounterClockwise : RotationDirection.Clockwise;
                }

                bool isBacktracking = (currentDirection == RotationDirection.CounterClockwise && diff < 0) ||
                                     (currentDirection == RotationDirection.Clockwise && diff > 0);

                if (isBacktracking)
                {
                    ResetGestureProgress();
                    prevAngle = angle;
                    hasPrevAngle = true;
                    return;
                }

                totalAngle += Mathf.Abs(diff);

                if (totalAngle >= angleThreshold)
                {
                    CompleteCircle();
                }
            }
        }
        else
        {
            hasPrevAngle = true;
        }

        prevAngle = angle;
    }

    private void CompleteCircle()
    {
        circleCount++;
        Debug.Log("Circle Complete!");
        onCircleCompleted?.Invoke();

        if (circleCount >= maxCirclesAllowed)
        {

            DisableDetector();
        }
        else
        {
            Debug.Log("No Backtracking");
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
        currentDirection = RotationDirection.None;
    }

    private Vector2 ScreenToWorld2D(Vector2 screenPos)
    {
        if (mainCam == null) return Vector2.zero;
        Vector3 world3 = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCam.transform.position.z));
        return new Vector2(world3.x, world3.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (drawArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(drawArea.bounds.center, drawArea.bounds.size);
        }
    }
}