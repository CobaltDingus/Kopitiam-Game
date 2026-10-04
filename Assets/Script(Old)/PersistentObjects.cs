using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentObjects : MonoBehaviour
{
    [SerializeField] private MixingCup mixingCup;
    [SerializeField] private PouringSlot pouringSlot;

    private static PersistentObjects instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateObjects(scene.name);
    }

    private void UpdateObjects(string sceneName)
    {
        Camera newCamera = Camera.main;

        mixingCup.SetCamera(newCamera);
        pouringSlot.SetCamera(newCamera);

        mixingCup.gameObject.SetActive(sceneName == "KitchenScene");
        pouringSlot.gameObject.SetActive(sceneName == "KitchenScene");
    }
}