using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        // Saat scene Settings terbuka, atur posisi slider sesuai volume yang tersimpan di AudioManager
        if (AudioManager.Instance != null)
        {
            volumeSlider.value = AudioManager.Instance.MasterVolume;
        }

        // Tambahkan listener agar setiap slider digerakkan, ia akan memanggil fungsi di AudioManager
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float value)
    {
        // Panggil fungsi global di AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
        }
    }
}