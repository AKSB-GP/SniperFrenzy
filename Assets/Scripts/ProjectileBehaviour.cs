using System;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [Header("Behaviour Settings")]

    public float speed = 50f;
    public float maxLifetime = 5f;
    public LayerMask hitLayerMask;
    private float lifetime;

    [Header("TrailSettings")]

    public ProjectileTrailConfig trailConfig;
    public static event Action<Collider> OnEnemyHit;
    private TrailRenderer activeTrail;


    void Start()
    {
        if (trailConfig != null)
        {
            trailConfig.Init();
            activeTrail = trailConfig.SpawnTrail();
            activeTrail.widthCurve = trailConfig.widthCurve;
            activeTrail.transform.SetParent(transform);
            activeTrail.transform.localPosition = Vector3.zero;
            activeTrail.transform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        Vector3 move = transform.forward * step;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, step, hitLayerMask))
        {
            if (activeTrail != null)
            {
                // Parent the trail to the projectile so it follows its movement
                Vector3 start = transform.position;
                Vector3 end = transform.position + transform.forward * trailConfig.missDistance;
                StartCoroutine(trailConfig.AnimateTrail(start, end));
            }
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("EnemyCritical"))
            {
                OnEnemyHit?.Invoke(hit.collider);
            }
            Destroy(gameObject);
        }
        else
        {
            transform.position += move;
            lifetime += Time.deltaTime;
            if (lifetime > maxLifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
