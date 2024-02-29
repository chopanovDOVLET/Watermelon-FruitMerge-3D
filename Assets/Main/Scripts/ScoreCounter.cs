using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UIElements;
using System.Collections;
public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance;
    [SerializeField] private TextMeshProUGUI HighScore;
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private Transform unlockNewHighScore;
    private bool isNewHighScore;
    public int score;
    public int highscore;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        isNewHighScore = true;
        score = PlayerPrefs.GetInt("Score");
        highscore = PlayerPrefs.GetInt("HighScore");

        HighScore.text = $"{highscore}";
        currentScore.text = $"{score}";
    }

    public void SetScore(int s)
    {
        score += s;
        if (highscore < score)
        {
            highscore = score;   
        }
        Refresh();
    }

    void Refresh()
    {
        HighScore.text = $"{highscore}";
        currentScore.text = $"{score}";
    }
 }