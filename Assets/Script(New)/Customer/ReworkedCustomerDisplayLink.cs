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

    // ================================ AWAKE START UPDATE ================================
    void Start()
    {
        // new shit
        ReworkedCustomerManager.instance.RegisterDisplayReferences(_customerSpriteRenderer, _dialogueText);

        ReworkedCustomerManager.instance.EvaluateAndUpdateCounterState();
        ReworkedCustomerManager.instance.EvaluateAndUpdateGameplayState();
    }
}