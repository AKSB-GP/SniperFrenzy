using UnityEngine;

[CreateAssetMenu(fileName = "ScoreManagerAudioConfig", menuName = "Scriptable Objects/ScoreManagerAudioConfig")]
public class ScoreManagerAudioConfig : ScriptableObject
{
    [Header("Clips")]
    public AudioClip criticalHitSound;
    public float criticalHitVolume = 1f;
    public AudioClip bodyHitSound;
    [Header("Volume")]

    public float bodyHitVolume = 1f;
}
