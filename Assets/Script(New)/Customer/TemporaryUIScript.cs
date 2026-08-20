using UnityEngine;

public class TemporaryUIScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static TemporaryUIScript Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
