using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level Info")]
   
    [Header("References")]
    public BallController ball;
    public PaddleController paddle;
    public List<Block> allBlocks = new List<Block>();

    [Header("UI (TextMeshPro)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;

    [Header("Game Settings")]
    public int score = 0;
    public int lives = 5;
    public float levelDuration = 180f; // 3 minutes = 180 seconds

    private float timeRemaining;
    private bool levelActive = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timeRemaining = levelDuration;
        UpdateUI();
    }

    void FixedUpdate()
    {
        if (!levelActive) return;
        if (ball == null || paddle == null) return;

        CheckBallPaddleCollision();
        CheckBallBlocksCollision();
        CheckBallOutOfBounds();
    }

    void Update()
    {
        if (!levelActive) return;

        // Timer
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            LevelEnd(false);
        }

        UpdateTimerUI();
    }

    void CheckBallPaddleCollision()
    {
        Bounds paddleB = paddle.GetComponent<SpriteRenderer>().bounds;
        Vector2 circlePos = ball.transform.position;
        float r = ball.radius;

        Vector2 closest = new Vector2(
            Mathf.Clamp(circlePos.x, paddleB.min.x, paddleB.max.x),
            Mathf.Clamp(circlePos.y, paddleB.min.y, paddleB.max.y)
        );

        Vector2 diff = circlePos - closest;
        float dist = diff.magnitude;

        if (dist < r)
        {
            Vector2 normal = dist > 0.0001f ? diff.normalized : Vector2.up;
            float penetration = r - dist;
            Vector2 paddleNormal = paddle.GetCollisionNormal(closest);
            Vector2 finalNormal = (normal + paddleNormal).normalized;
            ball.ResolveCollision(finalNormal, 0.9f, closest, penetration);
        }
    }

    void CheckBallBlocksCollision()
    {
        for (int i = allBlocks.Count - 1; i >= 0; i--)
        {
            Block b = allBlocks[i];
            if (b == null) { allBlocks.RemoveAt(i); continue; }

            // Ignorar bloques que ya están cayendo
            if (b.physicsEnabled) continue;

            Bounds blockB = b.GetComponent<SpriteRenderer>().bounds;
            Vector2 circlePos = ball.transform.position;
            float r = ball.radius;

            Vector2 closest = new Vector2(
                Mathf.Clamp(circlePos.x, blockB.min.x, blockB.max.x),
                Mathf.Clamp(circlePos.y, blockB.min.y, blockB.max.y)
            );

            Vector2 diff = circlePos - closest;
            float dist = diff.magnitude;

            if (dist < r)
            {
                Vector2 normal = dist > 0.0001f ? diff.normalized : Vector2.up;
                float penetration = r - dist;
                ball.ResolveCollision(normal, b.restitution, closest, penetration);
                b.OnHit(ball);
            }
        }

        // LÓGICA DE VICTORIA POR SCORE
        CheckVictoryByScore();
    }

    void CheckVictoryByScore()
    {
        if (!levelActive) return;

        string levelName = SceneManager.GetActiveScene().name;

        bool victory = false;

        if (levelName == "Nivel1" && score >= 3275)
            victory = true;

        if (levelName == "Nivel2" && score >= 7175)
            victory = true;

        if (levelName == "Nivel3" && score >= 18350)
            victory = true;

        if (victory)
        {
            Debug.Log("✔ Victoria por score en " + levelName);
            LevelEnd(true);
        }
    }


    void CheckBallOutOfBounds()
    {
        Rect bounds = ball.CameraBounds();
        if (ball.transform.position.y - ball.radius < bounds.yMin)
        {
            lives--;
            UpdateUI();

            // Reset ball and paddle
            ball.launched = false;
            ball.velocity = Vector2.zero;
            ball.transform.position = paddle.transform.position + (Vector3)ball.launchOffset;

            if (lives <= 0)
            {
                Debug.Log("GAME OVER");
                LevelEnd(false);
            }

        }
    }

    public void AddScore(int v)
    {
        score += v;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score;
        if (livesText) livesText.text = "Lives: " + lives;
        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    void LevelEnd(bool victory)
    {
        levelActive = false;
        Debug.Log("Level finished");

        string currentLevel = SceneManager.GetActiveScene().name;

        // Guardar score por nivel
        PlayerPrefs.SetInt("Score_Level_" + currentLevel, score);

        // Guardar datos para la pantalla final
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.SetInt("LastVictory", victory ? 1 : 0);
        PlayerPrefs.SetString("LastLevel", currentLevel);

        PlayerPrefs.Save();

        // Ir a la escena final
        SceneManager.LoadScene("LevelEndScene");
    }

    public void OnPowerupCollected(Powerup p)
    {
        Debug.Log("Powerup collected");
        //AddScore(100);
    }
    [ContextMenu("🗑 Reset PlayerPrefs")]
    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🔁 PlayerPrefs borrados para pruebas.");
    }

}
