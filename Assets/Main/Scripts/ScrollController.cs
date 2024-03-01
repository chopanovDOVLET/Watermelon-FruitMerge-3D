using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Lean.Touch;
public class ScrollController : MonoBehaviour
{
    public static ScrollController Instance;

    [SerializeField] Transform scroller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] GraphicRaycaster m_Raycaster;
    [SerializeField] EventSystem m_EventSystem;
    [SerializeField] DragonIndicator drIn;
    PointerEventData m_PointerEventData;
    Tweener tweener;
    private List<float> XPos = new List<float>();
    private Vector3 horizontalMove;
    public bool CanCheck;
    private bool TakeControl = true;
    Vector2 delta;

    void Start()
    {
        Input.multiTouchEnabled = false;
        Instance = this;
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();

        float x = 0;
        for (int i = 0; i < MoveController.Instance.dragonData.Count; i++)
        {
            XPos.Add(x);
            x -= 215;
        }
    }

    void Update()
    {
        if(CanCheck)
         CheckHit();
        if(!TakeControl)
         Controller();
    }

    private void Controller()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                delta = LeanGesture.GetScreenDelta(LeanTouch.Fingers);
                horizontalMove = (transform.right * (delta.x * 5) * Time.deltaTime) * 20f;
                scroller.localPosition += horizontalMove;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                delta = LeanGesture.GetScreenDelta(LeanTouch.Fingers);
                horizontalMove = (transform.right * (delta.x * 5) * Time.deltaTime) * 20f;
                scroller.localPosition += horizontalMove;
            }
        }

        if (scroller.localPosition.x > 0)
        {
            scroller.localPosition = new Vector3(0, 0, 0);
        }
        else if (scroller.localPosition.x < drIn.X)
        {
            scroller.localPosition = new Vector3(drIn.X, 0, 0);
        }
    }

    void CheckHit()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                m_PointerEventData = new PointerEventData(m_EventSystem);
                m_PointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();

                m_Raycaster.Raycast(m_PointerEventData, results);

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.tag == "Scroll")
                    {
                        if (tweener != null)
                            tweener.Kill();
                        TouchController.Instance.TakeControl = true;
                        TakeControl = false;
                    }
                }
            }
            else if(touch.phase == TouchPhase.Ended && !TakeControl)
            {
                TakeControl = true;
                SetToPos();
                StartCoroutine(AddTime());
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.tag == "Scroll")
                {
                    if(tweener != null)
                    tweener.Kill();
                    TouchController.Instance.TakeControl = true;
                    TakeControl = false;
                }
            }
        }
        else if(Input.GetMouseButtonUp(0) && !TakeControl)
        {
            TakeControl = true;
            SetToPos();
            StartCoroutine(AddTime());
        }

    }

    void SetToPos()
    {
        List<float> mathfAbs = new List<float>(); 

        for (int i = 0; i < XPos.Count; i++)
        {
            mathfAbs.Add(Mathf.Abs(XPos[i] - scroller.localPosition.x));
        }

        int num = 0;
        for (int i = 0; i < mathfAbs.Count; i++)
        {
            if (mathfAbs[num] > mathfAbs[i])
            {
                num = i;
            }             
        }

        tweener = scroller.DOLocalMoveX(XPos[num], 0.5f);
    }

    IEnumerator AddTime()
    {
        yield return new WaitForSeconds(0.25f);
        TouchController.Instance.TakeControl = false;
    }

}