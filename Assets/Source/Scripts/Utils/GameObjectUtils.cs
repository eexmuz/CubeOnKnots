using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public static class GameObjectUtils
{
    #region Public Methods and Operators

    public static Component CopyComponent(Component original, GameObject destination)
    {
        if (original == null) throw new NullReferenceException("Original object is null");

        var type = original.GetType();
        var copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
#if !NETFX_CORE
        var fields = type.GetFields();
#else
        IEnumerable<FieldInfo> fields = type.GetRuntimeFields();
#endif
        foreach (var field in fields)
            field.SetValue(copy, field.GetValue(original));
        return copy;
    }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        if (original == null) throw new NullReferenceException("Original object is null");
        var type = original.GetType();
        var copy = destination.AddComponent(type);
#if !NETFX_CORE
        var fields = type.GetFields();
#else
        IEnumerable<FieldInfo> fields = type.GetRuntimeFields();
#endif
        foreach (var field in fields)
            field.SetValue(copy, field.GetValue(original));
        return copy as T;
    }

    public static GameObject CreateGameObject(GameObject source, Transform parent = null)
    {
        var newGameObject = Object.Instantiate(source);
        var pos = newGameObject.transform.position;
        if (parent != null)
            newGameObject.transform.parent = parent;
        newGameObject.transform.localPosition = pos;

        return newGameObject;
    }

    public static GameObject DecapsulateGameObject(GameObject obj)
    {
        var incapsObj = obj.transform.GetChild(0).gameObject;
        incapsObj.transform.parent = obj.transform.parent;
        incapsObj.transform.localPosition = obj.transform.localPosition;
        Object.Destroy(obj);
        return incapsObj;
    }

    public static void DoWithAllChilds(Transform parent, Action<Transform> processMethod)
    {
        processMethod(parent);
        foreach (Transform tmp in parent.transform)
            DoWithAllChilds(tmp, processMethod);
    }

    public static List<GameObject> GetAllChildren(Transform root)
    {
        var children = new List<GameObject>();
        for (var i = 0; i < root.childCount; i++)
            children.Add(root.GetChild(i).gameObject);
        return children;
    }

    public static List<T> GetChildsOfType<T>(GameObject parent) where T : Component
    {
        var items = new List<T>();
        foreach (Transform tmp in parent.transform)
        {
            var item = tmp.GetComponent<T>();
            if (item == null)
                continue;

            var itemPosition = item.transform.position;

            int i;
            for (i = 0; i < items.Count; ++i)
            {
                var tmpItemPosition = items[i].transform.position;
                if (itemPosition.y > tmpItemPosition.y ||
                    itemPosition.x <= tmpItemPosition.x && itemPosition.y >= tmpItemPosition.y)
                    break;
            }
            items.Insert(i, item);
        }
        return items;
    }

    public static List<T> GetChildsOfTypeById<T>(GameObject parent) where T : Component
    {
        string[] itemNameParts;

        var items = new List<T>();
        foreach (Transform tmp in parent.transform)
        {
            var item = tmp.GetComponent<T>();
            if (item == null)
                continue;

            itemNameParts = tmp.gameObject.name.Split('_');
            if (itemNameParts.Length < 2)
                continue;
            var itemId = Convert.ToInt32(itemNameParts[itemNameParts.Length - 1]);

            int i;
            for (i = 0; i < items.Count; ++i)
            {
                itemNameParts = items[i].gameObject.name.Split('_');
                var tmpItemId = Convert.ToInt32(itemNameParts[itemNameParts.Length - 1]);

                if (itemId < tmpItemId)
                    break;
            }
            items.Insert(i, item);
        }
        return items;
    }

    public static GameObject IncapsulateGameObject(Transform obj, Vector3 deltaPos, Vector3 deltaContPos)
    {
        var iconCont = new GameObject();
        iconCont.transform.parent = obj.parent;
        iconCont.transform.localPosition = obj.localPosition + deltaPos;
        obj.parent = iconCont.transform;
        obj.localPosition = -deltaPos;
        iconCont.transform.localPosition += deltaContPos;
        return iconCont;
    }

    public static GameObject SafeGetGameObject(Component c)
    {
        try
        {
            return c != null ? c.gameObject : null;
        }
        catch (Exception e)
        {
            Debug.Log("GameObject is null: " + e.Message);
        }
        return null;
    }

    public static void SafeSetActive(this GameObject gameObject, bool enabled)
    {
        if (gameObject != null) gameObject.SetActive(enabled);
    }

    public static void ResetForces(this Rigidbody2D rigidbody)
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
    }

    #endregion
}