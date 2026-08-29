using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CounterNoteUI : MonoBehaviour
{
    public bool isRead;

    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI text;   
    [SerializeField] UIPanel note;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRead)
        {
            if (SaveManager.saveManager.DayCount >= 2)
            {
                // button.enabled = true;
                button.interactable = true;
                // button.image.gameObject.SetActive(true);
                // text.gameObject.SetActive(true);
                text.text = "Note from uncle";
                text.color = Color.white;
                button.image.color = Color.red;
            }
            else
            {
                // button.enabled = false;
                button.interactable = false;
                // button.image.gameObject.SetActive(false);
                // text.gameObject.SetActive(false);
                text.text = "No notes";

            }
        }
        else
        {
            button.gameObject.SetActive(false);
        }

    }
}
