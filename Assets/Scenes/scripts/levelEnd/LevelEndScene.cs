using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelEndScene : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoriaPanel;   // Contiene texto Victoria + botón Next
    public GameObject derrotaPanel;    // Contiene texto Derrota + botón Retry

    public TextMeshProUGUI scoreText;
    public Button homeButton;
    public Button nextButton;
    public Button retryButton;

    void Start()
    {
        int score = PlayerPrefs.GetInt("LastScore", 0);
        bool victory = PlayerPrefs.GetInt("LastVictory", 0) == 1;
        string levelName = PlayerPrefs.GetString("LastLevel", "Nivel1");

        // Mostrar puntaje
        scoreText.text = "Score: " + score;

        // Activar solo el panel correcto
        victoriaPanel.SetActive(victory);
        derrotaPanel.SetActive(!victory);

        // Botón Home (siempre visible)
        homeButton.onClick.AddListener(GoHome);

        // Si ganó
        if (victory)
        {
            nextButton.onClick.AddListener(NextLevel);
        }
        // Si perdió
        else
        {
            retryButton.onClick.AddListener(RetryLevel);
        }
    }

    // -------------------------------
    // Métodos públicos para OnClick()
    // -------------------------------

    public void GoHome()
    {
        SceneManager.LoadScene("Niveles");
    }

    public void RetryLevel()
    {
        string levelName = PlayerPrefs.GetString("LastLevel", "Nivel1");
        SceneManager.LoadScene(levelName);
    }

    public void NextLevel()
    {
        string levelName = PlayerPrefs.GetString("LastLevel", "Nivel1");
        int n = ExtractLevelNumber(levelName);
        string next = "Nivel" + (n + 1);
        SceneManager.LoadScene(next);
    }

    int ExtractLevelNumber(string name)
    {
        string digits = "";
        foreach (char c in name)
            if (char.IsDigit(c)) digits += c;

        return int.Parse(digits);
    }
}
