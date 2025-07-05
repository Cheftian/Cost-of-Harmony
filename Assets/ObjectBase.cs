using UnityEngine;

// Enum untuk mendefinisikan semua tipe objek yang ada di dalam game.
// Ini akan muncul sebagai dropdown di Unity Inspector.
public enum ObjectType
{
    Small,
    Medium,
    Large,
    Heavy,
    Vulnerable,
    Fragile
}

// Semua objek interaktif harus memiliki Rigidbody2D dan Collider2D.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ObjectBase : MonoBehaviour
{
    [Header("Object Properties")]
    public ObjectType objectType; // Tipe objek yang bisa diatur dari Inspector.

    // Komponen yang akan sering digunakan. 'protected' agar bisa diakses oleh kelas turunan.
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected bool isBeingInteractedWith = false; // Status untuk melacak apakah objek sedang dipegang/didorong.

    /// <summary>
    /// Awake dipanggil saat skrip pertama kali dimuat.
    /// Digunakan untuk inisialisasi komponen.
    /// </summary>
    protected virtual void Awake()
    {
        // Mengambil referensi komponen dari GameObject ini.
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Pesan error jika komponen penting tidak ditemukan.
        if (rb == null)
        {
            Debug.LogError($"[ObjectBase] Rigidbody2D tidak ditemukan pada {gameObject.name}!");
        }
        if (col == null)
        {
            Debug.LogError($"[ObjectBase] Collider2D tidak ditemukan pada {gameObject.name}!");
        }
    }

    // --- METODE INTERAKSI VIRTUAL ---
    // Metode ini sengaja dibuat 'virtual' agar bisa di-override (diubah perilakunya)
    // oleh skrip objek yang lebih spesifik (misalnya GrabbableObject, VulnerableObject, dll.)

    /// <summary>
    /// Dipanggil saat pemain memulai interaksi (misalnya menekan 'Enter').
    /// </summary>
    public virtual void StartInteraction(Transform playerTransform)
    {
        isBeingInteractedWith = true;
        Debug.Log($"Memulai interaksi dengan {gameObject.name}");
    }

    /// <summary>
    /// Dipanggil saat pemain mengakhiri interaksi (misalnya melepas 'Enter').
    /// </summary>
    public virtual void EndInteraction()
    {
        isBeingInteractedWith = false;
        Debug.Log($"Mengakhiri interaksi dengan {gameObject.name}");
    }
    
    /// <summary>
    /// Dipanggil saat objek hancur (khusus untuk objek Fragile atau Vulnerable).
    /// </summary>
    public virtual void DestroyObject()
    {
        Debug.Log($"{gameObject.name} hancur.");
        Destroy(gameObject); // Menghancurkan GameObject dari scene.
    }

    /// <summary>
    /// Dipanggil saat pemain menginjak objek ini.
    /// </summary>
    public virtual void OnSteppedOn()
    {
        // Logika untuk objek Rentan atau Rapuh akan ditempatkan di sini pada kelas turunannya.
    }
    
    /// <summary>
    /// Dipanggil saat pemain turun/meninggalkan objek ini setelah menginjaknya.
    /// </summary>
    public virtual void OnLeft()
    {
        // Logika untuk objek Rentan akan ditempatkan di sini pada kelas turunannya.
    }
}