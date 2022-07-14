using UnityEngine;

public class UILocker : MonoBehaviour
{
    #region Fields

    private bool _isDestroyed;

    #endregion

    #region Public Methods and Operators

    public void destroy()
    {
        if (!_isDestroyed)
        {
            _isDestroyed = true;
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Trying to destroy already destroyed object");
        }
    }

    #endregion
}