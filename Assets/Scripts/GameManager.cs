using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public CapsuleCollider enemyGoal;
    public int lives;
    public GameManagerAudioConfig gameManagerAudioConfig;
    private AudioSource audioSource;

    public int countDownTimeRemaining = 5;
    public bool countDownIsRunning = false;
    public string timeText;
    public float countDownStepInterval = 1f;
    private float countDownStepTimer = 0f;
    public static event Action OnGameStart;
    private bool hasAnnouncedGameStart = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (enemyGoal == null)
        {
            Debug.LogWarning("GameManager: enemyGoal is not assigned.");
            return;
        }
        enemyGoal.isTrigger = true;
        countDownIsRunning = true;
    }

    private void Update()
    {
        gameCountDown();

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyCritical"))
        {

            if (lives == 1)
            {
                Time.timeScale = 0f;
                audioSource.PlayOneShot(gameManagerAudioConfig.gameOver);
                GameOver();
            }
            else
            {
                removeALife();
            }
        }
    }
    public void StartGameMusic()
    {
        audioSource.clip = gameManagerAudioConfig.gameMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void gameCountDown()
    {
        if (countDownIsRunning)
        {
            //if not at final step
            countDownStepTimer += Time.deltaTime;
            if (countDownTimeRemaining > 0 && countDownStepTimer >= countDownStepInterval)
            {
                countDownTimeRemaining -= 1;
                countDownStepTimer = 0f;
                //audio
                if (gameManagerAudioConfig.countDownSoundStep != null && audioSource != null)
                    audioSource.PlayOneShot(gameManagerAudioConfig.countDownSoundStep);

                DisplayTime(countDownTimeRemaining);
            }
            //at final step 
            else if (countDownTimeRemaining == 0 && countDownIsRunning)
            {
                audioSource.PlayOneShot(gameManagerAudioConfig.countDownSoundFinal);
                countDownIsRunning = false;
                countDownTimeRemaining = 0;
                if (!hasAnnouncedGameStart)
                {
                    hasAnnouncedGameStart = true;
                    OnGameStart?.Invoke();
                }
                StartGameMusic();
            }
        }
    }

    private void DisplayTime(float timeToDisplay)
    {

        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText = string.Format("{0}", seconds);
    }

    private void removeALife()
    {
        if (lives != 0)
        {
            lives -= 1;
        }
    }

    private void GameOver()
    {
        // Restart current scene immediately
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}



