
using System;
using UdonSharp;
using UnityEngine.UI;


public class ResetScrollBar : UdonSharpBehaviour
{
    
    public Scrollbar _scrollbar;

    private bool _isNeedToResetScrollBar;
    private bool _isNeedToWaitFrame;
    private void OnEnable()
    {
        Reset();
    }
    
    private void Update()
    {
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
    
    private void Reset()
    {
        _isNeedToResetScrollBar = true;
        _isNeedToWaitFrame = true;
    }
}
