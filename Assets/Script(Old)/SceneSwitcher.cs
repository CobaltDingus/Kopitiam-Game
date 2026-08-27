using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // void Start()
    // {
    //     SceneManager.LoadScene("KitchenScene", LoadSceneMode.Additive);
    // }
    public void LoadKitchen()
    {
        SceneManager.LoadScene("KitchenScene");
    }

    public void LoadCounter()
    {
        SceneManager.LoadScene("CounterScene");
    }
}