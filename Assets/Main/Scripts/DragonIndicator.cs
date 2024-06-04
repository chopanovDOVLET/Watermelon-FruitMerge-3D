using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class DragonIndicator : MonoBehaviour
{
    public static DragonIndicator Instance;

    [SerializeField] Transform handle;
    public List<DragonIconController> dr;
    [SerializeField] Transform unlockNewDr;
    [SerializeField] Transform Dr;
    [SerializeField] Transform Shine;
    public AudioSource NewDragonSound;
    [SerializeField] TextMeshProUGUI RewardNumber;
    [SerializeField] Transform BombIcon;
    [SerializeField] Transform UnicornIcon;
    [SerializeField] Transform RewardText;
    public bool isOn;
    private int nextDr, rewardNm;
    public float bigX, littleX;
    private bool Rotate;
    public bool Sp, uni, bomb;

    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
        nextDr = MoveController.Instance.currentdr;
        int a = PlayerPrefs.GetInt("Sp");
        if (a == 0)
            Sp = false;
        else
            Sp = true;

        Initialize();
        Init();
    }

    private void Update()
    {
        //
        if (Rotate)
            Dr.Rotate(Vector3.forward * Time.unscaledDeltaTime * 20);
    }

    public IEnumerator Next(int drIndex)
    {
        Init();
        nextDr = drIndex;
        //dr[nextDr].transform.GetChild(1).localScale = Vector3.zero;
        //dr[nextDr].transform.GetChild(1).gameObject.SetActive(true);
        //Tweener t = handle.DOLocalMoveX(bigX + 215, 0.5f);
        //t.SetUpdate(true);
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 0;
        littleX = bigX;// handle.localPosition.x - 215;
        //Tweener t2 = dr[nextDr].transform.GetChild(1).DOScale(Vector3.one, 0.5f);
        //t2.SetUpdate(true);
        Tweener t3 = handle.DOLocalMoveX(littleX, 1f);
        t3.SetUpdate(true);
        yield return new WaitForSecondsRealtime(1f);
        Tweener t2 = dr[nextDr].transform.GetChild(2).DOScale(Vector3.one, 0.5f);
        t2.SetUpdate(true);
        isOn = false;
        TouchController.Instance.TakeControl = true;
        yield return new WaitForSecondsRealtime(.5f);
        StartCoroutine(NewDr());
    }

    private IEnumerator NewDr()
    {
        var newFruitName = MoveController.Instance.dragonData[nextDr].nameDr;
        var wasThisUnlockedBefore = PlayerPrefs.GetInt(newFruitName, 0) == 1;

        if (!wasThisUnlockedBefore)
        {
            PlayerPrefs.SetInt(newFruitName, 1);
            GameManager.Instance.StopGamePlayEventCrazy();
            // YsoCorp.GameUtils.YCManager.instance.OnGameStarted(nextDr);
            // YsoCorp.GameUtils.YCManager.instance.OnGameFinished(true);


            unlockNewDr.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = newFruitName;
            unlockNewDr.gameObject.SetActive(true);
            Image im = unlockNewDr.GetComponent<Image>();
            Tweener t = im.DOFade(0.97f, 0.4f);
            t.SetUpdate(true);
            Vector3 pos = Camera.main.ScreenToWorldPoint(unlockNewDr.localPosition);
            pos = pos + (unlockNewDr.position - Camera.main.transform.position) / 1.5f;
            yield return new WaitForSecondsRealtime(.5f);
            Tweener t1 = unlockNewDr.GetChild(1).DOScale(Vector3.one, 0.5f);
            t1.SetUpdate(true); //nextDr
            Transform newDr = Instantiate(MoveController.Instance.dragonData[nextDr].DragonPrefapUI, pos,
                Quaternion.Euler(-113, -30, 90), unlockNewDr).transform;
            newDr.transform.localRotation = Quaternion.Euler(-94, -30, 90);
            newDr.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Vector3 drScale = newDr.localScale * 350;
            Dr = newDr;
            Rotate = true;
            Tweener t2 = newDr.DOScale(drScale, .5f);
            t2.SetUpdate(true);
            AudioSource sd = Instantiate(NewDragonSound, newDr.position, newDr.rotation);
            Destroy(sd.gameObject, 3f);
            sd.Play();
            CheckSpecialDragon();
            /*
            RewardNumber.text = "+" + rewardNm;
            if (uni)
                UnicornIcon.gameObject.SetActive(true);
            else if (bomb)
                BombIcon.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(.75f);
            if (bomb || uni)
            {
                Tweener t5 = RewardText.DOScale(Vector3.one, 0.5f);
                t5.SetUpdate(true);
                yield return new WaitForSecondsRealtime(0.5f);
                Tweener t6 = BombIcon.parent.DOScale(Vector3.one, 0.5f);
                t6.SetUpdate(true);
            }*/

            Tweener t4 = unlockNewDr.GetChild(2).DOScale(Vector3.one, 0.5f);
            t4.SetUpdate(true);
        }
        else
        {
            isOn = false;
            Time.timeScale = 1;
            ScrollController.Instance.CanCheck = true;
            TouchController.Instance.TakeControl = false;   
        }
    }

    public void ContinueButton()
    {
        GameManager.Instance.StartGamePlayEventCrazy();
        StartCoroutine(End());
    }

    void CheckSpecialDragon()
    {
        uni = false;
        bomb = false;
        UnicornIcon.gameObject.SetActive(false);
        BombIcon.gameObject.SetActive(false);
        
        if (PlayerPrefs.GetInt("UnLockBomb", 0) == 0 && nextDr == 3)
        {
            SpecialDragons.Instance.bombSize += 1;
            rewardNm = 1;
            bomb = true;
            PlayerPrefs.SetInt("UnLockBomb", 1);
        }
                
        if (PlayerPrefs.GetInt("UnLockUnicorn", 0) == 0 && nextDr == 5)
        {
            SpecialDragons.Instance.unicornSize += 1;
            uni = true;
            rewardNm = 1;
            PlayerPrefs.SetInt("UnLockUnicorn", 1);
        }
            // case 6:
            //     SpecialDragons.Instance.unicornSize += 1;
            //     rewardNm = 1;
            //     uni = true;
            //     break;
            // case 7:
            //     SpecialDragons.Instance.bombSize += 1;
            //     rewardNm = 1;
            //     bomb = true;
            //     break;
            // case 8:
            //     SpecialDragons.Instance.unicornSize += 2;
            //     rewardNm = 2;
            //     uni = true;
            //     break;
            // case 9:
            //     SpecialDragons.Instance.bombSize += 2;
            //     rewardNm = 2;
            //     bomb = true;
            //     break;
        //}
        // if (nextDr > 9)
        // {
        //     if (!Sp)
        //     {
        //         SpecialDragons.Instance.unicornSize += 2;
        //         rewardNm = 2;
        //         uni = true;
        //         Sp = true;
        //     }
        //     else
        //     {
        //         SpecialDragons.Instance.bombSize += 2;
        //         rewardNm = 2;
        //         bomb = true;
        //         Sp = false;
        //     }
        // }
        SpecialDragons.Instance.Refresh();
        StartCoroutine(SpecialDragons.Instance.UnLock());
    }

    IEnumerator End()
    {
        isOn = false;
        Time.timeScale = 1;
        unlockNewDr.gameObject.SetActive(true);
        unlockNewDr.gameObject.SetActive(false);
        Image im = unlockNewDr.GetComponent<Image>();
        unlockNewDr.GetChild(0).DOScale(Vector3.zero, 0.2f);
        unlockNewDr.GetChild(1).DOScale(Vector3.zero, 0.2f);
        //unlockNewDr.GetChild(2).DOScale(Vector3.zero, 0.2f);
        RewardText.DOScale(Vector3.zero, 0.2f);
        BombIcon.parent.DOScale(Vector3.zero, 0.2f);
        Dr.DOScale(0, 0.2f);
        im.DOFade(0, 0.2f);
        yield return new WaitForSeconds(0.25f);
        Rotate = false;
        unlockNewDr.GetChild(0).localScale = Vector3.zero;
        unlockNewDr.GetChild(1).localScale = Vector3.zero;
        //unlockNewDr.GetChild(2).localScale = Vector3.zero;
        Destroy(Dr.gameObject);
        im.DOFade(0, 0.01f);
        yield return new WaitForSeconds(0.01f);
        ScrollController.Instance.CanCheck = true;
        TouchController.Instance.TakeControl = false;
    }

    public void Init()
    {
        bigX = 0;
        for (int i = 0; i < MoveController.Instance.currentdr; i++)
        {
            bigX -= 215;
        }
    }

    public void Initialize()
    {
        float x = 0;
        for (int i = 0; i < dr.Count; i++)
        {
            dr[i].gameObject.SetActive(true);
            dr[i].transform.localPosition = new Vector3(x, 0, 0);
            x += 215;
        }
        float handleX = 0;
        for (int i = 0; i <= nextDr; i++)
        {
            dr[i].transform.GetChild(1).localScale = Vector3.one;
            dr[i].transform.GetChild(2).localScale = Vector3.one;
            dr[i].transform.GetChild(1).gameObject.SetActive(true);
            if (i != 0)
                handleX -= 215;
        }

        handle.localPosition = new Vector3(handleX, 0, 0);
    }
}