using DG.Tweening;

using UnityEngine;

public class LoadingWheel : MonoBehaviour
{
    [SerializeField]
    private float _rotationTime = 1f;

    [SerializeField]
    private Transform _wheel;

    private void Start()
    {
        _wheel.DORotate(new Vector3(0f, 0f, -360f), _rotationTime, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental)
              .SetEase(Ease.Linear);
    }
}
