using System;
using System.Collections.Generic;
using UnityEngine;
using Tags = TagSystem.TagSystem.Tags;

namespace TagSystem
{
    public class TagManager : MonoBehaviour
    {
        [SerializeField] List<Tags> tags;

        void OnEnable()
        {
            this.CacheObjectToTagSystem(gameObject, tags);
        }

        void OnDisable()
        {
            this.RemoveObjectFromTagSystem(gameObject, tags);
        }

        public bool HasTag(Tags tag) => tags.Contains(tag);
    }
}
