using UdonSharp;
using UnityEngine;


public class UdonClicker : UdonSharpBehaviour
{
    [SerializeField] private AudioClip bonk;
    [SerializeField] private float volume = 1f;
    
    
    
    public override void Interact()
    {
        AudioSource.PlayClipAtPoint(bonk, transform.position, volume);
    }
}
