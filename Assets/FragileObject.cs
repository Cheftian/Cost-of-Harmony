using UnityEngine;

public class FragileObject : ObjectBase // Cukup mewarisi dari ObjectBase
{
    [Header("Fragile Properties")]
    [Tooltip("Partikel atau efek visual yang muncul saat hancur (opsional).")]
    public GameObject breakEffectPrefab;
    [Tooltip("Collectible (Key/Heal) yang muncul saat hancur (opsional).")]
    public GameObject collectibleToSpawn;

    private bool isBreaking = false;
    
    // Override metode saat dicoba dipegang
    public override void StartInteraction(Transform playerTransform)
    {
        // Jangan panggil base.StartInteraction(), langsung hancurkan.
        BreakAndSpawn();
    }

    // Override metode saat diinjak
    public override void OnSteppedOn()
    {
        base.OnSteppedOn();
        BreakAndSpawn();
    }

    private void BreakAndSpawn()
    {
        // Pastikan proses hancur hanya berjalan sekali
        if (isBreaking) return;
        isBreaking = true;

        // Munculkan efek hancur jika ada
        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
        }

        // Munculkan collectible jika ada
        if (collectibleToSpawn != null)
        {
            Instantiate(collectibleToSpawn, transform.position, Quaternion.identity);
        }
        
        // Hancurkan objek ini
        DestroyObject();
    }
}