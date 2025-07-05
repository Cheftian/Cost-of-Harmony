using UnityEngine;

public class PushOnlyObject : ObjectBase
{
    [Header("Interaction Modifiers")]
    public float speedModifier = 0.5f;
    public override void StartInteraction(Transform playerTransform)
    {
        base.StartInteraction(playerTransform);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    /// <summary>
    /// Saat interaksi selesai, biarkan ground check yang mengambil alih.
    /// </summary>
    public override void EndInteraction()
    {
        base.EndInteraction();
    }
}