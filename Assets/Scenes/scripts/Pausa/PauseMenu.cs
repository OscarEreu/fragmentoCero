using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        Debug.Log("Update funcionando");

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Presioné P");

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Detiene el juego
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Reanuda el juego
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Niveles"); // Tu menú principal
    }

    // INICIO
    public void GoToInicio()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); 
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
