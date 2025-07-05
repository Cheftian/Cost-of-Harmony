using UnityEngine;

public class PlayerGrabber : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRadius = 0.5f;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerController playerController;
    private ObjectBase currentInteractedObject;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentInteractedObject == null)
            {
                TryGrabObject();
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (currentInteractedObject != null)
            {
                ReleaseObject();
            }
        }
    }

    private void TryGrabObject()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(interactionPoint.position, interactionRadius, interactableLayer);
        float closestDistance = float.MaxValue;
        Collider2D closestObjectCollider = null;

        foreach (Collider2D col in detectedObjects)
        {
            float distance = Vector2.Distance(interactionPoint.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObjectCollider = col;
            }
        }

        if (closestObjectCollider != null)
        {
            currentInteractedObject = closestObjectCollider.GetComponent<ObjectBase>();
            if (currentInteractedObject != null)
            {
                currentInteractedObject.StartInteraction(transform);

                // Logika yang disederhanakan
                if (currentInteractedObject is GrabbableObject grabbable)
                {
                    playerController.ApplyModifiers(grabbable.GetSpeedModifier(), grabbable.GetJumpModifier());
                }
                else if (currentInteractedObject is PushOnlyObject pushOnly)
                {
                    playerController.ApplyModifiers(pushOnly.speedModifier, 0f);
                }
            }
        }
    }

    private void ReleaseObject()
    {
        if (currentInteractedObject == null) return;

        // Selalu reset modifier pemain saat interaksi apa pun selesai
        playerController.ResetModifiers();

        currentInteractedObject.EndInteraction();
        currentInteractedObject = null;
    }

    private void OnDrawGizmos()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRadius);
        }
    }
}