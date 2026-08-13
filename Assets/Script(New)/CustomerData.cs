using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomer", menuName = "Customers/CustomerData")]
public class CustomerData : ScriptableObject
{
    [Header("Customer Info")]
    [SerializeField] private string id;
    [SerializeField] private string customerName;
    [SerializeField] private List<Sprite> customerSprite;
    //[SerializeField] private Sprite customerSprite;
    [SerializeField] private string variant;

    [Header("Dialogue Lists")]
    [SerializeField] private List<string> startFrontDialogue = new();
    [SerializeField] private List<string> startBackDialogue = new();

    [SerializeField] private List<string> perfectFrontDialogue = new();
    [SerializeField] private List<string> perfectBackDialgue = new();

    [SerializeField] private List<string> decentFrontDialogue = new();
    [SerializeField] private List<string> decentBackDialogue = new();

    [SerializeField] private List<string> wrongFrontDialogue = new();
    [SerializeField] private List<string> wrongBackDiaogue = new();

    // getter for Customer Info
    public string Id => id;
    public string CustomerName => customerName;
    public List<Sprite> CustomerSprite => customerSprite;

    public string Variant => variant;

    // Start dialogue
    public List<string> StartFrontDialogue => startFrontDialogue;
    public List<string> StartBackDialogue => startBackDialogue;

    // Perfect dialogue (all drinks correct)
    public List<string> PerfectFrontDialogue => perfectFrontDialogue;
    public List<string> PerfectBackDialogue => perfectBackDialgue;

    // Decent dialogue (some drinks wrong)
    public List<string> DecentFrontDialogue => decentFrontDialogue;
    public List<string> DecentBackDialogue => decentBackDialogue;

    // Wrong dialogue (all drinks wrong)
    public List<string> WrongFrontDialogue => wrongFrontDialogue;
    public List<string> WrongBackDialogue => wrongBackDiaogue;
}