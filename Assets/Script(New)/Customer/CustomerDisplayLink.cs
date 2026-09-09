using TMPro;
using UnityEngine;
using DG.Tweening;

public class CustomerDisplayLink : MonoBehaviour
{
    [SerializeField] private SpriteRenderer customerSpriteRenderer;
    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float startYOffset = -800f; // how far below screen to start

    [SerializeField] private Transform customerTransform;
    private Vector3 onscreenWorldPos;

    void OnEnable()
    {
        CustomerManager.OnNewCustomer += SlideIn;
    }

    void OnDisable()
    {
        CustomerManager.OnNewCustomer -= SlideIn;
    }

    void Awake()
    {
        onscreenWorldPos = customerTransform.position;
        SlideIn();
    }

    public void SlideIn()
    {
        Vector3 startPos = onscreenWorldPos + Vector3.down * 5f; // world units, tune this
        customerTransform.position = startPos;

        customerTransform.DOMove(onscreenWorldPos, slideDuration)
            .SetEase(Ease.OutBack);
    }

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