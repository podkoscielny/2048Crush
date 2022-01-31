using System;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace MultipleTagSystem
{
    public class TagManager : MonoBehaviour
    {
        [SerializeField] List<Tags> tags;

        private void OnEnable() => this.CacheObjectToTagSystem(gameObject, tags);

        private void OnDisable() => this.RemoveObjectFromTagSystem(gameObject, tags);

        private void OnValidate() => PreventFromDuplicatingTags();

        public bool HasTag(Tags tag) => tags.Contains(tag);

        private void PreventFromDuplicatingTags()
        {
            List<Tags> tagsWithoutDuplicates = new List<Tags>();

            foreach (Tags tag in tags)
            {
                if (tagsWithoutDuplicates.Contains(tag))
                {
                    tagsWithoutDuplicates.Add(Tags.None);
                    continue;
                }

                tagsWithoutDuplicates.Add(tag);
            }

            tags = tagsWithoutDuplicates;
        }
    }
}
