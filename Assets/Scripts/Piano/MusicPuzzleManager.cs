using UnityEngine;
using System.Collections.Generic; // Dibutuhkan untuk menggunakan List

// Class untuk mendefinisikan setiap puzzle di Inspector
[System.Serializable]
public class MusicPuzzle
{
    public string puzzleName; // Hanya untuk penanda di inspector
    public string[] correctMelody;
    public string doorID; // ID pintu yang akan dibuka
}

public class MusicPuzzleManager : MonoBehaviour
{
    // Daftar semua puzzle yang harus diselesaikan
    public MusicPuzzle[] puzzles;
    
    // Jumlah penalti pengurangan nyawa jika salah
    public float healthPenalty = 0.1f; // Mengurangi 10% nyawa

    private int currentPuzzleIndex = 0;
    private List<string> playerInputSequence = new List<string>();

    // Fungsi ini dipanggil oleh PianoUIManager setiap kali not dimainkan
    public void NotePlayed(string noteName)
    {
        if (currentPuzzleIndex >= puzzles.Length)
        {
            Debug.Log("Semua puzzle sudah selesai!");
            return;
        }

        playerInputSequence.Add(noteName);
        int currentInputIndex = playerInputSequence.Count - 1;

        // Cek apakah not yang dimainkan benar
        string[] correctSequence = puzzles[currentPuzzleIndex].correctMelody;

        // Jika not yang ditekan salah
        if (correctSequence[currentInputIndex] != noteName)
        {
            PuzzleFailed();
            return;
        }

        // Jika not yang ditekan benar dan merupakan not terakhir dari melodi
        if (playerInputSequence.Count == correctSequence.Length)
        {
            PuzzleSolved();
        }
    }

    private void PuzzleFailed()
    {
        Debug.Log("Urutan salah! Coba lagi.");
        // Kurangi nyawa melalui AudioManager
        AudioManager.Instance.DecreaseHealthVolume(healthPenalty);
        // Reset urutan input pemain
        playerInputSequence.Clear();
    }

    private void PuzzleSolved()
    {
        Debug.Log("Puzzle " + puzzles[currentPuzzleIndex].puzzleName + " Selesai!");
        
        // --- LOGIKA MEMBUKA PINTU ---
        // Kita akan implementasikan ini nanti. Untuk sekarang, kita beri log.
        Debug.Log("Membuka pintu dengan ID: " + puzzles[currentPuzzleIndex].doorID);
        // DoorManager.Instance.OpenDoor(puzzles[currentPuzzleIndex].doorID);
        
        // Lanjut ke puzzle berikutnya
        currentPuzzleIndex++;
        // Reset urutan input pemain untuk puzzle selanjutnya
        playerInputSequence.Clear();

        if (currentPuzzleIndex >= puzzles.Length)
        {
            Debug.Log("Selamat! Semua puzzle musik berhasil diselesaikan!");
        }
    }
}