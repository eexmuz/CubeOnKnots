using System;
using Core.Attributes;
using UnityEngine;

namespace Core.Services
{
    [InjectionAlias(typeof(IPlayerPrefsService))]
    public class PlayerPrefsService : Service, IPlayerPrefsService
    {
#if UNITY_WEBGL
    public Action<string, object> OnChangeValue;

    private Dictionary<string, object> StorageData;
#endif

        public override void Run()
        {
            base.Run();

#if UNITY_WEBGL
            StorageData = new Dictionary<string, object>();
#endif
        }

        public void DeleteAll()
        {
#if UNITY_WEBGL
            StorageData = new Dictionary<string, object>();
#else
            PlayerPrefs.DeleteAll();
#endif
        }

        public void DeleteKey(string key)
        {
#if UNITY_WEBGL
            if (StorageData.ContainsKey(key))
                StorageData.Remove(key);
#else
            PlayerPrefs.DeleteKey(key);
#endif
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
#if UNITY_WEBGL
            if (!StorageData.ContainsKey(key))
                return defaultValue;
    
            return Convert.ToSingle(StorageData[key]);
#else
            return PlayerPrefs.GetFloat(key, defaultValue);
#endif
        }

        public int GetInt(string key, int defaultValue = 0)
        {
#if UNITY_WEBGL
            if (!StorageData.ContainsKey(key))
                return defaultValue;
    
            return Convert.ToInt32(StorageData[key]);
#else
            return PlayerPrefs.GetInt(key, defaultValue);
#endif
        }

        public string GetString(string key, string defaultValue = "")
        {
#if UNITY_WEBGL
            if (!StorageData.ContainsKey(key))
                return defaultValue;
    
            return Convert.ToString(StorageData[key]);
#else
            return PlayerPrefs.GetString(key, defaultValue);
#endif
        }

        public bool HasKey(string key)
        {
#if UNITY_WEBGL
            return StorageData.ContainsKey(key);
#else
            return PlayerPrefs.HasKey(key);
#endif
        }

        public void Save()
        {
#if UNITY_WEBGL
            //Do nothing
#else
            PlayerPrefs.Save();
#endif
        }

        public void SetFloat(string key, float value)
        {
#if UNITY_WEBGL
            SetStorageValue(key, value);
#else
            PlayerPrefs.SetFloat(key, value);
#endif
        }

        public void SetInt(string key, int value)
        {
#if UNITY_WEBGL
            SetStorageValue(key, value);
#else
            PlayerPrefs.SetInt(key, value);
#endif
        }

        public void SetString(string key, string value)
        {
#if UNITY_WEBGL
            SetStorageValue(key, value);
#else
            PlayerPrefs.SetString(key, value);
#endif
        }

        private void SetStorageValue(string key, object value)
        {
#if UNITY_WEBGL
            if (StorageData.ContainsKey(key))
                StorageData[key] = value;
            else
                StorageData.Add(key, value);
    
            if (OnChangeValue != null)
                OnChangeValue(key, value);
#endif
            //callback
        }
        
        public object GetValue(string key, object defaultValue = null)
        {
            if (!HasKey(key))
                return defaultValue;

            if ((GetString(key, "a") != "a") ||
                (GetString(key, "b") != "b"))
                return GetString(key);

            if ((GetInt(key, 1) != 1) ||
                (GetInt(key, 2) != 2))
                return GetInt(key);

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if ((GetFloat(key, 1f) != 1f) ||
                (GetFloat(key, 2f) != 2f))
                return GetFloat(key);
            // ReSharper restore CompareOfFloatsByEqualityOperator

            throw new Exception("Unknown object type.");
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            return Convert.ToBoolean(GetInt(key, Convert.ToInt32(defaultValue)));
        }

        public void SetBool(string key, bool value)
        {
            SetInt(key, Convert.ToInt32(value));
        }

        public bool SetField(string key, ref bool field, bool value)
        {
            if (value == field)
                return false;

            field = value;
            SetBool(key, field);
            return true;
        }

        public bool SetField(string key, ref int field, int value)
        {
            if (value == field)
                return false;

            field = value;
            SetInt(key, field);
            return true;
        }

        public bool SetField(string key, ref float field, float value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value == field)
                return false;
            // ReSharper restore CompareOfFloatsByEqualityOperator
            
            field = value;
            SetFloat(key, field);
            return true;
        }

        public bool SetField(string key, ref string field, string value)
        {
            if (value == field)
                return false;

            field = value;
            SetString(key, field);
            return true;
        }
    }
}