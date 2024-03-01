using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveController : MonoBehaviour
{
    public static MoveController Instance { get; private set; }

    public List<DragonData> dragonData = new List<DragonData>();
    [SerializeField] private Dragon Bomb;
    [SerializeField] private Dragon Unicorn;
    [SerializeField] private Transform nextRandomDr;
    [SerializeField] private Transform obiSolver;

    [SerializeField] Transform InstanPos;
    public Transform dragonsParent;
    [SerializeField] float pushForce;
    private Dragon mainDragon;
    [HideInInspector]
    public int hashId;
    public int currentdr;
    public int randomDr = 0;
    public int min, max;
    private bool tapToStart, isSpecialDr;
    Touch touch;
    TouchController Tscript;

    private void Awake()
    {
        Time.timeScale = 1;
        Instance = this;
        currentdr = PlayerPrefs.GetInt("CurrentDragon", 0);
        nextRandomDr.GetChild(randomDr).gameObject.SetActive(true);
    }

    void Start()
    {
        min = 0;
        max = currentdr;
        Application.targetFrameRate = 60;  
        Tscript = GetComponent<TouchController>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
             touch = Input.GetTouch(0);
        
         if (Input.touchCount > 0 && touch.phase == TouchPhase.Ended && Tscript.mainDragon != null && !Tscript.TakeControl)
         {
            Tscript.mainDragon = null;
            InstanPos.position = mainDragon.transform.position;
            if (isSpecialDr)
            {
                if (mainDragon.tag == "Bomb")
                    SpecialDragons.Instance.bombSize--;
                else if(mainDragon.tag == "Unicorn")
                    SpecialDragons.Instance.unicornSize--;

                SpecialDragons.Instance.Refresh();
                isSpecialDr = false;
            }
            
            ThrowDragon();
            Invoke("SpawnNewDragon", 0.5f);
         }
         else if ((Input.GetMouseButtonUp(0) || touch.phase == TouchPhase.Ended) && !tapToStart)
         {
            tapToStart = true;
            SpawnNewDragon();
         }
    }

    void SpawnNewDragon()
    {
        if (mainDragon != null)
            mainDragon.isMain = false;

        bool isContinueMerge = false;
        for (int i = 0; i < dragonsParent.childCount; i++) 
            if (dragonsParent.GetChild(i).GetComponent<Dragon>().isMergeing) 
            {
                isContinueMerge = true;
                break;
            }
        if (!isContinueMerge) 
            DragonSpawner.Instance.mergeCount = 0;
        
        Vector3 pos = new Vector3(InstanPos.position.x, dragonData[randomDr].DragonPrefap.transform.position.y, InstanPos.position.z-6);
        mainDragon = Instantiate(dragonData[randomDr].DragonPrefap, pos, Quaternion.Euler(-90,0,-90)).GetComponent<Dragon>();
        mainDragon.transform.DOMoveZ(InstanPos.position.z, 0.2f).OnComplete(() => mainDragon.transform.DORotateQuaternion(Quaternion.Euler(-55,0,90), 0.15f).OnComplete(() => SetPar()));
        mainDragon.isMergeing = true;
        mainDragon.isMain = true;
        mainDragon.id = dragonData[randomDr].id;
        mainDragon.score = dragonData[randomDr].score;
        mainDragon.hashId = hashId;
        hashId++;

        max = (currentdr > 3 && currentdr < 7) ? 3 : currentdr;
        if (currentdr > 6)
        {
            min = currentdr - 6;
            max = currentdr - 3;
        }
        
        randomDr = Random.Range(min, max);


        // if ((currentdr + 1) < 6)
        //     randomDr = Random.Range(0, currentdr + 1);
        // else
        //     randomDr = Random.Range(0, 7);

        for (int i = 0; i < nextRandomDr.childCount; i++)
            nextRandomDr.GetChild(i).gameObject.SetActive(false);

        nextRandomDr.GetChild(randomDr).gameObject.SetActive(true);
    }

    void SetPar()
    {
        if(isSpecialDr)
         TouchController.Instance.TakeControl = false;
        mainDragon.transform.GetChild(0).gameObject.SetActive(true);
        Tscript.mainDragon = mainDragon.transform;
    }

    void ThrowDragon()
    {
        mainDragon.isMergeing = false;
        mainDragon.drRigidbody.isKinematic = false;
        mainDragon.transform.GetChild(0).gameObject.SetActive(false);
        mainDragon.drRigidbody.AddForce(Vector3.forward * pushForce, ForceMode.Impulse);
        mainDragon.transform.parent = dragonsParent;
        mainDragon = null;
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0);
        
    }

    public void TakeBomb()
    {
        if (mainDragon != null && mainDragon.drRigidbody.velocity.magnitude > 0)
            mainDragon.isMain = false;
        else if (mainDragon != null)
            Destroy(mainDragon.transform.gameObject);

        isSpecialDr = true;
        Vector3 pos = new Vector3(InstanPos.position.x, Bomb.transform.position.y, InstanPos.position.z);
        Dragon spDr = Instantiate(Bomb,pos, Bomb.transform.rotation,dragonsParent);    
        Vector3 scale = spDr.transform.localScale;
        spDr.transform.localScale = Vector3.zero;
        spDr.isMain = true;
        mainDragon = spDr;
        spDr.transform.DOScale(scale, 0.2f).OnComplete(() => spDr.transform.GetChild(0).gameObject.SetActive(true)).OnComplete(() => SetPar());
        spDr.id = -1;
    }

    public void TakeUnicorn()
    {
        if (mainDragon != null && mainDragon.drRigidbody.velocity.magnitude > 0)
            mainDragon.isMain = false;
        else if (mainDragon != null)
            Destroy(mainDragon.transform.gameObject);

        isSpecialDr = true;
        Vector3 pos = new Vector3(InstanPos.position.x, Unicorn.transform.position.y, InstanPos.position.z);
        Dragon spDr = Instantiate(Unicorn, pos, Unicorn.transform.rotation, dragonsParent);
        Vector3 scale = spDr.transform.localScale;
        spDr.transform.localScale = Vector3.zero;
        mainDragon = spDr;
        spDr.isMain = true;
        spDr.transform.DOScale(scale, 0.2f).OnComplete(() => spDr.transform.GetChild(0).gameObject.SetActive(true)).OnComplete(() => SetPar());
        spDr.id = -1;
        spDr.hashId = hashId+20;
    }
}