using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "ProjectileTrailConfig", menuName = "Scriptable Objects/ProjectileTrailConfig")]
public class ProjectileTrailConfig : ScriptableObject
{
    [Header("Materials")]
    public Material material;
    public AnimationCurve widthCurve;
    public Gradient color;
    [Header("Characteristics")]

    public float duration = 10.2f;
    public float minVertexDistance = 0.1f;
    public float missDistance = 100f;
    public float simualationSpeed = 20f;
    public bool isTrailEmitting = true;
    //probably overkill to use Objectpool in this instance since the rifle is bolt action and thus the rof is low
    //however this leaves room for faster firing weapons in the future
    public ObjectPool<TrailRenderer> TrailPool;

    public void Init()
    {
        if (TrailPool == null)
            TrailPool = new ObjectPool<TrailRenderer>(SpawnTrail);
    }

    public IEnumerator AnimateTrail(Vector3 start, Vector3 end)
    {

        TrailRenderer trailInstance = TrailPool.Get();
        trailInstance.widthCurve = widthCurve;
        trailInstance.gameObject.SetActive(true);
        trailInstance.transform.position = start;
        //start trail next frame
        yield return null;

        trailInstance.emitting = true;
        float traildistance = Vector3.Distance(start, end);
        float distanceleft = traildistance;
        while (distanceleft > 0)
        {
            float t = distanceleft / traildistance;
            trailInstance.transform.position = Vector3.Lerp(start, end,
            Mathf.Clamp01(1 - t));
            //substract distance
            distanceleft -= simualationSpeed * Time.deltaTime; ;
            yield return null;
        }
        //wait till trail is over, remove emit and release from pool
        yield return new WaitForSeconds(duration);
        yield return null;
        trailInstance.emitting = false;

        trailInstance.gameObject.SetActive(false);
        TrailPool.Release(trailInstance);
    }
    public TrailRenderer SpawnTrail()
    {
        GameObject trailInstance = new GameObject("Projectile Trail");
        TrailRenderer trail = trailInstance.AddComponent<TrailRenderer>();
        trail.colorGradient = color;
        trail.material = material;
        trail.emitting = isTrailEmitting;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.widthCurve = widthCurve;
        trail.minVertexDistance = minVertexDistance;
        trail.time = duration;

        return trail;
    }


}
