using UnityEngine;
using UnityEngine.Pool;

//object pooling using unitys built in pooling
public class ObjectPoolClass : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private string prefabTag;
    public int initialPoolSize = 20;
    public int maxPoolSize = 40;
    ObjectPool<GameObject> objectPool;


    // Our class's constructor. Takes the prefab to spawn as an argument.
    public void GameObjectPool(GameObject prefab, int initialPoolSize = 20, int maxPoolSize = 40)
    {
        this.prefabToSpawn = prefab;
        this.initialPoolSize = initialPoolSize;
        this.maxPoolSize = maxPoolSize;

        objectPool = new ObjectPool<GameObject>(CreateObject, OnRetrieveFromPool, OnReturnToPool, DestroyObject, true, initialPoolSize, maxPoolSize);
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj = objectPool.Get();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }

    // Wrapper function for pool.Release
    public void ReleaseObject(GameObject obj)
    {
        objectPool.Release(obj);
    }

    //pool methods
    GameObject CreateObject()
    {
        GameObject newObject = Instantiate(prefabToSpawn);
        return newObject;
    }
    void DestroyObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }
    void OnRetrieveFromPool(GameObject pooledObject)
    {
        pooledObject.SetActive(true);

    }
    void OnReturnToPool(GameObject pooledObject)
    {
        pooledObject.SetActive(false);
    }


}