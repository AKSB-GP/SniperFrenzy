using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField] public Canvas canvas;
    [SerializeField] public TMP_Text CountDownText;
    [SerializeField] public TMP_Text ScoreText;
    [SerializeField] public TMP_Text AmmoText;
    [SerializeField] public TMP_Text LivesText;

    [SerializeField] RifleHandling rifleHandling;
    [SerializeField] ScoreManager scoreManager;
    public RifleCharacteristics rifleCharacteristics;
    public GameManager gameManager;

    LineRenderer lineRenderer;

    void Start()
    {
        GameObject lineObj = new GameObject("LineObj");
        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {

        CountDownText.text = $"{gameManager.countDownTimeRemaining}";
        if(gameManager.countDownTimeRemaining ==0) CountDownText.text = null;
        ScoreText.text = $"SCORE:{scoreManager.playerScore}";
        AmmoText.text = $"AMMO:{rifleHandling.totalAmmoSize}";
        LivesText.text = $"LIVES:{gameManager.lives}";
    }
}