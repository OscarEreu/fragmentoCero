using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Refs")]
    public BallController ball;
    public PaddleController paddle;
    public List<Block> allBlocks = new List<Block>();

    [Header("UI")]
    public Text scoreText;
    public Text livesText;

    [Header("Game")]
    public int score = 0;
    public int lives = 3;

    void Awake() { Instance = this; }

    void Start()
    {
        UpdateUI();
    }

    void FixedUpdate()
    {
        if (ball == null || paddle == null) return;

        CheckBallPaddleCollision();
        CheckBallBlocksCollision();
        CheckBallOutOfBounds();
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
                ball.ResolveCollision(normal, b.bounciness, closest, penetration);
                b.OnHit(ball);
            }
        }
    }

    void CheckBallOutOfBounds()
    {
        Rect bounds = ball.CameraBounds();
        if (ball.transform.position.y - ball.radius < bounds.yMin)
        {
            lives--;
            UpdateUI();
            // Reset bola y no lanzada
            ball.launched = false;
            ball.velocity = Vector2.zero;
            ball.transform.position = paddle.transform.position + (Vector3)ball.launchOffset;

            if (lives <= 0)
            {
                Debug.Log("Game Over");
                // Aquí puedes cargar menú o reiniciar
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
    }

    public void OnPowerupCollected(Powerup p)
    {
        // Implementa efectos de powerup (ej: +1 vida, expand paddle, etc)
        Debug.Log("Powerup recogido");
        // Ejemplo: +100 puntos
        AddScore(100);
    }
}
