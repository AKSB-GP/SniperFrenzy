using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int playerScore;

    public int headShotScore;
    public int bodyScore;
    [SerializeField]
    private GameObject scorePopupPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerScore = 0;
    }

    private void OnEnable()
    {
        ProjectileBehaviour.OnEnemyHit += HandleEnemyHit;
    }

    private void OnDisable()
    {
        ProjectileBehaviour.OnEnemyHit -= HandleEnemyHit;
    }

    private void HandleEnemyHit(Collider hitCollider)
    {
        int scoreToAdd = hitCollider.CompareTag("EnemyCritical") ? headShotScore : bodyScore;
        playerScore += scoreToAdd;

        var enemyRoot = hitCollider.transform.root.gameObject;

        Destroy(enemyRoot);
    }

 
}



