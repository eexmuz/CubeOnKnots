using UnityEngine;

namespace Utility
{
    [ExecuteInEditMode]
    public class ObjectsToggle : MonoBehaviour
    {
        #region Fields

        public GameObject[] ToggleObjects;

        #endregion

        #region Methods

        private void OnDisable()
        {
            if (ToggleObjects != null)
                foreach (var obj in ToggleObjects)
                    if (obj != null)
                        obj.SetActive(true);
        }

        private void OnEnable()
        {
            if (ToggleObjects != null)
                foreach (var obj in ToggleObjects)
                    if (obj != null)
                        obj.SetActive(false);
        }

        #endregion
    }
}