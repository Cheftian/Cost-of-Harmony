using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    // Komponen dan state internal
    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool canFlip = true;

    // Variabel untuk menyimpan nilai asli
    private float baseMoveSpeed;
    private float baseJumpForce;
    private ObjectBase currentStandingObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Simpan nilai awal saat permainan dimulai
        baseMoveSpeed = moveSpeed;
        baseJumpForce = jumpForce;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Gunakan jumpForce yang sudah dimodifikasi (jika ada)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        // Cek ground seperti biasa
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundCollider != null;

        ObjectBase newStandingObject = null;
        if (isGrounded)
        {
            // Jika menyentuh tanah, coba ambil komponen ObjectBase darinya
            newStandingObject = groundCollider.GetComponent<ObjectBase>();
        }

        // Cek apakah objek yang kita injak berubah
        if (newStandingObject != currentStandingObject)
        {
            // Jika kita baru saja meninggalkan objek sebelumnya, panggil OnLeft()
            if (currentStandingObject != null)
            {
                currentStandingObject.OnLeft();
            }

            // Jika kita baru saja menginjak objek baru, panggil OnSteppedOn()
            if (newStandingObject != null)
            {
                newStandingObject.OnSteppedOn();
            }
            
            // Update objek yang sedang diinjak
            currentStandingObject = newStandingObject;
        }

        // Gerakan pemain (tidak berubah)
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        // Jangan lakukan flip jika tidak diizinkan
        if (!canFlip) return;

        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
        }
    }

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }

    // -------------------------------------------------------------------

    // Tambahkan fungsi publik baru ini di bagian bawah skrip:
    /// <summary>
    /// Mengatur apakah pemain bisa membalikkan badan atau tidak.
    /// </summary>
    public void SetCanFlip(bool status)
    {
        canFlip = status;
    }

    // --- FUNGSI BARU UNTUK MEMODIFIKASI STATS ---

    /// <summary>
    /// Menerapkan modifier ke kecepatan gerak dan lompatan.
    /// </summary>
    public void ApplyModifiers(float speedMod, float jumpMod)
    {
        moveSpeed = baseMoveSpeed * speedMod;
        jumpForce = baseJumpForce * jumpMod;
    }

    /// <summary>
    /// Mengembalikan kecepatan gerak dan lompatan ke nilai semula.
    /// </summary>
    public void ResetModifiers()
    {
        moveSpeed = baseMoveSpeed;
        jumpForce = baseJumpForce;
    }
}