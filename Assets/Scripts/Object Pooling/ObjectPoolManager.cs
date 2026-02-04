using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private bool _addToDontDestroyOnLoad = false;

    private GameObject _emptyHolder;

    private static GameObject _bulletsEmpty;

    private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    private static Dictionary<GameObject, GameObject> _cloneToPrefabMap;

    public enum PoolType
    {
        Bullets,
    }

    private void Awake()
    {
        _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        _cloneToPrefabMap = new Dictionary<GameObject, GameObject>();
    }

    private void SetupEmpties()
    {
        _emptyHolder = new GameObject("Object Pools");

        _bulletsEmpty = new GameObject("Bullets");
        _bulletsEmpty.transform.parent = _emptyHolder.transform;
    }

    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.Bullets)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, pos, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject
            );
        _objectPools.Add(prefab, pool);
    }

    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.Bullets)
    {
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, pos, rot);

        prefab.SetActive(true);

        GameObject parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);

        return obj;
    }

    private static void OnGetObject(GameObject obj)
    { 
        //logic for when object is taken from pool
    }
    private static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    private static void OnDestroyObject(GameObject obj)
    {
        if (_cloneToPrefabMap.ContainsKey(obj))
        {
            _cloneToPrefabMap.Remove(obj);
        }
    }
     
    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Bullets:

                return _bulletsEmpty;

            default:
                return null;
        }
    }

    private static T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.Bullets) where T : Object
    {
        if (!_objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);
        }

        GameObject obj = _objectPools[objectToSpawn].Get();

        if (obj != null)
        {
            if (!_cloneToPrefabMap.ContainsKey(obj))
            {
                _cloneToPrefabMap.Add(obj, objectToSpawn);
            }

            obj.transform.position = spawnPos;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }
            
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
                return null;
            }

            return component;
        }
        return null;
    }

    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.Bullets) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
    }
}
