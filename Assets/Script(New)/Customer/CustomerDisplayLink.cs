using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

        if (!CustomerManager.Instance.IsNext)
        {
            CustomerManager.Instance.NextButton.gameObject.SetActive(false);
        }

        if (!CustomerManager.Instance.IsOkay)
        {
            CustomerManager.Instance.OkayButton.gameObject.SetActive(false);
        }
        if (!CustomerManager.Instance.IsTryAgain)
        {
            CustomerManager.Instance.TryAgainButton.gameObject.SetActive(false);
        }
        if (CustomerManager.Instance.tutorialserve)
        {
            CustomerManager.Instance.OkayButton.gameObject.SetActive(true);
        }
        if (CustomerManager.Instance.tutorialComplete)
        {
            CustomerManager.Instance.NextButton.gameObject.SetActive(true);
        }
    }
}
