using UdonSharp;
using UnityEngine;
using VRC.SDKBase;


public class ActiveInVr : UdonSharpBehaviour
{
    public GameObject _GameObject;
    public bool setStateInVR;
    
    void Start()
    {
        if (Networking.LocalPlayer.IsUserInVR())
        {
            _GameObject.SetActive(setStateInVR);
        }
        else
        {
            _GameObject.SetActive(!setStateInVR);
        }
    }
}
