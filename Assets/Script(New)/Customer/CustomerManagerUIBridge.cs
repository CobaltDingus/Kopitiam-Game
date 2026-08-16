using UnityEngine;

public class CustomerManagerUIBridge : MonoBehaviour
{
    private void Start()
    {
        // method of using singleton scripts
        //CustomerManager.Instance.GenerateNewCustomer();
    }
    public void OnGenerateNewCustomerClicked()
    {
        if (CustomerManager.Instance != null)
            CustomerManager.Instance.GenerateNewCustomer();
        else
            Debug.LogError("CustomerManager instance not found!");
    }

    public void OnServePerfectDrinksClicked()
    {
        if (CustomerManager.Instance != null)
            CustomerManager.Instance.ServePerfectDrinksForTesting();
        else
            Debug.LogError("CustomerManager instance not found!");
    }

    public void OnServeOrderClicked()
    {
        if (CustomerManager.Instance != null)
            CustomerManager.Instance.ServeOrder();
        else
            Debug.LogError("CustomerManager instance not found!");
    }
}
