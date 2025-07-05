using System.Collections;
using UnityEngine;

public class VulnerableObject : GrabbableObject // Mewarisi dari GrabbableObject
{
    [Header("Vulnerable Properties")]
    [Tooltip("Jeda waktu sebelum objek hancur setelah ditinggalkan pemain.")]
    public float breakDelay = 0.5f;
    [Tooltip("Partikel atau efek visual yang muncul saat hancur (opsional).")]
    public GameObject breakEffectPrefab;

    private bool isBreaking = false;

    /// <summary>
    /// Override metode OnLeft dari ObjectBase.
    /// Metode ini akan dipanggil oleh PlayerController.
    /// </summary>
    public override void OnLeft()
    {
        base.OnLeft();
        // Mulai proses penghancuran jika belum dimulai
        if (!isBreaking)
        {
            StartCoroutine(BreakAfterDelay());
        }
    }

    private IEnumerator BreakAfterDelay()
    {
        isBreaking = true;
        // Tunggu sejenak sesuai delay
        yield return new WaitForSeconds(breakDelay);

        // Munculkan efek hancur jika ada
        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
        }

        // Hancurkan objek ini
        DestroyObject();
    }
}