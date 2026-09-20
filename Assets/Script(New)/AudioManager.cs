using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource BGM;
    [SerializeField] AudioSource SFX;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider BGMSlider;
    void Start() //something here is not letting me set volume to 0.5 at playmode???
    {

        if (!PlayerPrefs.HasKey("Sound Volume")) //why switching the if else statments doesnt make it work*
        {
            Load();
        }

        else
        {
            PlayerPrefs.SetFloat("Sound Volume", 0.5f);
            Load();
        } //needs to change
    }
    public void ChangeVolume()
    {
        BGM.volume = BGMSlider.value;
        SFX.volume = SFXSlider.value;
        Save();
    }
    private void Load()
    {
        BGMSlider.value = PlayerPrefs.GetFloat("Sound Volume");
    } //add one for each slider
    private void Save()
    {
        PlayerPrefs.SetFloat("Sound Volume", SFXSlider.value);
    } // add one for each slider
    void Update()
    {

    }

   

}
