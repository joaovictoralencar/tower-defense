using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab; // The prefab to be pooled
        private int _maxPoolSize; // The maximum number of objects to be pooled
        private readonly Transform _parent; //The parent for the objects in the pool
        private readonly List<T> _pool;

        public int ActiveObjectsCount { get; private set; } //The number of active objects

        // An event for when an object is spawned
        public event System.Action<T> OnObjectSpawn;

        // An event for when an object is activated
        public event System.Action<T> OnObjectActivate;

        // An event for when an object is deactivated
        public event System.Action<T> OnObjectDeactivate;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            ActiveObjectsCount = 0;
            _pool = new List<T>(initialSize);
            _parent = parent;
            for (int i = 0; i < initialSize; i++)
            {
                InitializeObject();
            }
        }

        private void InitializeObject()
        {
            T obj = Object.Instantiate(_prefab, Vector3.zero, Quaternion.identity);
            if (_parent != null) obj.transform.SetParent(_parent);
            obj.gameObject.SetActive(false);
            _pool.Add(obj);
            OnObjectSpawn?.Invoke(obj);
        }

        public T ActivateObject(Vector3 position = default, Quaternion rotation = default)
        {
            //Get inactive and activate
            T component = Get(false);
            
            if (component == null) return null;
            
            component.gameObject.SetActive(true);
        
            //Set position
            Transform componentTransform = component.transform;
            if (position != default) componentTransform.position = position;
            if (rotation != default) componentTransform.rotation = rotation;
        
            ActiveObjectsCount++;
        
            OnObjectActivate?.Invoke(component);
            return component;
        }

        public void DeactivateObject(T obj)
        {
            if (!obj.gameObject.activeInHierarchy) return;
        
            if (_pool.Count >= _maxPoolSize)
                Object.Destroy(obj.gameObject);

            obj.gameObject.SetActive(false);
            ActiveObjectsCount--;
            OnObjectDeactivate?.Invoke(obj);
        }

        private T Get(bool isActive)
        {
            if (_pool.Count == 0)
                InitializeObject();

            T obj = _pool.Find(x => x.gameObject.activeInHierarchy == isActive);
            
            if (obj == null) Debug.LogWarning("Could not find available component for pool");
            return obj;
        }
    
    }
}