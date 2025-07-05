using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Tambahkan ini untuk bisa menggunakan Slider

public class InGameUIManager : MonoBehaviour
{
    // Hubungkan Panel/Canvas Pause Menu di Inspector
    public GameObject pauseMenuUI; 
    
    // --- TAMBAHAN BARU ---
    // Hubungkan Slider dari panel pause ke sini
    public Slider healthDisplaySlider;

    void Start()
    {
        // Pastikan menu pause nonaktif saat scene dimulai
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        // Pastikan waktu berjalan normal saat scene dimulai
        Time.timeScale = 1f;
        AudioManager.Instance.PlayMusic("Proetta");
    }

    // --- TAMBAHAN BARU ---
    void Update()
    {
        // Fungsi Update akan berjalan setiap frame
        // Kita akan selalu menyamakan nilai slider dengan MasterVolume global
        if (healthDisplaySlider != null && AudioManager.Instance != null)
        {
            healthDisplaySlider.value = AudioManager.Instance.MasterVolume;
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f; // Hentikan waktu
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f; // Jalankan waktu kembali
        }
    }

    public void GoToMainMenu()
    {
        // Penting: Selalu set Time.timeScale kembali ke 1 sebelum pindah scene
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
}