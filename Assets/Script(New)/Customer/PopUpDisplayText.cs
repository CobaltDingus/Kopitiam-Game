using TMPro;
using UnityEngine;

public class PopUpDisplayText : MonoBehaviour
{
    [Header("Target Object")]
    public GameObject targetObject1;

    [Header("Movement Settings")]
    public Vector2 initialVelocity;
    public Rigidbody2D rb;
    public float lifetime = 1.5f;

    [Header("Fade Settings")]
    public TMP_Text text;
    public float fadeDuration = 1.5f;
    public bool isFading = true;

    private float currentAlpha;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (text == null) text = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {

        if (targetObject1 != null && text != null)
        {
            text.transform.position = targetObject1.transform.position;
        }


        if (rb != null)
        {
            rb.linearVelocity = initialVelocity;
        }


        if (text != null)
        {
            currentAlpha = text.color.a;
        }


        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!isFading || text == null) return;

        // Reduce alpha over time
        currentAlpha -= Time.deltaTime / fadeDuration;
        currentAlpha = Mathf.Clamp01(currentAlpha);

        // Apply updated alpha back to TMP component
        Color currentColor = text.color;
        currentColor.a = currentAlpha;
        text.color = currentColor;

        if (currentAlpha <= 0f)
        {
            isFading = false;
        }
    }
}
