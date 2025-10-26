using UnityEngine;
using System.Collections;

public enum BlockColorType { Azul, Verde, Amarillo, Naranja, Rojo, Morado }

public class Block : MonoBehaviour
{
    public BlockColorType blockType = BlockColorType.Azul;
    int hits = 0;
    int hitsRequired;
    int scoreValue;
    public float restitution; // rebote
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.white;
        ApplyBlockProperties();
        ApplyColorVisual();

        if (GameManager.Instance != null)
            GameManager.Instance.allBlocks.Add(this);
    }

    void ApplyBlockProperties()
    {
        switch (blockType)
        {
            case BlockColorType.Azul:
                restitution = 0.9f; // rebote leve
                scoreValue = 150;
                hitsRequired = 1;
                break;

            case BlockColorType.Verde:
                restitution = 1.0f; // rebote medio
                scoreValue = 125;
                hitsRequired = 2;
                break;


            case BlockColorType.Amarillo:
                restitution = 0.85f; // rebote débil
                scoreValue = 50;
                hitsRequired = 2;
                break;

            case BlockColorType.Naranja:
                restitution = 1f; // rebote normal
                scoreValue = 75;
                hitsRequired = 1;
                break;


            case BlockColorType.Rojo:
                restitution = 1.15f; // rebote fuerte
                scoreValue = 100;
                hitsRequired = 1;
                break;


            case BlockColorType.Morado:
                restitution = 1.15f; // rebote fuerte
                scoreValue = 200;
                hitsRequired = 2;
                break;
        }
    }

    void ApplyColorVisual()
    {
        switch (blockType)
        {
            case BlockColorType.Azul: sr.color = Color.cyan; break;
            case BlockColorType.Verde: sr.color = Color.green; break;
            case BlockColorType.Amarillo: sr.color = Color.yellow; break;
            case BlockColorType.Naranja: sr.color = Color.yellow; break;
            case BlockColorType.Rojo: sr.color = Color.red; break;
            case BlockColorType.Morado: sr.color = new Color(0.6f, 0f, 0.6f); break;
        }
    }

    public void OnHit(BallController ball)
    {
        hits++;
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        // Efectos especiales
        switch (blockType)
        {
            case BlockColorType.Azul:
                // Cambia el color de la bola
                ball.GetComponent<SpriteRenderer>().color = sr.color;
                break;
            case BlockColorType.Verde:
                // Rebote medio, pero el bloque se rompe más lento
                break;

            case BlockColorType.Amarillo:
                ball.StartCoroutine(BlinkBall(ball));// Desaparece la pelota 
                break;
            case BlockColorType.Naranja:
                // Mantiene la velocidad
                ball.velocity += Vector2.down * 1.2f;
                break;


            case BlockColorType.Rojo:
                ball.velocity *= 1.20f; // Aumenta velocidad
                break;



            case BlockColorType.Morado:
                // Bola cae más rápido después del impacto
                ball.velocity += Vector2.down * 1.5f;
                break;
        }

        // Rebote físico (coeficiente de restitución)
        ball.velocity *= restitution;

        // Ver si el bloque se destruye
        if (hits >= hitsRequired)
            Destroy(gameObject);
        else
            sr.transform.localScale *= 0.95f; // Pequeño feedback visual
    }
    IEnumerator BlinkBall(BallController ball)
    {
        SpriteRenderer ballRenderer = ball.GetComponent<SpriteRenderer>();

        for (int i = 0; i < 6; i++) // 6 parpadeos rápidos
        {
            ballRenderer.enabled = !ballRenderer.enabled;
            yield return new WaitForSeconds(0.2f);
        }

        ballRenderer.enabled = true; // se asegura que al final quede visible
    }

}
