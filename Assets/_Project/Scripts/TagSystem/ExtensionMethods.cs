using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tags = MultipleTagSystem.TagSystem.Tags;

namespace MultipleTagSystem
{
    public static class ExtensionMethods
    {
        public static bool HasTag(this GameObject gameObject, Tags tag)
        {
            bool hasTag = false;

            if (gameObject.TryGetComponent(out TagManager tagManager))
            {
                hasTag = tagManager.HasTag(tag);
            }

            return hasTag;
        }

        public static bool HasTag(this Collision2D collision, Tags tag)
        {
            bool hasTag = false;

            if (collision.gameObject.TryGetComponent(out TagManager tagManager))
            {
                hasTag = tagManager.HasTag(tag);
            }

            return hasTag;
        }

        public static bool HasTag(this Collider2D collision, Tags tag)
        {
            bool hasTag = false;

            if (collision.TryGetComponent(out TagManager tagManager))
            {
                hasTag = tagManager.HasTag(tag);
            }

            return hasTag;
        }
    }
}
