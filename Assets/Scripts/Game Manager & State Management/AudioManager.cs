using UnityEngine;
using System; // Diperlukan untuk menggunakan Array.Find

// Struct ini adalah "paket" untuk setiap suara kita.
// [System.Serializable] membuatnya bisa terlihat di Inspector Unity.
[System.Serializable]
public struct Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    // --- Singleton Pattern (Tetap sama) ---
    public static AudioManager Instance { get; private set; }

    // --- Audio Sources (Tetap sama) ---
    public AudioSource musicSource;
    public AudioSource sfxSource;

    // --- PERPUSTAKAAN AUDIO (PERUBAHAN UTAMA) ---
    public Sound[] musicLibrary;
    public Sound[] sfxLibrary;

    // --- Master Volume (Tetap sama) ---
    public float MasterVolume { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(savedVolume);
    }

    // --- Fungsi Pengaturan Volume (Tetap sama) ---
    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = MasterVolume;
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.Save();
    }
    public void DecreaseHealthVolume(float amount) { SetMasterVolume(MasterVolume - amount); }
    public void IncreaseHealthVolume(float amount) { SetMasterVolume(MasterVolume + amount); }

    // --- FUNGSI PEMUTAR SUARA YANG DIPERBARUI ---

    public void PlayMusic(string name)
    {
        // Cari musik di perpustakaan berdasarkan nama
        Sound s = Array.Find(musicLibrary, sound => sound.name == name);

        if (s.clip == null)
        {
            // Beri peringatan jika musik dengan nama tersebut tidak ditemukan
            Debug.LogWarning("AudioManager: Musik dengan nama '" + name + "' tidak ditemukan!");
            return;
        }
        
        // Hentikan musik yang sedang berjalan dan mainkan yang baru
        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        // Cari SFX di perpustakaan berdasarkan nama
        Sound s = Array.Find(sfxLibrary, sound => sound.name == name);

        if (s.clip == null)
        {
            // Beri peringatan jika SFX dengan nama tersebut tidak ditemukan
            Debug.LogWarning("AudioManager: SFX dengan nama '" + name + "' tidak ditemukan!");
            return;
        }

        // Mainkan SFX
        sfxSource.PlayOneShot(s.clip);
    }
}