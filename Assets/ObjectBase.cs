using UnityEngine;

public enum ObjectType
{
    Small,
    Medium,
    Heavy,
    Vulnerable,
    Fragile
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ObjectBase : MonoBehaviour
{
    [Header("Object Properties")]
    public ObjectType objectType;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint; 
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    protected Rigidbody2D rb;
    protected Collider2D col;
    protected bool isBeingInteractedWith = false;
    private bool isGrounded = false;
    
    // Variabel baru untuk melacak status interaksi dari frame sebelumnya
    private bool wasBeingInteractedWith = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (groundCheckPoint == null) Debug.LogError($"[ObjectBase] 'Ground Check Point' belum di-set pada {gameObject.name}!");
    }

    private void FixedUpdate()
    {
        if (isBeingInteractedWith)
        {
            // Update status 'was' dan keluar jika masih diinteraksikan
            wasBeingInteractedWith = isBeingInteractedWith;
            return;
        }

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // Cek apakah interaksi baru saja selesai di frame ini
        bool justReleased = wasBeingInteractedWith && !isBeingInteractedWith;

        // --- KONDISI BARU DI SINI ---
        // Jadikan Kinematic jika:
        // 1. Baru saja mendarat, ATAU
        // 2. Berada di tanah dan baru saja dilepaskan
        if ((isGrounded && !wasGrounded) || (isGrounded && justReleased))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
        else if (!isGrounded && wasGrounded)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        
        // Update status interaksi untuk frame berikutnya
        wasBeingInteractedWith = isBeingInteractedWith;
    }

    public virtual void StartInteraction(Transform playerTransform)
    {
        isBeingInteractedWith = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public virtual void EndInteraction()
    {
        isBeingInteractedWith = false;
    }
    
    // ... (Sisa metode lain tidak berubah) ...
    public virtual void DestroyObject()
    {
        Debug.Log($"{gameObject.name} hancur.");
        Destroy(gameObject);
    }
    public virtual void OnSteppedOn() { }
    public virtual void OnLeft() { }
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}