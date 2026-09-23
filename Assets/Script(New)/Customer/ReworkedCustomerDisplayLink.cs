using DG.Tweening;
using TMPro;
using UnityEngine;

public class ReworkedCustomerDisplayLink : MonoBehaviour
{
    // ================================ REVAMP CODES ================================
    // [RULES]
    // [NOTE] Do Not Change Any Codes Here Unless Needed
    // - core variables
    // - functions for calculation and modifying said variables
    // - function starts with Capital
    // - variable starts with small
    // ================================ START ================================

    [Header("UI Display Referencing (Revamp)")]
    [SerializeField] private SpriteRenderer _customerSpriteRenderer;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float startYOffset = -800f; // how far below screen to start
    [SerializeField] private Transform customerTransform;
    private Vector3 onscreenWorldPos;

    // ================================ AWAKE START UPDATE ================================
    void Awake()
    {
        onscreenWorldPos = customerTransform.position;
    }
    void Start()
    {
        // fallback
        if (ReworkedCustomerManager.instance == null)
        {
            Debug.LogWarning("ReworkedCustomerDisplayLink: No ReworkedCustomerManager instance found in the scene yet.");
            return;
        }
        
        // new shit
        ReworkedCustomerManager.instance.RegisterDisplayReferences(_customerSpriteRenderer, _dialogueText);

        ReworkedCustomerManager.instance.EvaluateAndUpdateCounterState();
        ReworkedCustomerManager.instance.EvaluateAndUpdateGameplayState();
        ReworkedCustomerManager.instance.LoadIngredientDisplay();
    }
    // ================================ ANIMATION TWEEN ==================================
    void OnEnable()
    {
        CustomerManager.OnNewCustomer += SlideIn;
    }

    void OnDisable()
    {
        CustomerManager.OnNewCustomer -= SlideIn;
    }
    public void SlideIn()
    {
        Vector3 startPos = onscreenWorldPos + Vector3.down * 5f; // world units, tune this
        customerTransform.position = startPos;

        customerTransform.DOMove(onscreenWorldPos, slideDuration)
            .SetEase(Ease.OutBack);
    }

}