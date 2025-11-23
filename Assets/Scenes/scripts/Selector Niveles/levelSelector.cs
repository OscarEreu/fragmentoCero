using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [Header("Level Settings")]
    public string levelName;          // Nombre de la escena (Nivel1, Nivel2, Nivel3)
    public int levelNumber;           // 1, 2 o 3

    [Header("Score Requirement")]
    public int scoreRequired = 0;     // 0 para Nivel1, 500 para Nivel2, 1000 para Nivel3

    [Header("UI")]
    public Image lockImage;
    public Button levelButton;

    private bool unlocked = false;

    public void Start()
    {
        CheckUnlockState();
        UpdateLevelImage();

        levelButton.onClick.AddListener(() => PressSelection());
    }

    private void CheckUnlockState()
    {
        // Nivel1 siempre desbloqueado
        if (levelNumber == 1)
        {
            unlocked = true;
        }
        else
        {
            // Score del nivel anterior
            string previousLevelName = "Nivel" + (levelNumber - 1);
            int previousScore = PlayerPrefs.GetInt("Score_Level_" + previousLevelName, 0);

            unlocked = previousScore >= scoreRequired;
        }

        levelButton.interactable = unlocked;
    }

    private void UpdateLevelImage()
    {
        if (lockImage != null)
            lockImage.gameObject.SetActive(!unlocked);

        if (levelButton != null)
            levelButton.gameObject.SetActive(unlocked);
    }
    private void PressSelection()
    {
        if (unlocked)
        {
            Debug.Log("Loading: " + levelName);
            SceneManager.LoadScene(levelName);
        }
    }
}
