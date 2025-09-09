using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class UdonChatUser : UdonSharpBehaviour
{
    [UdonSynced]
    private string _playerName;
    
    // The player that this script is attached to
    //private VRCPlayerApi _owner;
    private VRCPlayerApi _localPlayer;
    
    private void Awake()
    {
        // Получаем локального игрока
        _localPlayer = Networking.LocalPlayer;
        

        if (_localPlayer != null)
        {
            // Получаем никнейм локального игрока
            _playerName = _localPlayer.displayName;
            Debug.Log("Ваш никнейм: " + _playerName);
        }
        else
        {
            _playerName = "User Unkown";
            Debug.Log(_playerName);
        }
        
    }

    private void Start()
    {
        /*if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            Debug.Log($"IsOwner true {_playerName} ");
            _owner = Networking.LocalPlayer;
            _playerName = _owner.displayName;
            Debug.Log($"_owner name {_playerName} ");
        }*/
    }


    public string GetUserName()
    {
        Debug.Log($"GetUserName {_playerName} ");
        Debug.Log($"Net GetUserName {Networking.LocalPlayer.displayName} ");
        return _playerName;
        //return _playerName;
    }
    
    
}
