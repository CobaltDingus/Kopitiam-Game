using UnityEngine;
using TMPro;
public class CustomerDisplayLink : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private SpriteRenderer customerSpriteRenderer;
    [SerializeField] private TMP_Text dialogueText;
    //[SerializeField] private TM_Text score;

    void Start()
    {
        if (CustomerManager.Instance == null)
        {
            Debug.LogWarning("CustomerDisplayLink: No CustomerManager instance found in the scene yet.");
            return;
        }

        CustomerManager.Instance.RegisterDisplayReferences(customerSpriteRenderer, dialogueText);
    }
}
