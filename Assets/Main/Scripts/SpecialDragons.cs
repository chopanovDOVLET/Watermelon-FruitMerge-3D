using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class SpecialDragons : MonoBehaviour
{
    public static SpecialDragons Instance;

    [Header("Bomb Fruit")] 
    [SerializeField] private ButtonController bomb;
    [SerializeField] private Animation BombAnim;
    [SerializeField] private TextMeshProUGUI bombText;
    [SerializeField] private GameObject bombAdsIcon;
    [SerializeField] private GameObject bombLock;
    [SerializeField] private GameObject bombTutHand;
    [SerializeField] private GameObject bombIcon;
    public int bombSize;

    [Header("Unicorn Fruit")]
    [SerializeField] private ButtonController uni;
    [SerializeField] private Animation UnicornAnim;
    [SerializeField] private TextMeshProUGUI unicornText;
    [SerializeField] private GameObject unicornAdsIcon;
    [SerializeField] private GameObject unicornLock;
    [SerializeField] private GameObject unicornTutHand;
    [SerializeField] private GameObject unicornIcon;
    public int unicornSize;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        bombSize = PlayerPrefs.GetInt("Bomb", 0);
        unicornSize = PlayerPrefs.GetInt("Unicorn", 0);

        Refresh();
        
        if (PlayerPrefs.GetInt("UnLockBomb", 0) == 2)
        {
            bombLock.SetActive(false);
            bombIcon.SetActive(true);
        }


        if (PlayerPrefs.GetInt("UnLockUnicorn", 0) == 2)
        {
            unicornLock.SetActive(false);
            unicornIcon.SetActive(true);
        }

        StartCoroutine(Anim());
    }

    
    IEnumerator Anim()
    {
        yield return new WaitForSeconds(15f);

        if (bombSize > 0)
            BombAnim.Play();
        if (unicornSize > 0)
            UnicornAnim.Play();

        StartCoroutine(Anim());
    }
    public void Refresh()
    {
        if (bombSize == 0)
        {
            if (PlayerPrefs.GetInt("UnLockBomb", 0) == 2)
                bomb.adState = ButtonController.AdState.ad;
            bombAdsIcon.SetActive(true);
        }
        else
        {
            bomb.adState = ButtonController.AdState.none;
            bombAdsIcon.SetActive(false);
            bombText.text = "" + bombSize;
        }


        if (unicornSize == 0)
        {
            if (PlayerPrefs.GetInt("UnLockUnicorn", 0) == 2)
                uni.adState = ButtonController.AdState.ad;
            unicornAdsIcon.SetActive(true);
        }
        else
        {
            uni.adState = ButtonController.AdState.none;
            unicornAdsIcon.SetActive(false);
            unicornText.text = "" + unicornSize;
        }
    }
    
    public IEnumerator UnLock()
    {
        if (bombSize > 0 && PlayerPrefs.GetInt("UnLockBomb", 0) == 1)
        {
            bombLock.SetActive(false);
            bombIcon.SetActive(true);
            bombTutHand.SetActive(true);
            PlayerPrefs.SetInt("UnLockBomb", 2);
            yield return new WaitForSeconds(.3f);
            TouchController.Instance.TakeControl = true;
        }


        if (unicornSize > 0 && PlayerPrefs.GetInt("UnLockUnicorn", 0) == 1)
        {
            unicornLock.SetActive(false);
            unicornIcon.SetActive(true);
            unicornTutHand.SetActive(true);
            PlayerPrefs.SetInt("UnLockUnicorn", 2);
            yield return new WaitForSeconds(.3f);
            TouchController.Instance.TakeControl = true;
        }
    }

    public void CloseTutorial()
    {
        bombTutHand.SetActive(false);
        unicornTutHand.SetActive(false);
    }
}