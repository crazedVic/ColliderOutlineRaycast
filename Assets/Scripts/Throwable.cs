using UnityEngine;

public class Throwable : MonoBehaviour
{
    public GameObject throwableObject; // Reference to the child object
    public float rayLength = 10f;
    private Vector3[] raycastOffsets;

    [SerializeField] private float forceZ = 10f; // Force applied along the Z-axis
    [SerializeField] private float forceY = 5f;  // Force applied along the Y-axis
    private Rigidbody rb;

    void Start()
    {
        // Initialize raycast offsets array with a reasonable default size
        raycastOffsets = new Vector3[8];

        // Get the Rigidbody component from the child object
        rb = throwableObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Generate raycast offsets based on the collider type
        Collider collider = throwableObject.GetComponent<Collider>();

        if (collider is SphereCollider)
        {
            GenerateSphereRaycastOffsets(collider as SphereCollider);
        }
        else if (collider is BoxCollider)
        {
            GenerateCubeRaycastOffsets(collider as BoxCollider);
        }
        else if (collider is CapsuleCollider)
        {
            GenerateCapsuleRaycastOffsets(collider as CapsuleCollider);
        }

        // Get the direction of the object's movement based on its velocity
        Vector3 direction = rb.velocity.normalized;

        foreach (Vector3 offset in raycastOffsets)
        {
            Vector3 origin = throwableObject.transform.position + offset; // Follow the child's position
            RaycastHit hit;

            if (Physics.Raycast(origin, direction, out hit, rayLength))
            {
                Debug.DrawRay(origin, direction * hit.distance, Color.red);

                if (hit.collider.CompareTag("Obstacle"))
                {
                    Debug.Log("Hit an obstacle!");
                    // Handle obstacle collision
                }
                else if (hit.collider.CompareTag("Target"))
                {
                    Debug.Log("Hit the target!");
                    // Handle target collision
                }
            }
            else
            {
                Debug.DrawRay(origin, direction * rayLength, Color.green);
            }
        }

        // Apply force when space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyForce();
        }
    }

    void ApplyForce()
    {
        Vector3 force = new Vector3(0, forceY, forceZ);
        rb.AddForce(force, ForceMode.Impulse);
    }

    void GenerateSphereRaycastOffsets(SphereCollider sphereCollider)
    {
        float radius = sphereCollider.radius * throwableObject.transform.localScale.x; // Adjust for scale
        raycastOffsets = new Vector3[]
        {
            new Vector3(radius, 0, 0),
            new Vector3(-radius, 0, 0),
            new Vector3(0, radius, 0),
            new Vector3(0, -radius, 0),
            new Vector3(0, 0, radius),
            new Vector3(0, 0, -radius),
            new Vector3(radius / Mathf.Sqrt(2), radius / Mathf.Sqrt(2), 0),
            new Vector3(-radius / Mathf.Sqrt(2), radius / Mathf.Sqrt(2), 0)
        };
    }

    void GenerateCubeRaycastOffsets(BoxCollider boxCollider)
    {
        Vector3 halfSize = boxCollider.size * 0.5f;
        raycastOffsets = new Vector3[]
        {
            new Vector3(halfSize.x, halfSize.y, halfSize.z),
            new Vector3(-halfSize.x, halfSize.y, halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
            new Vector3(halfSize.x, halfSize.y, -halfSize.z),
            new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
            new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            new Vector3(-halfSize.x, -halfSize.y, -halfSize.z)
        };
    }

    void GenerateCapsuleRaycastOffsets(CapsuleCollider capsuleCollider)
    {
        float radius = capsuleCollider.radius * throwableObject.transform.localScale.x;
        float height = capsuleCollider.height * throwableObject.transform.localScale.y * 0.5f - radius;
        raycastOffsets = new Vector3[]
        {
            new Vector3(radius, 0, height),
            new Vector3(-radius, 0, height),
            new Vector3(radius, 0, -height),
            new Vector3(-radius, 0, -height),
            new Vector3(0, radius, height),
            new Vector3(0, -radius, height),
            new Vector3(0, radius, -height),
            new Vector3(0, -radius, -height)
        };
    }
}
