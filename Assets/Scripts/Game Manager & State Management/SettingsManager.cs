using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}