using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerDatabase", menuName = "Customers/CustomerDatabase")]
public class CustomerDatabase : ScriptableObject
{
    // ltr change the name to allNormalCustomer
    [SerializeField] private List<CustomerData> allCustomers = new();
    // set to allTouristCustomer
    [SerializeField] private List<CustomerData> _allTouristCustomer = new();
    // set to allSleepDeprivedCustomer
    [SerializeField] private List<CustomerData> _allSleepDeprivedCustomer = new();
    public List<CustomerData> AllCustomers => allCustomers;
    public List<CustomerData> AllTouristCustomer => _allTouristCustomer;
    public List<CustomerData> AllSleepDeprivedCustomer => _allSleepDeprivedCustomer;

}