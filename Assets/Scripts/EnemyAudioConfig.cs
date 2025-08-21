using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAudioConfig", menuName = "Scriptable Objects/EnemyAudioConfig")]
public class EnemyAudioConfig : ScriptableObject
{
    [Header("Clips")]
    public AudioClip enemySpawn;
    public AudioClip enemyDespawn;
    [Header("Volume")]

    public float spawnVolume;
    public float DespawnVolume;

}
