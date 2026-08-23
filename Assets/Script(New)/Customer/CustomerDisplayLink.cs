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

        // Standard button initializations
        if (CustomerManager.Instance.NextButton != null)
            CustomerManager.Instance.NextButton.gameObject.SetActive(CustomerManager.Instance.IsNext);

        if (CustomerManager.Instance.TryAgainButton != null)
            CustomerManager.Instance.TryAgainButton.gameObject.SetActive(CustomerManager.Instance.IsTryAgain);

        // --- Tutorial Scene Load Handling ---
        if (SaveManager.saveManager != null && SaveManager.saveManager.DayCount == 0)
        {
            // Support BOTH Phase 5 and Phase 7
            if (CustomerManager.Instance.tutorialPhase == 5 || CustomerManager.Instance.tutorialPhase == 7)
            {
                if (!CustomerManager.Instance.tutorialserve)
                {
                    // Returned from kitchen scene: HIDE Okay button until drinks are served
                    if (CustomerManager.Instance.OkayButton != null)
                        CustomerManager.Instance.OkayButton.gameObject.SetActive(false);
                }
                else
                {
                    // Order has been served correctly: SHOW Okay button
                    if (CustomerManager.Instance.OkayButton != null)
                        CustomerManager.Instance.OkayButton.gameObject.SetActive(true);
                }
            }
        }
        else if (CustomerManager.Instance.tutorialComplete)
        {
            if (CustomerManager.Instance.NextButton != null)
                CustomerManager.Instance.NextButton.gameObject.SetActive(true);
        }
    }
}