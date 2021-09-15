using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyBrain.Utility
{
    public class ObjectPooler<T>
    {
        public class ObjectPoolItem
        {
            public GameObject pooledObject;
            public T component;

            public ObjectPoolItem(GameObject gameobject)
            {
                pooledObject = gameobject;
                component = gameobject.GetComponent<T>();
            }
        }

        public static ObjectPooler<T> SharedInstance;

        private  int amountToPool;
        private bool shouldExpand;

        private List<ObjectPoolItem> pooledObjects;
        private GameObject rootObject;

        public ObjectPooler(int poolAmount, bool shouldExpand = false)
        {
            SharedInstance = this;

            amountToPool = poolAmount;
            shouldExpand = false;

            pooledObjects = new List<ObjectPoolItem>();

            rootObject = new GameObject();
            rootObject.transform.position = new Vector3(-100, -100, -100);
            GameObject.DontDestroyOnLoad(rootObject);

            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = new GameObject($"{typeof(T).Name} ({i})",typeof(T));
                obj.SetActive(false);
                pooledObjects.Add(new ObjectPoolItem(obj));
            }
        }

        public ObjectPoolItem GetPooledObject()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (!pooledObjects[i].pooledObject.activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }

            if (shouldExpand)
            {
                GameObject obj = new GameObject($"{typeof(T).Name} ({pooledObjects.Count})", typeof(T));
                obj.SetActive(false);
                pooledObjects.Add(new ObjectPoolItem(obj));
                return pooledObjects[pooledObjects.Count-1];
            }
            return null;
        }

        public void Destroy()
        {
            GameObject.Destroy(rootObject);
        }
    }
}
