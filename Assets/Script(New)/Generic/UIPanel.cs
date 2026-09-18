using UnityEngine;

public class UIPanel : MonoBehaviour
{
    [SerializeField] private GameObject screenOverlay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // ClosePanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPanel()
    {
        if (screenOverlay != null)
        {
            screenOverlay.SetActive(true);
        }
        gameObject.SetActive(true);        
    }

    public void ClosePanel()
    {
        if (screenOverlay != null)
        {
            screenOverlay.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void TogglePanel()
    {
        if (screenOverlay != null)
        {
            screenOverlay.SetActive(!screenOverlay.activeSelf);
        }
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void TurnOn()
    {
        screenOverlay.gameObject.SetActive(true);
    }
    public void TurnOff()
    {
        screenOverlay.gameObject.SetActive(false);
    }
}
