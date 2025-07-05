using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Variabel untuk target yang akan diikuti (Player)
    [Header("Target to Follow")]
    [SerializeField] private Transform playerTransform;

    // Variabel untuk mengatur seberapa halus pergerakan kamera
    [Header("Camera Settings")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset; // Jarak antara kamera dan player

    // LateUpdate dipanggil setelah semua fungsi Update selesai.
    // Ini pilihan terbaik untuk kamera agar tidak ada getaran (jitter).
    void LateUpdate()
    {
        // Pastikan playerTransform sudah diisi sebelum mencoba mengikutinya
        if (playerTransform != null)
        {
            // Tentukan posisi tujuan kamera
            Vector3 desiredPosition = playerTransform.position + offset;

            // Gunakan Lerp (Linear Interpolation) untuk pergerakan kamera yang halus
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Terapkan posisi baru ke kamera
            transform.position = smoothedPosition;
        }
    }
}