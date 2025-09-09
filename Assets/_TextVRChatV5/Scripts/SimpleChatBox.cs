
using System;
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
    
    public InputField _vrinputField;
    public Scrollbar _vrscrollbar;
    public RectTransform _vrmessegeList;
    public GameObject _vrtextObject;
    
    public InputField _otherinputField;
    public Scrollbar _otherscrollbar;
    public RectTransform _othermessegeList;
    public GameObject _othertextObject;
    
    private InputField _inputField;
    private Scrollbar _scrollbar;
    private RectTransform _messegeList;
    private GameObject _textObject;

    public bool _isTestVr;
    public bool _isEnterToChat;
    
    private bool _isInputActiveByEnter;
    private bool _isNeedToResetScrollBar;
    private bool _isNeedToWaitFrame;
    private bool _isInputActive;

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
            _inputField = _vrinputField;
            _scrollbar = _vrscrollbar;
            _messegeList = _vrmessegeList;
            _textObject = _vrtextObject;
        }
        else
        {
            _inputField = _otherinputField;
            _scrollbar = _otherscrollbar;
            _messegeList = _othermessegeList;
            _textObject = _othertextObject;
        }
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

    private void OnEnable()
    {
        ResetScrollBar();
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
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(OnPlayerSendMessege), msg);
        
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
    public void OnPlayerSendMessege(string msg)
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
        
        
        var textObject = Instantiate(_textObject, _messegeList);
        
        
        var msgTextField = textObject.GetComponent<TMP_Text>();
        msgTextField.text = msg;
        
        
        float minHeight = CalculateLayoutMinHeight(msgTextField, simpleMsg);
        textObject.GetComponent<LayoutElement>().minHeight = minHeight;
        textObject.GetComponent<LayoutElement>().minWidth = _messegeList.rect.width;
        
        ResetScrollBar();
    }

    
    private const char SPACE = ' ';
    private float CalculateLayoutMinHeight(TMP_Text msgTextField, string simpleMsg)
    {
        float fontSize = msgTextField.fontSize;

        float lineWidth = _messegeList.rect.width;
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
