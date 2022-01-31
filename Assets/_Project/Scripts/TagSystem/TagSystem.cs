using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MultipleTagSystem
{
    public static class TagSystem
    {
        [Serializable]
        public enum Tags
        {
            None = 0,
            Tile = 1,
            SpecialTile = 2,
            //PrimaryColor = 3,
            //SecondaryColor = 4,
            //BackgroundColor = 5,
            //TextColor = 6
        }

        private static Dictionary<Tags, List<GameObject>> TaggedObjects = new Dictionary<Tags, List<GameObject>>();
        private static Dictionary<Tags, ReadOnlyCollection<GameObject>> _readonlyObjects = new Dictionary<Tags, ReadOnlyCollection<GameObject>>();

        private static ReadOnlyCollection<GameObject> _emptyList = new List<GameObject>().AsReadOnly();

        static TagSystem()
        {
            InitializeTaggedObjectsDictionary();
        }

        private static void InitializeTaggedObjectsDictionary()
        {
            Type enumType = typeof(Tags);
            var enumValues = Enum.GetValues(enumType);

            foreach (var value in enumValues)
            {
                TaggedObjects.Add((Tags)value, new List<GameObject>());
            }

            foreach (var item in TaggedObjects)
            {
                _readonlyObjects.Add(item.Key, item.Value.AsReadOnly());
            }
        }

        public static GameObject FindGameObjectWithTag(Tags tag)
        {
            if (!TaggedObjects.ContainsKey(tag) || TaggedObjects[tag].Count < 1) return null;

            return TaggedObjects[tag][0];
        }

        public static ReadOnlyCollection<GameObject> FindAllGameObjectsWithTag(Tags tag)
        {
            if (!TaggedObjects.ContainsKey(tag) || TaggedObjects[tag].Count < 1) return _emptyList;

            return _readonlyObjects[tag];
        }

        public static void CacheObjectToTagSystem(this TagManager tagManager, GameObject gameObjectToCache, List<Tags> tags)
        {
            CacheObjectToFindWithTag(gameObjectToCache, tags);
        }

        public static void RemoveObjectFromTagSystem(this TagManager tagManager, GameObject objectToBeRemoved, List<Tags> tags)
        {
            RemoveObjectFromFindWithTag(objectToBeRemoved, tags);
        }

        private static void CacheObjectToFindWithTag(GameObject objectToCache, List<Tags> tags)
        {
            foreach (var item in TaggedObjects)
            {
                if (tags.Contains(item.Key) && item.Key != Tags.None)
                    TaggedObjects[item.Key].Add(objectToCache);
            }
        }

        private static void RemoveObjectFromFindWithTag(GameObject objectToBeRemoved, List<Tags> tags)
        {
            foreach (var item in TaggedObjects)
            {
                if (tags.Contains(item.Key))
                    TaggedObjects[item.Key].Remove(objectToBeRemoved);
            }
        }
    }
}
