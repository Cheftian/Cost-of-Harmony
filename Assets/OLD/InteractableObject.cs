using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class InteractableObject : MonoBehaviour
{
    public Vector2 interactionPointOffset = Vector2.zero;
    public Transform currentStackPoint = null;

    protected Rigidbody2D rb;
    protected Collider2D objectCollider;
    protected bool isBeingDragged = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        objectCollider = GetComponent<Collider2D>();
        if (rb == null) Debug.LogError("Rigidbody2D not found on " + gameObject.name + " InteractableObject!");
        if (objectCollider == null) Debug.LogError("Collider2D not found on " + gameObject.name + " InteractableObject!");
    }

    void Start() { }

    public Vector2 GetInteractionPoint()
    {
        return (Vector2)transform.position + interactionPointOffset;
    }

    public virtual void StartDrag()
    {
        isBeingDragged = true;
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero; // KOREKSI: Gunakan velocity untuk Rigidbody2D
        }
        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }
        currentStackPoint = null;
        Debug.Log($"Mulai drag {gameObject.name}");
    }

    public void DragTo(Vector2 targetPos)
    {
        if (rb == null) { rb = GetComponent<Rigidbody2D>(); if (rb == null) return; }
        rb.MovePosition(targetPos);
    }

    public virtual void Drop()
    {
        isBeingDragged = false;
        // Logika gravityScale dan collider.enabled akan ditangani di HanoiDisk.Drop() atau di subclass spesifik
        Debug.Log($"Objek {gameObject.name} di-drop.");
    }

    public void SetCurrentStackPoint(Transform stackPoint)
    {
        currentStackPoint = stackPoint;
    }

    public bool IsBeingDragged()
    {
        return isBeingDragged;
    }
    
}