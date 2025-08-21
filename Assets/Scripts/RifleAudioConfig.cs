using UnityEngine;

[CreateAssetMenu(fileName = "RifleAudioConfig", menuName = "Scriptable Objects/RifleAudioConfig")]
public class RifleAudioConfig : ScriptableObject
{
    [Header("Clips")]
    public AudioClip rifleFire;
    public AudioClip boltLock;
    public AudioClip boltOpen;
    public AudioClip magazineRemoved;
    public AudioClip magazineInserted;
    [Header("Volume")]

    public float volume;

}
