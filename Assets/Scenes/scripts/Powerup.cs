using UnityEngine;

public class Powerup : MonoBehaviour
{
    public Vector2 velocity = Vector2.zero;
    public float gravity = -4f;
    public float fallLimit = -10f;

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        velocity += Vector2.up * gravity * dt;
        transform.position = (Vector2)transform.position + velocity * dt;

        // colisión manual con la paleta
        if (GameManager.Instance != null && GameManager.Instance.paddle != null)
        {
            Bounds paddleB = GameManager.Instance.paddle.GetComponent<SpriteRenderer>().bounds;
            if (paddleB.Contains(transform.position))
            {
                GameManager.Instance.OnPowerupCollected(this);
                Destroy(gameObject);
                return;
            }
        }

        // destruir si cae fuera de cámara abajo
        Rect b = Camera.main ? new Rect(Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect,
                                        Camera.main.transform.position.y - Camera.main.orthographicSize,
                                        Camera.main.orthographicSize * 2f * Camera.main.aspect,
                                        Camera.main.orthographicSize * 2f) : new Rect();
        if (transform.position.y < b.yMin + fallLimit) Destroy(gameObject);
    }
}
