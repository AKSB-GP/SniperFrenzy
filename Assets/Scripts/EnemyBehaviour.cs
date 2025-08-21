using System;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    private Vector3 startingposition;
    private Quaternion startingRotation;
    [SerializeField] private Collider goalPosition;
    public Vector3 directionToGoal;
    [SerializeField] private float movementSpeed;
    public static event Action<GameObject> OnEnemyReachedGoal;
    public EnemyAudioConfig audioConfig;
    public AudioSource audioSource;

    void Start()
    {
        startingposition = gameObject.transform.position;
        directionToGoal = (goalPosition.transform.position - startingposition).normalized;
        gameObject.GetComponent<Animator>().Play("EnemyMovement");
        audioSource = GetComponent<AudioSource>();
        PlaySpawnAudio();
        // Make X-axis point of mesh toward goal position
        startingRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up) * Quaternion.FromToRotation(Vector3.right, directionToGoal);
        transform.rotation = startingRotation;
        //to straighten out the mesh to be oriented straight
        Vector3 euler = startingRotation.eulerAngles;
        euler.x = 0;
        transform.rotation = Quaternion.Euler(euler);

    }
    void OnEnable()
    {
        PlaySpawnAudio();
    }

    void PlaySpawnAudio()
    {
        if (audioSource != null && audioConfig != null && audioConfig.enemySpawn != null)
            audioSource.PlayOneShot(audioConfig.enemySpawn, audioConfig.spawnVolume);
    }
    void PlayDepawnAudio()
    {
        if (audioConfig != null && audioConfig.enemyDespawn != null)
            AudioSource.PlayClipAtPoint(audioConfig.enemyDespawn, transform.position, audioConfig.DespawnVolume);
    }
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += directionToGoal * movementSpeed * Time.deltaTime;

    }

    void OnTriggerEnter(Collider other)
    {
        PlayDepawnAudio();
        if (other.CompareTag("EnemyGoal"))
        {
            OnEnemyReachedGoal?.Invoke(gameObject);
        }
    }

}
