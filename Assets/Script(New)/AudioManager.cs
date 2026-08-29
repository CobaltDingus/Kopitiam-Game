using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Slider AudioSlider;
    void Start() //something here is not letting me set volume to 0.5 at playmode???
    {

       if (!PlayerPrefs.HasKey("Music Volume")) //why switching the if else statments doesnt make it work*
        {
            Load();
        }

       else
        {
            PlayerPrefs.SetFloat("Music Volume", 0.5f);
            Load();
        }
    }
    public void ChangeVolume()
    {
        AudioListener.volume = AudioSlider.value;
    }
    private void Load()
    {
        AudioSlider.value = PlayerPrefs.GetFloat("Music Volume");
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("Music Volume", AudioSlider.value);
    }
    void Update()
    {

    }
}
