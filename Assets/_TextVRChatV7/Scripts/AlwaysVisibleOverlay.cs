using UdonSharp;
using UnityEngine;
using VRC.SDKBase;


public class AlwaysVisibleOverlay : UdonSharpBehaviour
{
    [SerializeField] private Transform canvasTransform; 
    [SerializeField] private float _headOffset = 0.5f;
    [SerializeField] private float _yOffset = 0f;
    [SerializeField] private float _xOffset = 0f;
    
    private bool _isOverlayActive;
    
    void Update()
    {
        if (Networking.LocalPlayer == null) return;

        // Получаем позицию и ориентацию головы игрока
        Vector3 headPosition = Networking.LocalPlayer.GetTrackingData
            (VRCPlayerApi.TrackingDataType.Head).position;
        Quaternion headRotation = Networking.LocalPlayer.GetTrackingData
            (VRCPlayerApi.TrackingDataType.Head).rotation;

        // Устанавливаем позицию Canvas перед игроком
        /*canvasTransform.position = headPosition + headRotation * 
            Vector3.forward * _headOffset;*/
        canvasTransform.position = headPosition + headRotation *
            new Vector3(_xOffset / 100, _yOffset / 100, 1f * _headOffset);
        
        canvasTransform.rotation = Quaternion.LookRotation
            (headRotation * Vector3.forward, Vector3.up);
    }
}
