using UnityEngine;

namespace WaterProject
{
    /// <summary>
    /// The object class for wave of water. Properties are as follows:
    /// Each wave is a rectanglur cube, the y scale would differ to show wave of different heights
    /// No hard collisions, but triggers to show buoyancy of bodies in water
    /// Waves move towards end point (beach) at a uniform speed before losing height and dying out
    /// </summary>
    public class Wave : MonoBehaviour
    {
        [SerializeField] private float waveHeight;
        [SerializeField] private float waveSpeed;

        [SerializeField] private Transform endPosition;

        [SerializeField] private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            waveHeight = transform.localScale.y;
        }

        private void Update()
        {
            WaveMovement();
        }

        /// <summary>
        /// Simple wave movement from point of origin to end point (beach)
        /// </summary>
        private void WaveMovement()
        {
            Vector3 direction = endPosition.position - transform.position;

            rb.velocity = direction * waveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// On wave gets close to beach, it starts losing its height
        /// </summary>
        private void OnLoseHeight()
        {
            float distance = Vector3.Distance(endPosition.position,transform.position);

            float timeToReachBeach = distance / waveSpeed;
        }
    }
}
