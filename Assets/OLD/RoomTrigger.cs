using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTrigger : MonoBehaviour
{
    // Bagian untuk Fungsionalitas RoomTrigger
    public string targetSceneName;    // Nama scene tujuan saat keluar dari trigger ini
    public string targetSpawnID_In_TargetScene; // ID spawner di scene tujuan yang akan dituju player

    // Bagian untuk Fungsionalitas RoomSpawner (spawnerID dari objek ini sendiri)
    public string thisSpawnerID; // ID unik untuk spawner ini di scene ini (misal: "EntranceA_Room1")

    // --- DEBUG FLAGS BARU ---
    [Header("Debug Flags")]
    public bool debug_PlayerEnteredTrigger = false; // TRUE jika player sedang di dalam collider ini
    public bool debug_PlayerClickedTrigger = false; // TRUE jika player sudah mengklik trigger ini dan siap transisi
    // --- AKHIR DEBUG FLAGS BARU ---

    // Flag internal untuk kontrol, sesuai dengan debug_PlayerClickedTrigger
    private bool hasBeenClicked = false; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            debug_PlayerEnteredTrigger = true; // Set debug flag
            Debug.Log($"[{gameObject.name}] Player MEMASUKI trigger. PlayerEnteredTrigger = TRUE.");

            // Cek kondisi transisi: sudah diklik DAN player sudah masuk (atau masih di dalam)
            if (hasBeenClicked)
            {
                TriggerSceneTransition();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            debug_PlayerEnteredTrigger = false; // Set debug flag
            Debug.Log($"[{gameObject.name}] Player KELUAR dari trigger. PlayerEnteredTrigger = FALSE.");
            // Reset flag klik jika player keluar dan belum sempat transisi
            // Ini agar player harus mengklik lagi jika dia bolak-balik
            hasBeenClicked = false;
            debug_PlayerClickedTrigger = false;
        }
    }

    // Fungsi ini dipanggil oleh PlayerInteraction saat RoomTrigger diklik
    public void SetTriggerClicked()
    {
        hasBeenClicked = true;
        debug_PlayerClickedTrigger = true; // Set debug flag
        Debug.Log($"[{gameObject.name}] Menerima sinyal 'Trigger Diklik'. PlayerClickedTrigger = TRUE.");

        // Jika player sudah berada di dalam trigger saat diklik
        if (debug_PlayerEnteredTrigger)
        {
            Debug.Log($"[{gameObject.name}] Player sudah di dalam trigger saat diklik. Memicu transisi.");
            TriggerSceneTransition();
        }
    }

    private void TriggerSceneTransition()
    {
        // Pastikan transisi hanya terjadi sekali
        if (hasBeenClicked && debug_PlayerEnteredTrigger) // Pastikan kondisi transisi terpenuhi
        {
            Debug.Log($"[{gameObject.name}] Kondisi transisi terpenuhi. Memuat scene: {targetSceneName}");
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene(targetSceneName, targetSpawnID_In_TargetScene);
            }
            else
            {
                Debug.LogError("SceneTransitionManager not found! Make sure it's in the scene and set up as a singleton.");
                SceneManager.LoadScene(targetSceneName); // Fallback
            }
            // Reset flags setelah transisi dipicu
            hasBeenClicked = false;
            debug_PlayerClickedTrigger = false;
            // Penting: Jangan langsung set debug_PlayerEnteredTrigger menjadi false di sini,
            // biarkan OnTriggerExit2D yang menanganinya nanti.
        }
    }


    // Fungsi ini akan dipanggil oleh SceneTransitionManager saat scene dimuat
    public Vector2 GetSpawnPosition()
    {
        return transform.position;
    }

    // Untuk visualisasi di editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f); // Warna hijau transparan
        Gizmos.DrawCube(transform.position, transform.localScale);

        Gizmos.color = Color.blue; // Warna biru
        Gizmos.DrawSphere(transform.position, 0.3f);
        Gizmos.DrawRay(transform.position, Vector3.up * 0.8f);
    }
}