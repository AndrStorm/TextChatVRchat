using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.UdonNetworkCalling;
using VRC.Udon.Common.Interfaces;  
using VRC.SDKBase;


[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class SimpleChatBox : UdonSharpBehaviour
{
    
    [Header("VR platform chat References")]
    public InputField _vrInputField;
    public Scrollbar _vrScrollbar;
    public RectTransform _vrMessageList;
    public GameObject _vrTextObject;
    public LayoutGroupHelper _vrGroupHelper;
    
    [Header("Other platforms chat References")]
    public InputField _otherInputField;
    public Scrollbar _otherScrollbar;
    public RectTransform _otherMessageList;
    public GameObject _otherTextObject;
    public LayoutGroupHelper _otherGroupHelper;
    
    [Header("Testing options")]
    public bool _isTestVr;
    public bool _isEnterToChat;
    
    
    private InputField _inputField;
    private Scrollbar _scrollbar;
    private RectTransform _messageList;
    private GameObject _textObject;
    private LayoutGroupHelper _groupHelper;
    
    
    private bool _isInputActiveByEnter;
    private bool _isNeedToResetScrollBar;
    private bool _isNeedToWaitFrame;
    private bool _isInputActive;

    
    private const char SPACE = ' ';
    
    
    
    public bool IsInputActiveByEnter()
    {
        return _isInputActiveByEnter;
    }
    
    public bool IsInputActive()
    {
        return _isInputActive;
    }


    private void Start()
    {
        if (Networking.LocalPlayer.IsUserInVR() || _isTestVr)
        {
            _inputField = _vrInputField;
            _scrollbar = _vrScrollbar;
            _messageList = _vrMessageList;
            _textObject = _vrTextObject;
            _groupHelper = _vrGroupHelper;
        }
        else
        {
            _inputField = _otherInputField;
            _scrollbar = _otherScrollbar;
            _messageList = _otherMessageList;
            _textObject = _otherTextObject;
            _groupHelper = _otherGroupHelper;
        }
    }
    
    private void OnEnable()
    {
        ResetScrollBar();
    }

    private void Update()
    {
        if (_isEnterToChat && Input.GetKeyDown(KeyCode.Return))
        {
            _isInputActive = false;
            
            if (!_isInputActiveByEnter)
            {
                //Debug.Log("Enter ActivateInputField");
                _isInputActiveByEnter = true;
                //_inputField.interactable = true;
                _inputField.ActivateInputField();
            }
            else
            {
                //Debug.Log("Enter DeactivateIputDield");
                _isInputActiveByEnter = false;
                //_inputField.interactable = false;
                _inputField.DeactivateInputField(); 
            }
        }

        if (_isNeedToResetScrollBar)
        {
            if (_isNeedToWaitFrame)
            {
                _isNeedToWaitFrame = false;
                return;
            }
            
            _isNeedToResetScrollBar = false;
            _scrollbar.value = 0f;
        }
    }

    
    
    
    
    
    public void OnInputChanged()
    {
        //Debug.Log($"OnInputChanged()");

        if (_isEnterToChat)
        {
            _isInputActive = true;
        }
    }

    public void OnEndEdit()
    {
        //Debug.Log($"OnEndEdit()");
        //_isInputActive = false;
        
        string msg = _inputField.text;
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(OnPlayerSendMessage), msg);
        
        _inputField.text = string.Empty;
        
        _isInputActiveByEnter = true;
        //_inputField.interactable = false;
        _inputField.DeactivateInputField(); 
    }
    
    
    public void OnInputFieldPressed()
    {
        //Debug.Log("OnInputFieldPressed");
        _inputField.interactable = true;
        _inputField.ActivateInputField();
    }
    
    
    
    
    /*TMP_inputField
    public TMP_InputField _inputField;
    public void OnInputFieldPressed()
    {
        Debug.Log("OnInputFieldPressed");
        //_inputField.interactable = true;
        //_inputField.ActivateInputField();
        
    }
    
    public void OnValueChanged()
    {
        Debug.Log("OnValueChanged");
        //_inputField.DeactivateInputField();
        //_inputField.interactable = false;
        OnEndEdit();
    }*/
    
    [NetworkCallable]  
    public void OnPlayerSendMessage(string msg)
    {
        string senderPlayerName = NetworkCalling.CallingPlayer.displayName;
        if (senderPlayerName == null)
        {
            senderPlayerName = string.Empty;
        }

        string localPlayerName = Networking.LocalPlayer.displayName;
        if (localPlayerName == null)
        {
            localPlayerName = string.Empty;
        }
        
        string simpleMsg = $"{senderPlayerName}: {msg}";

        if (string.CompareOrdinal(senderPlayerName, localPlayerName) == 0)
        {
            //Debug.Log($"OnPlayerSendMessege()");
            //_isInputActive = false;
            msg = $"<b>{senderPlayerName}</b>: {msg}";
        }
        else
        { msg = simpleMsg; }
        
        
        var textObject = Instantiate(_textObject, _messageList);
        
        
        var msgTextField = textObject.GetComponent<TMP_Text>();
        msgTextField.text = msg;
        
        
        float minHeight = CalculateLayoutMinHeight(msgTextField, simpleMsg);
        textObject.GetComponent<LayoutElement>().minHeight = minHeight;
        textObject.GetComponent<LayoutElement>().minWidth = _messageList.rect.width;
        
        ResetScrollBar();
        _groupHelper.SetUpVerticalLayoutGroup();
    }

    
    private float CalculateLayoutMinHeight(TMP_Text msgTextField, string simpleMsg)
    {
        float fontSize = msgTextField.fontSize;

        float lineWidth = _messageList.rect.width;
        float charWidth = fontSize / 2f;

        int charsInLine = Mathf.FloorToInt(lineWidth / charWidth);

        int charCount = 0;
        int charsBeforeSpace = 0;
        int extraCharsInLine = 0;

        //string log = "line log - ";
        foreach (var symb in simpleMsg)
        {
            charCount++;

            //log += $"№{charCount} {symb}, ";
            bool isLastChar = charCount == simpleMsg.Length;
            bool isSpaceChar = symb.CompareTo(SPACE) == 0;

            if (isLastChar)
            {
                if (charCount > charsInLine)
                {
                    extraCharsInLine += charsInLine - charsBeforeSpace;
                    //log += $"NEW LiINE {charCount} >= {charsInLine} ex {extraCharsInLine} lc {linesCount2}. ";
                    charCount = 0;
                }
            }
            
            if (isSpaceChar)
            {
                if (charCount >= charsInLine)
                {
                    extraCharsInLine += charsInLine - charsBeforeSpace;
                    //log += $"NEW LiINE {charCount} >= {charsInLine} ex {extraCharsInLine} lc {linesCount2}. ";
                    charCount -= charsBeforeSpace;
                }
                else
                {
                    charsBeforeSpace = charCount;
                    //log += $"CharsBeforeSpace {charsBeforeSpace}. ";
                }
                
            }
        }
        //Debug.Log($"linesCount2 {linesCount2} extraCharsInLine {extraCharsInLine}");
        //Debug.Log(log);
        
        
        int charsInMsg = simpleMsg.Length;
        charsInMsg += extraCharsInLine;

        float linesNumber = charsInMsg / (float)charsInLine;
        int linesCount = Mathf.CeilToInt(linesNumber);
        
        float minHeight = fontSize * 1.125f;
        minHeight *= linesCount;
        
        // Debug.Log($"lineWidth {lineWidth} fontSize {fontSize}"
        //           + $"charsInLine {charsInLine} charsInMsg {charsInMsg} "
        //           + $"linesNumber {linesNumber} linesCount {linesCount} "
        //           + $"minHeight {minHeight} charWidth {charWidth}"
        //           );
        
        return minHeight;
    }
    
    private void ResetScrollBar()
    {
        _isNeedToResetScrollBar = true;
        _isNeedToWaitFrame = true;
    }

}