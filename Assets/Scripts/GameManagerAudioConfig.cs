using UnityEngine;

[CreateAssetMenu(fileName = "GameManagerAudioConfig", menuName = "Scriptable Objects/GameManagerAudioConfig")]
public class GameManagerAudioConfig : ScriptableObject
{
    [Header("Clips")]
    public AudioClip countDownSoundStep;
    public AudioClip countDownSoundFinal;
    public AudioClip gameOver;
    public AudioClip gameMusic;
    [Header("Volume")]
    public float StepVolume;
    public float FinalVolume;
    public float gameOverVolume;

}
