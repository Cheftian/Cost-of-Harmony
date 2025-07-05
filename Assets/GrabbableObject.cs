using UnityEngine;

public class GrabbableObject : ObjectBase
{
    [Header("Grabbable Properties")]
    [Tooltip("Pengali kecepatan pemain saat membawa objek ini (Contoh: 0.7 = 70% kecepatan normal)")]
    public float speedModifier = 0.7f;

    [Tooltip("Pengali tinggi lompatan pemain saat membawa objek ini (Contoh: 0.8 = 80% tinggi normal)")]
    public float jumpModifier = 0.8f;

    [Tooltip("Posisi objek relatif terhadap pemain saat dipegang")]
    public Vector2 heldPositionOffset = new Vector2(0.6f, 0.2f);
    
    /// <summary>
    /// Override metode StartInteraction dari ObjectBase.
    /// </summary>
    public override void StartInteraction(Transform playerTransform)
    {
        // Memanggil logika dasar dari metode aslinya (misalnya, logging dan set flag)
        base.StartInteraction(playerTransform);

        // Menjadikan objek anak dari pemain agar ikut bergerak dan flip.
        transform.SetParent(playerTransform);

        // Mengatur posisi lokal objek sesuai offset yang ditentukan.
        // Ini membuat objek terlihat 'dipegang' di posisi yang benar.
        transform.localPosition = heldPositionOffset;

        // Menonaktifkan fisika agar tidak bertabrakan dengan lingkungan saat dipegang.
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
    }

    /// <summary>
    /// Override metode EndInteraction dari ObjectBase.
    /// </summary>
    public override void EndInteraction()
    {
        base.EndInteraction();

        // Melepaskan objek dari pemain.
        transform.SetParent(null);
        
        // Mengaktifkan kembali fisika.
        rb.bodyType = RigidbodyType2D.Dynamic;
        col.enabled = true;
    }

    /// <summary>
    /// Metode yang akan dipanggil PlayerController untuk menyesuaikan kecepatannya.
    /// </summary>
    public float GetSpeedModifier()
    {
        // Untuk objek kecil dan sedang, kita gunakan modifier yang sama dari inspector.
        // Nanti bisa dibuat lebih spesifik jika perlu.
        if (objectType == ObjectType.Small || objectType == ObjectType.Medium)
        {
            return speedModifier;
        }
        return 1f; // Default, tidak ada perubahan kecepatan.
    }

    /// <summary>
    /// Metode yang akan dipanggil PlayerController untuk menyesuaikan lompatannya.
    /// </summary>
    public float GetJumpModifier()
    {
        if (objectType == ObjectType.Small || objectType == ObjectType.Medium)
        {
            return jumpModifier;
        }
        return 1f; // Default, tidak ada perubahan lompatan.
    }
}