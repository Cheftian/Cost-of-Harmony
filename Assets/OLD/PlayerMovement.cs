using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f; // Kecepatan gerak Elias
    public float stopDistance = 0.1f; // Jarak default ke target bebas sebelum berhenti
    public LayerMask walkableLayer; // Layer yang bisa diinjak (misal: "Ground")

    private Vector2 targetPosition;
    private bool isMoving = false;
    private bool canMove = true;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerInteraction playerInteraction;

    private float currentStopThreshold;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInteraction = GetComponent<PlayerInteraction>();

        targetPosition = transform.position;
        currentStopThreshold = stopDistance;
    }

    void Update()
    {
        // Hanya deteksi klik jika Elias bisa bergerak dan tidak sedang menunggu interaksi (agar tidak override klik interaksi)
        if (canMove && !playerInteraction.IsAwaitingInteraction() && Input.GetMouseButtonDown(0))
        {
            Vector2 mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector2(mouseClickPos.x, transform.position.y);
            isMoving = true;
            currentStopThreshold = stopDistance; // Gunakan stopDistance default untuk klik bebas
        }

        if (isMoving && canMove)
        {
            float directionX = Mathf.Sign(targetPosition.x - transform.position.x);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (directionX > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (directionX < 0)
            {
                spriteRenderer.flipX = true;
            }

            if (Mathf.Abs(targetPosition.x - transform.position.x) < currentStopThreshold)
            {
                isMoving = false;
                // Panggil OnReachedInteractionPoint di PlayerInteraction
                if (playerInteraction != null)
                {
                    playerInteraction.OnReachedInteractionPoint();
                }
            }
        }
        else if (!canMove)
        {
            isMoving = false;
        }
    }

    public void SetTargetPosition(float xPos, float stopThreshold)
    {
        targetPosition = new Vector2(xPos, transform.position.y);
        currentStopThreshold = stopThreshold;
        isMoving = true;
        canMove = true;
    }

    public void CanMove(bool moveAllowed)
    {
        canMove = moveAllowed;
        if (!canMove)
        {
            isMoving = false;
        }
    }
}