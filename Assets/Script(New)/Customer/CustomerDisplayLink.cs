using TMPro;
using UnityEngine;

public class CustomerDisplayLink : MonoBehaviour
{
    [SerializeField] private SpriteRenderer customerSpriteRenderer;
    [SerializeField] private TMP_Text dialogueText;

    void Start()
    {
        if (CustomerManager.Instance == null)
        {
            Debug.LogWarning("CustomerDisplayLink: No CustomerManager instance found in the scene yet.");
            return;
        }

        CustomerManager.Instance.RegisterDisplayReferences(customerSpriteRenderer, dialogueText);

        // Standard Tutorial logic
        if (SaveManager.saveManager != null && SaveManager.saveManager.DayCount == 0 && !CustomerManager.Instance.tutorialComplete)
        {
            if (CustomerManager.Instance.tutorialPhase == 5 || CustomerManager.Instance.tutorialPhase == 7)
            {
                if (CustomerManager.Instance.OkayButton != null)
                    CustomerManager.Instance.OkayButton.gameObject.SetActive(CustomerManager.Instance.tutorialserve);
            }
        }
        else
        {
            // --- POST-TUTORIAL GAMEPLAY ---
            // Force Okay and TryAgain buttons off everywhere after tutorial
            if (CustomerManager.Instance.OkayButton != null)
                CustomerManager.Instance.OkayButton.gameObject.SetActive(false);

            if (CustomerManager.Instance.TryAgainButton != null)
                CustomerManager.Instance.TryAgainButton.gameObject.SetActive(false);

            // Handle Next button visibility based on serving status
            if (CustomerManager.Instance.NextButton != null)
            {
                if (!CustomerManager.Instance.serveStatus)
                {
                    CustomerManager.Instance.NextButton.gameObject.SetActive(false);
                }
                else
                {
                    CustomerManager.Instance.NextButton.gameObject.SetActive(true);
                }
            }
        }
        if (!CustomerManager.Instance.IsTryAgain)
        {
            CustomerManager.Instance.TryAgainButton.gameObject.SetActive(false);
        }
        if (!CustomerManager.Instance.IsOkay)
        {
            CustomerManager.Instance.OkayButton.gameObject.SetActive(false);
        }
        if (!CustomerManager.Instance.IsNext)
        {
            CustomerManager.Instance.NextButton.gameObject.SetActive(false);
        }
        if (!CustomerManager.Instance.IsCustomerCount)
        {
            UiManager.uiManager.HideCustomerCount();
        }
        if (!CustomerManager.Instance.IsFavour)
        {
            UiManager.uiManager.HideFavour();
        }
    }
}