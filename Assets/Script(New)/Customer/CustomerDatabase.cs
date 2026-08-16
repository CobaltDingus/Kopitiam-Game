using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerDatabase", menuName = "Customers/CustomerDatabase")]
public class CustomerDatabase : ScriptableObject
{
    [SerializeField] private List<CustomerData> allCustomers = new();
    public List<CustomerData> AllCustomers => allCustomers;
}