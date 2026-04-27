using UdonSharp;
using UnityEngine;


public class ToggleChat : UdonSharpBehaviour
{
    [Header("References to toggle")]
    [SerializeField] private GameObject _overlay;
    [SerializeField] private GameObject _chatBox;
    
    [Header("Chat box controller")]
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
    
    public void SwitchOverlayEvent()
    {
        SwitchOverlay();
    }
    
    private void SwitchOverlay()
    {
        //Debug.Log($"isActive {_isOverlayActive}");
        _overlay.SetActive(_isOverlayActive);
        _chatBox.SetActive(_isOverlayActive);
        _isOverlayActive = !_isOverlayActive;
    }
}
