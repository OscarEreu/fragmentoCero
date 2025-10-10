using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 10f;
    public float limit = 4.5f; // distancia máxima en X
    public Transform ballAttach; // referencia para posicionar la bola al inicio

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Vector3 pos = transform.position;
        pos.x += h * speed * Time.deltaTime;
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
