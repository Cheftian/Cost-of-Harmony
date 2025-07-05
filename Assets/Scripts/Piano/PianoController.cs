using UnityEngine;

public class PianoController : MonoBehaviour
{
    // Hubungkan Canvas UI Piano dari Inspector
    public GameObject pianoUIPanel;
    
    private bool isPlayerNear = false;

    void Start()
    {
        // Pastikan UI piano tidak aktif di awal
        if (pianoUIPanel != null)
        {
            pianoUIPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Cek jika pemain ada di dekat piano dan menekan "Enter" (Submit)
        if (isPlayerNear && Input.GetButtonDown("Submit"))
        {
            OpenPianoUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek jika yang masuk adalah objek dengan tag "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

    private void OpenPianoUI()
    {
        if (pianoUIPanel != null)
        {
            pianoUIPanel.SetActive(true);
            // Jalankan musik ambience piano
            AudioManager.Instance.PlayMusic("Ambience");
            
            // Opsional: nonaktifkan script gerakan pemain agar tidak bisa bergerak saat bermain piano
            // GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = false;
        }
    }
}