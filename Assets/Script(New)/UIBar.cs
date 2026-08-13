using UnityEngine;
using TMPro;
public class UIBar : MonoBehaviour
{
    public static UIBar Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TMP_Text customerCounter;
    [SerializeField] private TMP_Text dayCounter;

    private void Awake()
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
