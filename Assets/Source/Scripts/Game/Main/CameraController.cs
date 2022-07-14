using System.Collections;
using Core;
using Core.Attributes;
using Core.Settings;
using UnityEngine;

public class CameraController : DIBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _borderWidth;

    [SerializeField]
    private BackgroundScaler _background;
    
    [Inject]
    private GameSettings _gameSettings;
    
    public void SetupCamera(LevelConfig levelConfig)
    {
        float boardWidth = levelConfig.Dimensions.x * _gameSettings.CellSize.x + _borderWidth;
        float tanFOV = Mathf.Tan(Camera.VerticalToHorizontalFieldOfView(_camera.fieldOfView, _camera.aspect) * Mathf.Deg2Rad);

        var position = _camera.transform.position;
        position.z = -boardWidth / tanFOV;
        _camera.transform.position = position;

        StartCoroutine(UpdateBackground_co());
    }

    private IEnumerator UpdateBackground_co()
    {
        yield return new WaitForEndOfFrame();
        _background.UpdateBackground(_camera);
    }
}