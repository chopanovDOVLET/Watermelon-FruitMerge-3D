using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Drawing;
using Image = UnityEngine.UI.Image;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] GameObject StartPanel;
    [SerializeField] Transform LosePanel;
    [SerializeField] Transform YouLoseT;
    [SerializeField] Transform TruAgainB;
    [SerializeField] Transform CryingDr;
    [SerializeField] AudioSource LoseSound;

    [Header("New Best Score UI")]
    [SerializeField] Sprite backgroundBestScore;
    [SerializeField] Sprite bestScoreTextSprite;
    [SerializeField] TextMeshProUGUI bestScoreText;
    [SerializeField] Transform happyDr;
    private bool isNewBestScore;

    [Header("Tutorials")] 
    public GameObject swipeHand;

    [Header("ScreenResolutionUI")] 
    [SerializeField] private Transform landscapePar;
    [SerializeField] private Transform drLvlInd;
    [SerializeField] private Transform nextDr;
    [SerializeField] private Transform score;
    [SerializeField] private Transform best;
    [SerializeField] private Transform continueBtn;
    [SerializeField] private Transform tryAgainBtn;

    private bool rotate;
    private Transform Dr;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // int width = Screen.width;
        // int height = Screen.height;
        // Debug.Log(width + " " + height);
        
        SetScreenResolution();
        StartPanel.SetActive(true);
        YouLoseT.transform.localScale = Vector3.zero;
    }
    private void Update()
    {
        if (rotate)
        {
            Dr.Rotate(Vector3.forward * Time.unscaledDeltaTime * 20);
        }
    }

    public void Bomb()
    {
        if (SpecialDragons.Instance.bombSize > 0)
            MoveController.Instance.TakeBomb();
    }

    public void Unicorn()
    {
        if (SpecialDragons.Instance.unicornSize > 0)
            MoveController.Instance.TakeUnicorn();
    }

    public void TapToStart()
    {
        GameManager.Instance.StartGamePlayEventCrazy();
        ScrollController.Instance.CanCheck = true;
        StartPanel.SetActive(false);
    }

    public IEnumerator EnableLosePanel()
    {
        StartPanel.SetActive(false);
        // YsoCorp.GameUtils.YCManager.instance.OnGameFinished(false);
        
        Time.timeScale = 0;
        ScrollController.Instance.CanCheck = false;



        LosePanel.gameObject.SetActive(true);
        Tweener tweener = LosePanel.GetComponent<Image>().DOFade(0.97f, 0.75f);
        tweener.SetUpdate(true);
        yield return new WaitForSecondsRealtime(.75f);
        
        if (ScoreCounter.Instance.highscore == ScoreCounter.Instance.score && 
            PlayerPrefs.GetInt("HighScore") != ScoreCounter.Instance.highscore)
        {
            //LosePanel.GetComponent<Image>().sprite = backgroundBestScore;
            YouLoseT.GetChild(5).gameObject.SetActive(true);
            YouLoseT.GetChild(4).GetComponent<TextMeshProUGUI>().fontSize = 120;
            YouLoseT.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"Score";
            YouLoseT.GetChild(3).gameObject.SetActive(true);
            YouLoseT.GetChild(4).GetComponent<TextMeshProUGUI>().fontSize = 240;
            YouLoseT.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"{ScoreCounter.Instance.highscore}";
            YouLoseT.GetChild(4).gameObject.SetActive(true);
            //CryingDr = happyDr;
            LoseSound = DragonIndicator.Instance.NewDragonSound;
            isNewBestScore = true;
        } else
        {
            YouLoseT.GetChild(0).gameObject.SetActive(true);
            YouLoseT.GetChild(1).gameObject.SetActive(true);
            YouLoseT.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{ScoreCounter.Instance.score}";
            YouLoseT.GetChild(2).gameObject.SetActive(true);
            YouLoseT.GetChild(3).gameObject.SetActive(true);
            YouLoseT.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"{ScoreCounter.Instance.highscore}";
            YouLoseT.GetChild(4).gameObject.SetActive(true);

            LosePanel.GetChild(2).gameObject.SetActive(true);
        }
        //yield return new WaitForSecondsRealtime(.75f);
        if (isNewBestScore)
        {
            happyDr.gameObject.SetActive(true);
            Dr = happyDr;
            LosePanel.GetChild(3).gameObject.SetActive(true);
            LosePanel.GetChild(4).gameObject.SetActive(true);
            isNewBestScore = false;
        }
        else
        {
            CryingDr.gameObject.SetActive(true);
            Dr = CryingDr;
        }
        Tweener tweener4 = LosePanel.GetChild(0).DOScale(1, .5f);
        tweener4.SetUpdate(true);
        Vector3 pos = Camera.main.ScreenToWorldPoint(LosePanel.localPosition);
        pos = pos + (LosePanel.position - Camera.main.transform.position) / 1.5f;
        //yield return new WaitForSecondsRealtime(.3f);
        Dr.position = pos;
        Dr.rotation = Quaternion.Euler(300, 0, 90);
        //Dr.parent = LosePanel;
        //Dr.SetAsLastSibling();
        //Transform newDr = Instantiate(CryingDr, pos, Quaternion.Euler(300, 0, 90),LosePanel);
        Vector3 drScale = Dr.localScale * 160;
        Dr.localScale = Vector3.zero;
        //Dr = newDr;
        Tweener tweener2 = Dr.DOScale(drScale, .75f);
        tweener2.SetUpdate(true);
        rotate = true;
        AudioSource sd = Instantiate(LoseSound, Dr.position, Dr.rotation);
        Destroy(sd.gameObject, 3f);
        sd.Play();
        yield return new WaitForSecondsRealtime(1.5f);
        Tweener tweener3 = LosePanel.GetChild(1).DOScale(Vector3.one, 0.75f);
        tweener3.SetUpdate(true);
    }

    private void SetScreenResolution()
    {
        int width = Screen.width;
        int height = Screen.height;

        Vector3 drLvlIndicator = new Vector3(0, 0, 0);
        Vector3 nextDr = new Vector3(1050, 0, 0);
        Vector3 score = new Vector3(-825, 0, 0);
        Vector3 best = new Vector3(0, -260, 0);
        Vector3 bestScale = new Vector3(1.3f, 1.3f, 1.3f);

        if (width > height || width >= 789)
        {
            drLvlInd.SetParent(landscapePar);
            this.nextDr.SetParent(landscapePar);
            this.score.SetParent(landscapePar);
            this.best.SetParent(landscapePar);
            
            drLvlInd.localPosition = drLvlIndicator;
            this.nextDr.localPosition = nextDr;
            this.score.localPosition = score;
            this.best.localScale = bestScale;
            this.best.localPosition = best;
        }
    }
}