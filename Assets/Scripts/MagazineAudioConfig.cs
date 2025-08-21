using UnityEngine;

[CreateAssetMenu(fileName = "MagazineAudioConfig", menuName = "Scriptable Objects/MagazineAudioConfig")]
public class MagazineAudioConfig : ScriptableObject
{
    [Header("Clips")]
    public AudioClip magazineRemoved;
    public AudioClip magazineInserted;
    [Header("Volume")]
    public float volume;

}
