using UnityEngine;
using System.Collections.Generic;

public class HanoiPuzzleManager : MonoBehaviour
{
    public List<HanoiPeg> allHanoiPegs; // Daftar semua tiang puzzle Hanoi
    public HanoiPeg targetPeg; // Tiang tujuan untuk memecahkan puzzle (misal: tiang paling kanan)
    public int totalDisks = 3; // Jumlah disk dalam puzzle ini

    void Start()
    {
        if (allHanoiPegs == null || allHanoiPegs.Count < 3)
        {
            Debug.LogError("HanoiPuzzleManager membutuhkan setidaknya 3 HanoiPegs di daftar 'All Hanoi Pegs'.");
        }
        if (targetPeg == null)
        {
            Debug.LogError("HanoiPuzzleManager membutuhkan 'Target Peg' yang ditentukan.");
        }

        // Opsional: Validasi awal disk di tiang awal
        // Pastikan tiang awal memiliki semua disk dengan urutan yang benar
        // Misalnya, jika tiang pertama (allHanoiPegs[0]) adalah tiang awal
        // for (int i = 0; i < totalDisks; i++)
        // {
        //    if (allHanoiPegs[0].stackedDisks.Count != totalDisks || allHanoiPegs[0].stackedDisks[i].diskSize != (totalDisks - i))
        //    {
        //        Debug.LogWarning("Disk awal di tiang pertama tidak diatur dengan benar!");
        //        break;
        //    }
        // }
    }

    void Update()
    {
        CheckForWinCondition();
    }

    void CheckForWinCondition()
    {
        if (targetPeg == null) return;

        // Cek apakah tiang tujuan memiliki semua disk
        if (targetPeg.stackedDisks.Count == totalDisks)
        {
            bool isSolved = true;
            // Cek apakah disk-disk di tiang tujuan tersusun dengan benar (dari terbesar di bawah ke terkecil di atas)
            for (int i = 0; i < targetPeg.stackedDisks.Count; i++)
            {
                // Disk paling bawah harus paling besar, disk kedua paling besar kedua, dst.
                // Jika list diurutkan dari yang paling bawah (terbesar) ke atas (terkecil)
                // maka diskSize[i] harus = totalDisks - i
                // Contoh: diskSize[0] = 3, diskSize[1] = 2, diskSize[2] = 1
                if (targetPeg.stackedDisks[i].diskSize != (totalDisks - i)) // Asumsi urutan disk di list dari bawah ke atas
                {
                    isSolved = false;
                    break;
                }
            }

            if (isSolved)
            {
                Debug.Log("Puzzle Tower of Hanoi SOLVED!");
                // Tambahkan logika untuk apa yang terjadi setelah puzzle terpecahkan:
                // Misalnya, buka pintu, beri petunjuk, atau picu event lain.
                enabled = false; // Nonaktifkan skrip agar tidak terus-menerus mengecek
            }
        }
    }
}