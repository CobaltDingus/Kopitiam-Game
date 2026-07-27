using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadKitchen()
    {
        SceneManager.LoadScene("KitchenScene");
    }

    public void LoadCounter()
    {
        SceneManager.LoadScene("CounterScene");
    }
}