using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        // Memainkan musik tema utama saat game dimulai
        AudioManager.Instance.PlayMusic("MainMenuTheme");
    }
    public void StartGame()
    {
        // Ganti "Game" dengan nama scene level pertamamu
        SceneManager.LoadScene("Game"); 
    }

    public void GoToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}