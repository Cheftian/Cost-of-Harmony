using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Untuk menggunakan OrderByDescending

public class HanoiPeg : MonoBehaviour
{
    public float diskSpacingY = 0.2f; // Jarak vertikal antar disk (mungkin masih relevan untuk visualisasi Gizmos saja)
    public LayerMask hanoiDiskLayer; // Layer untuk HanoiDisk

    [Header("Debug")]
    public List<HanoiDisk> stackedDisks = new List<HanoiDisk>(); // Disk yang secara LOGIS berada di tumpukan ini
    public HanoiDisk topDisk = null; // Disk teratas secara LOGIS di tumpukan ini

    // Metode ini dipanggil saat disk memasuki trigger collider Peg
    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hanoiDiskLayer) != 0)
        {
            HanoiDisk disk = other.GetComponent<HanoiDisk>();
            // Tambahkan disk ke daftar jika belum ada dan disk ini sedang tidak di-drag
            // dan belum terasosiasi dengan peg lain yang valid.
            // Cek ini untuk menghindari disk terdaftar di banyak peg jika collider-nya tumpang tindih.
            if (disk != null && !stackedDisks.Contains(disk) && !disk.IsBeingDragged())
            {
                // Disarankan: Tambahkan disk hanya jika posisinya relevan dengan peg ini.
                // Untuk sekarang, kita asumsikan jika masuk trigger peg, dia valid secara fisik.
                stackedDisks.Add(disk);
                UpdateTopDisk(); // Perbarui disk teratas
                Debug.Log($"Disk {disk.name} masuk ke area deteksi Peg {gameObject.name}. Total: {stackedDisks.Count}");
            }
        }
    }

    // Metode ini dipanggil saat disk meninggalkan trigger collider Peg
    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hanoiDiskLayer) != 0)
        {
            HanoiDisk disk = other.GetComponent<HanoiDisk>();
            // Hapus disk dari daftar jika dia benar-benar meninggalkan trigger atau sedang di-drag
            if (disk != null && stackedDisks.Contains(disk) && disk.currentStackPoint != transform) 
            {
                stackedDisks.Remove(disk);
                UpdateTopDisk(); // Perbarui disk teratas
                Debug.Log($"Disk {disk.name} keluar dari area deteksi Peg {gameObject.name}. Sisa: {stackedDisks.Count}");
            }
        }
    }
    
    // Memperbarui topDisk berdasarkan posisi Y fisik disk yang ada
    void UpdateTopDisk()
    {
        // Hapus disk yang mungkin sudah tidak aktif atau null (misalnya, dihancurkan)
        stackedDisks.RemoveAll(d => d == null);

        if (stackedDisks.Count > 0)
        {
            // Temukan disk dengan posisi Y tertinggi di antara yang terdeteksi
            topDisk = stackedDisks.OrderByDescending(d => d.transform.position.y).FirstOrDefault();
        }
        else
        {
            topDisk = null;
        }
    }

    // Metode untuk memeriksa apakah disk bisa diletakkan di tiang ini sesuai aturan Hanoi
    public bool CanPlaceDisk(HanoiDisk newDisk)
    {
        // Panggil UpdateTopDisk terlebih dahulu untuk memastikan topDisk adalah yang teratas secara fisik
        // sebelum validasi aturan.
        UpdateTopDisk(); 

        if (topDisk != null)
        {
            // Aturan Tower of Hanoi: Disk yang lebih besar tidak boleh diletakkan di atas disk yang lebih kecil
            if (newDisk.diskSize > topDisk.diskSize)
            {
                Debug.LogWarning($"Tidak bisa meletakkan Disk {newDisk.name} (Ukuran {newDisk.diskSize}) di atas Disk {topDisk.name} (Ukuran {topDisk.diskSize}). Aturan Hanoi dilanggar.");
                return false;
            }
        }
        Debug.Log($"Disk {newDisk.name} (Ukuran {newDisk.diskSize}) bisa diletakkan di Peg {gameObject.name}.");
        return true;
    }

    // Metode ini dipanggil oleh PlayerInteraction setelah validasi berhasil
    // Peg hanya akan melacak disk secara LOGIS
    public void PlaceDisk(HanoiDisk diskToPlace)
    {
        if (!stackedDisks.Contains(diskToPlace))
        {
            stackedDisks.Add(diskToPlace);
        }
        
        // HAPUS: Baris ini tidak lagi mengatur posisi Y disk secara paksa
        // for (int i = 0; i < stackedDisks.Count; i++)
        // {
        //     float newYPos = transform.position.y + i * diskSpacingY;
        //     stackedDisks[i].transform.position = new Vector2(transform.position.x, newYPos);
        // }
        
        diskToPlace.SetCurrentStackPoint(transform); // Beri tahu disk bahwa dia secara LOGIS berada di peg ini
        UpdateTopDisk(); // Perbarui topDisk setelah penambahan

        // HAPUS: Peg tidak lagi mengubah RigidbodyType disk menjadi Kinematic
        // if (diskToPlace.rb != null)
        // {
        //     diskToPlace.rb.bodyType = RigidbodyType2D.Kinematic;
        //     diskToPlace.rb.gravityScale = 0;
        //     diskToPlace.rb.velocity = Vector2.zero;
        // }

        Debug.Log($"Disk {diskToPlace.name} berhasil diletakkan secara LOGIS di Peg {gameObject.name}. Sekarang akan jatuh/stabil secara fisika.");
    }

    // Metode untuk menghapus disk dari daftar logis peg (saat diambil)
    public void RemoveDisk(HanoiDisk diskToRemove)
    {
        if (stackedDisks.Contains(diskToRemove))
        {
            stackedDisks.Remove(diskToRemove);
            diskToRemove.SetCurrentStackPoint(null); 

            // HAPUS: Baris ini tidak lagi mengatur posisi Y disk yang tersisa secara paksa
            // for (int i = 0; i < stackedDisks.Count; i++)
            // {
            //     float newYPos = transform.position.y + i * diskSpacingY;
            //     stackedDisks[i].transform.position = new Vector2(transform.position.x, newYPos);
            // }

            UpdateTopDisk(); // Perbarui topDisk setelah penghapusan
            Debug.Log($"Disk {diskToRemove.name} diambil dari Peg {gameObject.name}. Sisa disk: {stackedDisks.Count}");
        }
    }

    // Untuk visualisasi di editor (tetap sama)
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(1f, diskSpacingY * 4, 0.1f));
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, diskSpacingY * 4, 0.1f));

        if (topDisk != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, topDisk.transform.position);
            Gizmos.DrawSphere(topDisk.transform.position, 0.1f);
        }
    }
}