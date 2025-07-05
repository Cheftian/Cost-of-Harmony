using UnityEngine;

[System.Serializable]
public struct PianoKey
{
    public KeyCode key;
    public string noteName;
    public GameObject keyVisual;
}

public class PianoUIManager : MonoBehaviour
{
    public MusicPuzzleManager puzzleManager;
    public PianoKey[] keys;

    void Start()
    {
        foreach (var key in keys)
        {
            if (key.keyVisual != null) key.keyVisual.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePianoUI();
        }

        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key.key))
            {
                AudioManager.Instance.PlaySFX(key.noteName);
                if (key.keyVisual != null) key.keyVisual.SetActive(true);
                if (puzzleManager != null) puzzleManager.NotePlayed(key.noteName);
            }
            else if (Input.GetKeyUp(key.key))
            {
                if (key.keyVisual != null) key.keyVisual.SetActive(false);
            }
        }
    }

    public void ClosePianoUI()
    {
        // Kembalikan ke musik utama level
        AudioManager.Instance.PlayMusic("Proetta");
        
        gameObject.SetActive(false);
        
        // Opsional: aktifkan kembali script gerakan pemain
        // GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = true;
    }
}