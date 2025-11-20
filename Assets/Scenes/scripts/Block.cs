using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BlockColorType
{
    Azul, Verde, Amarillo, Naranja, Rojo, Morado
}

public class Block : MonoBehaviour
{
    [Header("Física por color")]
    public float gravity;
    public float airFriction;
    public float maxFallSpeed;
    public float spinSpeed;

    private Vector2 physicsVelocity;
    private bool physicsEnabled = false;

    public BlockColorType blockType = BlockColorType.Azul;
    int hits = 0;
    int hitsRequired;
    int scoreValue;
    public float restitution;

    SpriteRenderer sr;

    // --- MASA RESORTE ---
    bool springActive = false;
    float springBaseY;
    float springDisplacement = 0f;
    float springVelocity = 0f;
    bool hasSpring = false;
    bool springDestroyed = false; // Una vez que cae, ya no puede volver a oscilar

    // Parámetros masa-resorte
    float k = 50f;  // constante resorte
    float c = 0.3f; // amortiguamiento

    public static List<Block> springBlocks = new List<Block>();
    public static bool springsInitialized = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.white;

        ApplyBlockProperties();
        ApplyColorVisual();

        if (GameManager.Instance != null)
            GameManager.Instance.allBlocks.Add(this);

        if (!springsInitialized)
            StartCoroutine(InitializeSpringSystem());
    }

    IEnumerator InitializeSpringSystem()
    {
        yield return new WaitForEndOfFrame();
        if (springsInitialized) yield break;
        springsInitialized = true;

        Block[] allBlocks = FindObjectsOfType<Block>();
        Dictionary<BlockColorType, List<Block>> eligibleByColor = new Dictionary<BlockColorType, List<Block>>();
        
        foreach (Block block in allBlocks)
        {
            if (!HasBlockBelow(block, allBlocks))
            {
                if (!eligibleByColor.ContainsKey(block.blockType))
                    eligibleByColor[block.blockType] = new List<Block>();
                eligibleByColor[block.blockType].Add(block);
            }
        }

        List<BlockColorType> colors = new List<BlockColorType>(eligibleByColor.Keys);
        int minColors = Mathf.Min(3, colors.Count);
        
        for (int i = 0; i < minColors; i++)
        {
            int idx = Random.Range(0, colors.Count);
            BlockColorType color = colors[idx];
            colors.RemoveAt(idx);
            
            List<Block> colorBlocks = eligibleByColor[color];
            if (colorBlocks.Count > 0)
            {
                Block selected = colorBlocks[Random.Range(0, colorBlocks.Count)];
                selected.hasSpring = true;
                selected.k = Random.Range(45f, 65f);
                selected.c = Random.Range(0.2f, 0.4f);
                selected.sr.color = Color.Lerp(selected.sr.color, Color.white, 0.15f);
                springBlocks.Add(selected);
            }
        }
    }

    bool HasBlockBelow(Block block, Block[] allBlocks)
    {
        foreach (Block other in allBlocks)
        {
            if (other == block) continue;
            if (other.transform.position.y < block.transform.position.y &&
                Mathf.Abs(other.transform.position.x - block.transform.position.x) < 0.5f &&
                Mathf.Abs(other.transform.position.y - block.transform.position.y) < 1.5f)
                return true;
        }
        return false;
    }

    void ApplyBlockProperties()
    {
        switch (blockType)
        {
            case BlockColorType.Azul:
                restitution = 0.9f; scoreValue = 150; hitsRequired = 1;
                gravity = -4f; airFriction = 0.05f; maxFallSpeed = -3f; spinSpeed = 30f;
                break;
            case BlockColorType.Verde:
                restitution = 1.0f; scoreValue = 125; hitsRequired = 2;
                gravity = -6f; airFriction = 0.08f; maxFallSpeed = -4f; spinSpeed = 50f;
                break;
            case BlockColorType.Amarillo:
                restitution = 0.85f; scoreValue = 50; hitsRequired = 2;
                gravity = -8f; airFriction = 0.1f; maxFallSpeed = -6f; spinSpeed = 80f;
                break;
            case BlockColorType.Naranja:
                restitution = 1f; scoreValue = 75; hitsRequired = 1;
                gravity = -5f; airFriction = 0.12f; maxFallSpeed = -4f; spinSpeed = 40f;
                break;
            case BlockColorType.Rojo:
                restitution = 1.15f; scoreValue = 100; hitsRequired = 1;
                gravity = -10f; airFriction = 0.15f; maxFallSpeed = -7f; spinSpeed = 120f;
                break;
            case BlockColorType.Morado:
                restitution = 1.15f; scoreValue = 200; hitsRequired = 2;
                gravity = -12f; airFriction = 0.2f; maxFallSpeed = -8f; spinSpeed = 140f;
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
            case BlockColorType.Naranja: sr.color = Color.white; break;
            case BlockColorType.Rojo: sr.color = Color.red; break;
            case BlockColorType.Morado: sr.color = new Color(0.6f, 0f, 0.6f); break;
        }
    }

    public void OnHit(BallController ball)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        hits++;

        // --- SISTEMA DE RESORTE ---
        if (hasSpring && !springDestroyed)
        {
            // Para bloques con resorte, solo verificamos golpes después del sistema de resorte
            if (springActive)
            {
                // Ya está oscilando
                if (Mathf.Abs(springVelocity) > 0.05f || Mathf.Abs(springDisplacement) > 0.05f)
                {
                    // Está oscilando activamente - CAER
                    springActive = false;
                    springDestroyed = true;
                    ball.velocity *= restitution;
                    ActivatePhysics();
                    return;
                }
                else
                {
                    // Se detuvo - REACTIVAR
                    springDisplacement = 0f;
                    springVelocity = -4f;
                    ball.velocity *= restitution;
                    return;
                }
            }
            else
            {
                // PRIMER TOQUE - ACTIVAR
                springActive = true;
                springBaseY = transform.position.y;
                springDisplacement = 0f;
                springVelocity = -4f;
                ball.velocity *= restitution;
                return;
            }
        }

        // --- BLOQUES NORMALES ---
        
        // Aplicar efectos según el color
        switch (blockType)
        {
            case BlockColorType.Azul:
                ball.GetComponent<SpriteRenderer>().color = sr.color;
                break;
            case BlockColorType.Amarillo:
                StabilizeSpeed(ball);
                StartCoroutine(BlinkBall(ball));
                break;
            case BlockColorType.Naranja:
                ball.velocity += Vector2.down * 1.2f;
                break;
            case BlockColorType.Rojo:
                ball.velocity *= 1.20f;
                break;
            case BlockColorType.Morado:
                ball.velocity += Vector2.down * 1.5f;
                break;
        }

        ball.velocity *= restitution;

        // Solo cae si ha recibido los golpes requeridos
        if (hits >= hitsRequired)
        {
            ActivatePhysics();
        }
    }

    void StabilizeSpeed(BallController ball)
    {
        float s = ball.velocity.magnitude;
        ball.velocity = ball.velocity.normalized * Mathf.Lerp(s, 8f, 0.5f);
    }

    IEnumerator BlinkBall(BallController ball)
    {
        SpriteRenderer ballRenderer = ball.GetComponent<SpriteRenderer>();
        for (int i = 0; i < 6; i++)
        {
            ballRenderer.enabled = !ballRenderer.enabled;
            yield return new WaitForSeconds(0.2f);
        }
        ballRenderer.enabled = true;
    }

    void ActivatePhysics()
    {
        if (physicsEnabled) return;
        physicsEnabled = true;
        sr.transform.localScale *= 0.9f;
        physicsVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, -3f));
    }

    void Update()
    {
        // --- OSCILACIÓN MASA-RESORTE (Método de Euler) ---
        if (springActive)
        {
            float dt = Time.deltaTime;
            
            // Euler: F = -kx - cv
            float accel = (-k * springDisplacement - c * springVelocity);
            
            springVelocity += accel * dt;
            springDisplacement += springVelocity * dt;
            
            transform.position = new Vector3(
                transform.position.x,
                springBaseY + springDisplacement,
                transform.position.z
            );
            return;
        }

        // --- CAÍDA LIBRE ---
        if (!physicsEnabled) return;

        physicsVelocity.y += gravity * Time.deltaTime;
        physicsVelocity *= (1 - airFriction * Time.deltaTime);
        physicsVelocity.y = Mathf.Max(physicsVelocity.y, maxFallSpeed);

        transform.position += (Vector3)(physicsVelocity * Time.deltaTime);
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

        if (transform.position.y < -10f)
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (springBlocks.Contains(this))
            springBlocks.Remove(this);
    }
}