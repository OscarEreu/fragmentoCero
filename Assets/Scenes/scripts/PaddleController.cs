using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 10f;
    public Transform ballAttach; // referencia para posicionar la bola al inicio

    private float limit;

    void Start()
    {
        // Calcula automáticamente el límite visible de la cámara
        float halfPaddleWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2f;
        float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        limit = screenHalfWidth - halfPaddleWidth;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Vector3 pos = transform.position;
        pos.x += h * speed * Time.deltaTime;

        // Limita el movimiento para que no se salga de la pantalla
        pos.x = Mathf.Clamp(pos.x, -limit, limit);
        transform.position = pos;
    }

    // Devuelve una normal custom según el punto de contacto (para dar control al jugador)
    public Vector2 GetCollisionNormal(Vector2 contactPoint)
    {
        Vector2 local = contactPoint - (Vector2)transform.position;
        float halfWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2f;
        float normalizedX = Mathf.Clamp(local.x / halfWidth, -1f, 1f);
        Vector2 n = new Vector2(normalizedX, 1f).normalized;
        return n;
    }
}
