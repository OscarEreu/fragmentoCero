using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BallController : MonoBehaviour
{
    [Header("Estado físico")]
    public Vector2 velocity = Vector2.zero;
    public float radius = 0.16f;

    [Header("Parámetros")]
    public float speedClampMax = 12f;
    public float speedClampMin = 0.5f;
    public bool launched = false;

    [Header("Refs")]
    public Transform paddleTransform;
    public Vector2 launchOffset = new Vector2(0f, 0.4f);

    void Update()
    {
        if (!launched)
        {
            // Sigue la paleta hasta lanzar
            transform.position = (Vector2)paddleTransform.position + launchOffset;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                launched = true;
                velocity = new Vector2(2.5f, 7f); // valor inicial ajustable
            }
        }
    }

    void FixedUpdate()
    {
        if (!launched) return;

        float dt = Time.fixedDeltaTime;
        transform.position = (Vector2)transform.position + velocity * dt;

        // Paredes / Techo
        Rect wb = CameraBounds();
        Vector2 pos = transform.position;

        if (pos.x - radius < wb.xMin)
        {
            pos.x = wb.xMin + radius;
            velocity.x = -velocity.x;
        }
        else if (pos.x + radius > wb.xMax)
        {
            pos.x = wb.xMax - radius;
            velocity.x = -velocity.x;
        }

        if (pos.y + radius > wb.yMax)
        {
            pos.y = wb.yMax - radius;
            velocity.y = -velocity.y;
        }

        transform.position = pos;

        // Limitar velocidad
        float s = velocity.magnitude;
        if (s > speedClampMax) velocity = velocity.normalized * speedClampMax;
        if (s < speedClampMin) velocity = velocity.normalized * speedClampMin;
    }

    public void ResolveCollision(Vector2 contactNormal, float restitution, Vector2 contactPoint, float penetration)
    {
        // Sacar la bola de la penetración
        transform.position = (Vector2)transform.position + contactNormal * penetration;

        // Reflejar la velocidad
        velocity = velocity - 2f * Vector2.Dot(velocity, contactNormal) * contactNormal;

        // Aplicar restitución (bounciness)
        velocity *= restitution;

        // Evitar que la bola se "pegue" a la superficie
        if (Mathf.Abs(Vector2.Dot(velocity.normalized, contactNormal)) < 0.01f)
            velocity += contactNormal * 0.1f;
    }

    public Rect CameraBounds()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        Vector2 center = cam.transform.position;
        return new Rect(center.x - width / 2f, center.y - height / 2f, width, height);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
