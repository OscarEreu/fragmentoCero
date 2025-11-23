using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public InstructionsManager instructionsManager;

    public void quitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Niveles");
    }
    public void Instructions()
    {
        Debug.Log("Abriendo panel de instrucciones...");
        instructionsManager.Abrir();
    }

}
