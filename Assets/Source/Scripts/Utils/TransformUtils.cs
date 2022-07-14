using System.Collections.Generic;
using UnityEngine;

namespace com.murka.utils
{
    public static class TransformUtils
    {
        #region Public Methods and Operators

        public static void DestroyChildren(this Transform target)
        {
            var children = new List<GameObject>();
            foreach (Transform child in target)
                children.Add(child.gameObject);
            children.ForEach(Object.Destroy);
            children.Clear();
        }

        public static float GetScaleY(this Transform target)
        {
            return target.localScale.y;
        }

        public static void SetLocalPosition(this Transform target, float x, float y)
        {
            var localPos = target.localPosition;
            localPos.x = x;
            localPos.y = y;
            target.localPosition = localPos;
        }

        public static void SetLocalPositionX(this Transform target, float val)
        {
            var localPos = target.localPosition;
            localPos.x = val;
            target.localPosition = localPos;
        }

        public static void SetLocalPositionY(this Transform target, float val)
        {
            var localPos = target.localPosition;
            localPos.y = val;
            target.localPosition = localPos;
        }

        public static void SetLocalPositionZ(this Transform target, float val)
        {
            var localPos = target.localPosition;
            localPos.z = val;
            target.localPosition = localPos;
        }

        public static void SetScaleY(this Transform target, float val)
        {
            var scale = target.localScale;
            scale.y = val;
            target.localScale = scale;
        }

        #endregion
    }
}