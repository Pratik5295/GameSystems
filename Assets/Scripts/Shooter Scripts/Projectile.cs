using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("The amount of distance a projectile will travel")]
    [SerializeField] private float Range;

    [SerializeField] private ParticleSystem explosionEffect;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private MeshRenderer meshRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void ProjectileMovement(Vector3 direction)
    {

        float distValue = (Range * 9.81f);
        float angleValue = Vector3.Angle(direction, Vector3.up);
        float divValue = Mathf.Sin(2 * Mathf.Deg2Rad * angleValue);
        
        //To avoid getting NaN float value for velocity
        if (divValue < 0)
        {
            divValue = 0.1f;
        }

        // Calculate the initial velocity based on the displacement equation.
        float initialVelocity = Mathf.Sqrt(distValue / divValue);

        // Calculate the initial velocity vector.
        Vector3 initialVelocityVector = direction.normalized * initialVelocity;


        // Access the projectile's Rigidbody component.
        //Rigidbody rb = GetComponent<Rigidbody>();

        
        // Apply the initial velocity to the projectile.
        rb.velocity = initialVelocityVector;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        meshRenderer.enabled = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        explosionEffect.gameObject.SetActive(true);
        explosionEffect.Play();
        Destroy(gameObject,1.5f);
    }
}
