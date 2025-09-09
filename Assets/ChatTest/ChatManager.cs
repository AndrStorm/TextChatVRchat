using System.Collections.Generic;
using TankAndHealerStudioAssets;

public class ChatManager : Singleton<ChatManager>
{
    public UltimateChatBox _worldChatBox;
    
    private List<UltimateChatBox> _chatBoxes;

    protected override void Awake()
    {
        base.Awake();
        
        _chatBoxes = new List<UltimateChatBox>();

        if (_worldChatBox != null)
        {
            _chatBoxes.Add(_worldChatBox);
        }
    }
    
    public void RegisterNewChat(UltimateChatBox senderChatBox)
    {
        _chatBoxes.Add(senderChatBox);
    }
        

    public void SubmitMessege(UltimateChatBox senderChatBox, string userName, string msg)
    {
        foreach (var chatBox in _chatBoxes)
        {
            if (chatBox == senderChatBox)
            {
                chatBox.RegisterChat(userName, msg, UltimateChatBoxStyles.blueUsername);
            }
            else
            {
                chatBox.RegisterChat(userName, msg);
            }
        }
    }
    
}
