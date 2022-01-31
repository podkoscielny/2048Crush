using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace Crush2048
{
    [CreateAssetMenu(fileName = "ObjectPool", menuName = "ScriptableObjects/ObjectPool")]
    public class ObjectPool : ScriptableObject
    {
        [System.Serializable]
        public struct Pool
        {
            public Tags tag;
            public GameObject prefab;
            public int size;
        }

        [SerializeField] List<Pool> pools;
        [SerializeField] Dictionary<Tags, Queue<GameObject>> poolDictionary;

        private void OnDisable() => poolDictionary = new Dictionary<Tags, Queue<GameObject>>();

        public void InitializePool()
        {
            poolDictionary = new Dictionary<Tags, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.AddComponent<PooledObject>().SetPoolTag(pool.tag);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public void AddToPool(Tags tag, GameObject instance)
        {
            if (!poolDictionary.ContainsKey(tag)) return;

            Queue<GameObject> pool = poolDictionary[tag];

            instance.SetActive(false);
            pool.Enqueue(instance);
        }

        public GameObject GetFromPool(Tags tag)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                return null;
            }
            else if (poolDictionary[tag].Count < 1)
            {
                GameObject desiredPrefab = pools.Find(p => p.tag == tag).prefab;
                GameObject desiredObject = Instantiate(desiredPrefab);

                return desiredObject;
            }
            else
            {
                Queue<GameObject> pool = poolDictionary[tag];
                GameObject objectToSpawn = pool.Dequeue();

                objectToSpawn.SetActive(true);

                return objectToSpawn;
            }
        }

        public GameObject GetFromPool(Tags tag, Vector3 position)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                return null;
            }
            else if (poolDictionary[tag].Count < 1)
            {
                GameObject desiredPrefab = pools.Find(p => p.tag == tag).prefab;
                GameObject desiredObject = Instantiate(desiredPrefab);
                desiredObject.transform.position = position;

                return desiredObject;
            }
            else
            {
                Queue<GameObject> pool = poolDictionary[tag];
                GameObject objectToSpawn = pool.Dequeue();

                objectToSpawn.transform.position = position;
                objectToSpawn.SetActive(true);

                return objectToSpawn;
            }
        }

        public GameObject GetFromPool(Tags tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                return null;
            }
            else if (poolDictionary[tag].Count < 1)
            {
                GameObject desiredPrefab = pools.Find(p => p.tag == tag).prefab;
                GameObject desiredObject = Instantiate(desiredPrefab);
                desiredObject.transform.SetPositionAndRotation(position, rotation);

                return desiredObject;
            }
            else
            {
                Queue<GameObject> pool = poolDictionary[tag];
                GameObject objectToSpawn = pool.Dequeue();

                objectToSpawn.transform.SetPositionAndRotation(position, rotation);
                objectToSpawn.SetActive(true);

                return objectToSpawn;
            }
        }

        public bool IsTagInDictionary(Tags tag) => poolDictionary.ContainsKey(tag);
    }
}
