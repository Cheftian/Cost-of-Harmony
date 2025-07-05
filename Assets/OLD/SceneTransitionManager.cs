using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    public string nextSpawnID = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && !string.IsNullOrEmpty(nextSpawnID))
        {
            // Perubahan di sini: Menggunakan FindObjectsByType
            // FindObjectsSortMode.None dipilih karena kita tidak peduli urutan, hanya butuh objeknya
            RoomTrigger[] roomTriggers = Object.FindObjectsByType<RoomTrigger>(FindObjectsSortMode.None);
            
            foreach (RoomTrigger trigger in roomTriggers)
            {
                if (trigger.thisSpawnerID == nextSpawnID)
                {
                    player.transform.position = trigger.GetSpawnPosition();
                    nextSpawnID = "";
                    break;
                }
            }
        }
    }

    public void LoadScene(string sceneName, string spawnID)
    {
        nextSpawnID = spawnID;
        SceneManager.LoadScene(sceneName);
    }
}