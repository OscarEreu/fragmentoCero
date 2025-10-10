using UnityEngine;

public enum BlockColorType { Azul, Verde, Naranja, Amarillo, Rojo, Morado }

public class Block : MonoBehaviour
{
    public BlockColorType blockType = BlockColorType.Azul;
    public int hitsRequired = 1;
    public int scoreValue = 150;
    [Range(0f, 1f)] public float bounciness = 0.9f;
    public bool spawnPowerup = false;
    public GameObject powerupPrefab;

    int hits = 0;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        ApplyColorVisual();
        if (GameManager.Instance != null) GameManager.Instance.allBlocks.Add(this);
    }

    void ApplyColorVisual()
    {
        switch (blockType)
        {
            case BlockColorType.Azul: sr.color = Color.cyan; break;
            case BlockColorType.Verde: sr.color = Color.green; break;
            case BlockColorType.Naranja: sr.color = new Color(1f, 0.5f, 0f); break;
            case BlockColorType.Amarillo: sr.color = Color.yellow; break;
            case BlockColorType.Rojo: sr.color = Color.red; break;
            case BlockColorType.Morado: sr.color = new Color(0.6f, 0f, 0.6f); break;
        }
    }

    public void OnHit(BallController ball)
    {
        hits++;
        if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);

        // Efectos según tipo (puedes ajustar en inspector)
        switch (blockType)
        {
            case BlockColorType.Azul:
                ball.GetComponent<SpriteRenderer>().color = sr.color;
                break;
            case BlockColorType.Verde:
                ball.velocity *= 0.95f;
                break;
            case BlockColorType.Naranja:
                // sin cambio
                break;
            case BlockColorType.Amarillo:
                ball.velocity *= 0.85f;
                break;
            case BlockColorType.Rojo:
                ball.velocity *= 1.15f;
                break;
            case BlockColorType.Morado:
                ball.velocity += Vector2.down * 1.5f;
                break;
        }

        if (hits >= hitsRequired)
        {
            if (spawnPowerup && powerupPrefab)
                Instantiate(powerupPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            sr.transform.localScale *= 0.95f;
        }
    }
}
