using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenOverlay : 
MonoBehaviour, 
IPointerClickHandler
// IPointerDownHandler
{
    [SerializeField] private List<UIPanel> uiToClose;
    [SerializeField] private GameObject panel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (UIPanel uiPanel in uiToClose)
        {
            if (uiPanel.gameObject.activeSelf)
            {
                uiPanel.ClosePanel();
            }
        }
        Debug.Log("Overlay clicked");
    }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     foreach (UIPanel panel in uiToClose)
    //     {
    //         panel.ClosePanel();
    //     }
    //     Debug.Log("Overlay clicked");
    // }
}
