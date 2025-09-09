
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleChat : UdonSharpBehaviour
{
    [SerializeField] private GameObject _overlay;
    [SerializeField] private GameObject _chatBox;

    [SerializeField] private SimpleChatBox _simpleChatBox;
    
    private bool _isOverlayActive;

    private void Update()
    {
        bool isActiveChat = _simpleChatBox.IsInputActive();
        
        if (!isActiveChat && Input.GetKeyDown(KeyCode.T))
        {
            SwitchOverlay();
        }
    }
    
    public void SwitchOverlay()
    {
        //Debug.Log($"isActive {_isOverlayActive}");
        _overlay.SetActive(_isOverlayActive);
        _chatBox.SetActive(_isOverlayActive);
        _isOverlayActive = !_isOverlayActive;
    }
}
