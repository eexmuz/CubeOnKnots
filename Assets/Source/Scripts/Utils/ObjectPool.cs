using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool 
{
    private Queue<GameObject> _inactive;
    private List<GameObject> _active;
    private GameObject _prefab;

    public ObjectPool(GameObject prefab)
    {
        _prefab = prefab;
        _active = new List<GameObject>();
        _inactive = new Queue<GameObject>();
    }
    
    public GameObject Get()
    {
        GameObject obj = _inactive.Count > 0 ? _inactive.Dequeue() : Object.Instantiate(_prefab);
        obj.gameObject.SetActive(true);
        _active.Add(obj);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        if (_active.Contains(obj))
        {
            _active.Remove(obj);
        }
        
        _inactive.Enqueue(obj);
    }
}

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _inactive;
    private List<T> _active;
    private T _prefab;
    
    public ObjectPool(T prefab)
    {
        _prefab = prefab;
        _active = new List<T>();
        _inactive = new Queue<T>();
    }

    public ObjectPool(T prefab, int preallocate, Transform parent = null)
    {
        _prefab = prefab;
        _active = new List<T>(preallocate);
        _inactive = new Queue<T>(preallocate);

        for (int i = 0; i < preallocate; i++)
        {
            T obj = Object.Instantiate(_prefab, parent);
            obj.gameObject.SetActive(false);
            _inactive.Enqueue(obj);
        }
    }

    public T Spawn()
    {
        T obj = _inactive.Count > 0 ? _inactive.Dequeue() : Object.Instantiate(_prefab);
        obj.gameObject.SetActive(true);
        _active.Add(obj);
        return obj;
    }
    
    public void Despawn(T obj)
    {
        obj.gameObject.SetActive(false);
        if (_active.Contains(obj))
        {
            _active.Remove(obj);
        }
        
        _inactive.Enqueue(obj);
    }
}
