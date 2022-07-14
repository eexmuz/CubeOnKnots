using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Settings
{
    public class UiSystemSettingsAsset : ScriptableObject
    {
        #region Fields

        public UiSystemSettings data;

        #endregion
    }

    [Serializable]
    public class UiSystemSettings : ISettings
    {
        #region Fields

        public GameObject shroudPrefab;

        public GameObject uiLockerPrefab;
        private IDictionary<ViewName, GameObject> _prefabByName;

        [SerializeField]
        private ViewName[] names;

        [SerializeField]
        private GameObject[] prefabs;

        #endregion

        #region Public Methods and Operators

        public GameObject GetDialogPrefabForName(ViewName name)
        {
            if (_prefabByName == null)
            {
                // construct prefabs map
                _prefabByName = new Dictionary<ViewName, GameObject>();
                if (names != null)
                    for (var i = 0; i < names.Length; i++)
                        _prefabByName[names[i]] = prefabs[i];
            }

            GameObject res;
            _prefabByName.TryGetValue(name, out res);
            return res;
        }

        #endregion
    }
}