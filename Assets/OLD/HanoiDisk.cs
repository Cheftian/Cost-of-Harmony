using UnityEngine;

public class HanoiDisk : InteractableObject
{
    public int diskSize; 
    
    public float kinematicVelocityThreshold = 0.05f; 
    public float settlingTime = 0.2f; 

    private float currentSettlingTimer; 
    
    // BARU: Referensi ke PlayerInteraction untuk cek apakah sedang menunggu penempatan di Peg
    private PlayerInteraction playerInteraction; 

    protected override void Awake() 
    {
        base.Awake(); 
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; 
            rb.gravityScale = 1; 
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
        }
        currentSettlingTimer = settlingTime; 
    }

    void Start()
    {
        // BARU: Dapatkan referensi ke PlayerInteraction di Start
        playerInteraction = FindObjectOfType<PlayerInteraction>();
        if (playerInteraction == null)
        {
            Debug.LogWarning("PlayerInteraction script not found in scene. Disk stabilization might not behave as expected for Hanoi game.");
        }
    }

    void FixedUpdate()
    {
        // HANYA ubah ke Kinematic jika TIDAK sedang di-drag DAN (sudah secara LOGIS diletakkan di StackPoint ATAU tidak ada StackPoint yang diharapkan)
        // Dan disknya memang sedang dynamic
        if (!isBeingDragged && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            // Tambahkan kondisi untuk hanya menjadi Kinematic jika disk sudah memiliki currentStackPoint
            // ATAU jika Elias tidak sedang mencoba menempatkannya di suatu peg (awaitingInteractionPoint false)
            // Ini mencegah disk menjadi Kinematic jika drop di peg salah
            bool shouldAttemptKinematic = (currentStackPoint != null); // Jika sudah logis di peg, bisa jadi kinematic
            
            // Aturan tambahan: Jika player tidak sedang menunggu interaksi dan disk tidak di stack point, biarkan dia jadi kinematic juga
            // Ini untuk kasus disk yang jatuh bebas ke tanah/lantai
            if (!playerInteraction.IsAwaitingInteraction() && currentStackPoint == null)
            {
                shouldAttemptKinematic = true;
            }

            if (shouldAttemptKinematic)
            {
                if (Mathf.Abs(rb.linearVelocity.y) < kinematicVelocityThreshold) 
                {
                    currentSettlingTimer -= Time.fixedDeltaTime; 
                    if (currentSettlingTimer <= 0)
                    {
                        rb.bodyType = RigidbodyType2D.Kinematic; 
                        rb.gravityScale = 0; 
                        rb.linearVelocity = Vector2.zero; 
                        Debug.Log($"Disk {gameObject.name} stabil setelah {settlingTime} detik. Menjadi Kinematic.");
                    }
                }
                else 
                {
                    currentSettlingTimer = settlingTime; 
                }
            }
            else
            {
                // Jika tidak seharusnya menjadi Kinematic (misal: di-drop di peg yang salah),
                // pastikan disk tetap Dynamic dan gravitasi aktif.
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 1;
                // Reset timer agar tidak langsung jadi kinematic jika kondisi berubah
                currentSettlingTimer = settlingTime;
            }
        }
    }

    public override void StartDrag()
    {
        base.StartDrag(); 

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; 
            rb.gravityScale = 0; 
            rb.linearVelocity = Vector2.zero; 
        }
        if (objectCollider != null)
        {
            objectCollider.enabled = false; 
        }
        currentStackPoint = null; 
        Debug.Log($"Mulai drag Disk: {gameObject.name} (Size: {diskSize})");
    }

    public override void Drop()
    {
        base.Drop(); 

        if (rb == null) return;

        objectCollider.enabled = true; 

        // Setelah drop, disk akan Dynamic dengan gravitasi.
        // FixedUpdate akan membuatnya Kinematic jika sudah stabil DAN secara logis diizinkan.
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.gravityScale = 1;
        rb.linearVelocity = Vector2.zero; 
        currentSettlingTimer = settlingTime; 

        Debug.Log($"Disk {gameObject.name} di-drop. Menjadi Dynamic, akan Kinematic jika stabil dan diletakkan di tempat valid.");

        // Catatan: Mekanisme PlaceDisk di HanoiPeg akan secara eksplisit membuatnya Kinematic
        // saat berhasil diletakkan di atas Peg.
    }
}