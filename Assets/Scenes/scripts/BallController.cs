using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BallController : MonoBehaviour
{
    [Header("Estado fisico")]
    public Vector2 velocity = Vector2.zero;
    public float radius = 0.16f;

    [Header("Parametros")]
    public float speedClampMax = 14f;
    public float speedClampMin = 2f;
    public bool launched = false;
    float minRestitution = 0.98f;

    [Header("Referencias")]
    public Transform paddleTransform;
    public Vector2 launchOffset = new Vector2(0f, 0.4f);

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!launched)
        {
            // Sigue la paleta hasta que se lance
            transform.position = (Vector2)paddleTransform.position + launchOffset;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                launched = true;
                velocity = new Vector2(3f, 8f); // velocidad inicial
            }
        }
    }

    void FixedUpdate()
    {
        if (!launched) return;

        float dt = Time.fixedDeltaTime;
        Vector2 pos = transform.position;
        pos += velocity * dt;

        Rect wb = CameraBounds();

        // Rebote en las paredes
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

        // Rebote en el techo
        if (pos.y + radius > wb.yMax)
        {
            pos.y = wb.yMax - radius;
            velocity.y = -velocity.y;
        }

        // Si cae por debajo de la camara, se reinicia
        if (pos.y < wb.yMin - 1f)
        {
            launched = false;
            velocity = Vector2.zero;
        }

        transform.position = pos;

        // Controlar la velocidad para evitar que se dispare o se detenga
        float s = velocity.magnitude;
        if (s > speedClampMax)
            velocity = velocity.normalized * speedClampMax;
        else if (s < speedClampMin)
            velocity = velocity.normalized * speedClampMin;
    }

    public void ResolveCollision(Vector2 contactNormal, float restitution, Vector2 contactPoint, float penetration)
    {
        // Corrige la posicion
        transform.position = (Vector2)transform.position + contactNormal * penetration;

        // Refleja la velocidad segun el angulo de impacto
        velocity = velocity - 2f * Vector2.Dot(velocity, contactNormal) * contactNormal;

        // Aplica rebote segun restitucion
        velocity *= Mathf.Max(restitution, minRestitution);

        // Pequena correccion para evitar "pegado"
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

    // Metodo extra para efectos especiales de bloques (Morado, etc.)
    public void ApplyExtraForce(Vector2 direction, float amount)
    {
        velocity += direction.normalized * amount;
    }
}
