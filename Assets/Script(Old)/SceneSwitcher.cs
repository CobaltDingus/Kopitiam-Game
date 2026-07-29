using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadKitchen()
    {
        SceneManager.LoadScene("KitchenRearranged");
    }

    public void LoadCounter()
    {
        SceneManager.LoadScene("CounterScene");
    }
}