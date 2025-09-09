using System;
using UnityEngine;
using TankAndHealerStudioAssets;

public class ChatBox : MonoBehaviour
{
    public UltimateChatBox _chatBox;
    public UdonChatUser _chatUser;
    
    
    private string _userName = "User";
    

    private void Awake()
    {
        _chatBox.OnInputFieldSubmitted += OnSubmit;
    }
    
    private void OnDestroy()
    {
        _chatBox.OnInputFieldSubmitted -= OnSubmit;
    }
    
    
    private void OnSubmit(string msg)
    {
        Debug.Log($"OnSubmit msg {msg}");
        ChatManager.Instance.SubmitMessege(_chatBox, _userName, msg);
    }

    public void Test(string msg)
    {
        Debug.Log($"Test msg {msg}");
    }
    
    private void Start()
    {
        if (_chatUser != null)
        {
            _userName = _chatUser.GetUserName();
        }
        
        Debug.Log($"Start _chatBox {_chatBox} _userName {_userName}");
        ChatManager.Instance.RegisterNewChat(_chatBox);
    }


    public AlwaysVisibleOverlay AlwaysVisibleOverlay;
    private void Update()
    {
        //Debug.Log($"КAlwaysVisibleOverlay.CheckPkey() {AlwaysVisibleOverlay.CheckPkey()}");
        /*if (AlwaysVisibleOverlay.CheckPkey())
        {
            AlwaysVisibleOverlay.onPressed();
        }*/
        
    }
}

