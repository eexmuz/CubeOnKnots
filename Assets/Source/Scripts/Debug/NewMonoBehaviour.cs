using UnityEngine;

public class NewMonoBehaviour : MonoBehaviour
{
    #region Constants

    private const string ValidationMethodName = "Validate";

    #endregion

    #region Fields

    private State _state;

    #endregion

    #region Enums

    private enum State : byte
    {
        Inactive,
        Active,
        WaitValidation
    }

    #endregion

    #region Public Methods and Operators

    public void Invalidate()
    {
        if (_state == State.Inactive)
            return;

        _state = State.WaitValidation;
        Invoke(ValidationMethodName, 0f);
    }

    public void Validate()
    {
        if (_state == State.Inactive)
            return;

        if (_state == State.WaitValidation)
            CancelInvoke(ValidationMethodName);

        _state = State.Active;

        _Draw();
    }

    #endregion

    #region Methods

    protected virtual void _Draw()
    {
    }

    protected virtual void _OnDestroy()
    {
    }

    protected virtual void _Start()
    {
    }

    protected void OnDestroy()
    {
        _OnDestroy();

        if (_state == State.WaitValidation)
            CancelInvoke(ValidationMethodName);

        _state = State.Inactive;
    }

    protected void Start()
    {
        _Start();

        _state = State.Active;
        Invalidate();
    }

    #endregion
}