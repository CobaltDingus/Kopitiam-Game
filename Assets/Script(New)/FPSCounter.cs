using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public static FPSCounter Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private float pollingTime = 0.5f;

    private float timeElapsed;
    private int frameCount;

    void Awake()
    {
        // Check if an instance already exists in the new scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    void Update()
    {
        timeElapsed += Time.unscaledDeltaTime;
        frameCount++;

        if (timeElapsed >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / timeElapsed);
            
            // Safe check in case the text component is missing
            if (fpsText != null)
            {
                fpsText.text = $"{frameRate} FPS";
            }

            timeElapsed = 0f;
            frameCount = 0;
        }
    }
}
