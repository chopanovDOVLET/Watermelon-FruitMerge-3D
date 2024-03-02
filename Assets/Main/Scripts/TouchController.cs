using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Lean.Touch;

public class TouchController : MonoBehaviour
{
    public static TouchController Instance {  get; private set; }

    private float moveDistance = 2.35f;
    public Transform mainDragon;
    public Vector3 horizontalMove;
    public bool TakeControl;
    Vector2 delta;



    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (mainDragon != null && !TakeControl)
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
                horizontalMove = (transform.right * delta.x * Time.deltaTime) / (Screen.width / 400);
                Vector3 vector = mainDragon.position + horizontalMove;
                mainDragon.position = Vector3.Lerp(mainDragon.position, vector, Time.deltaTime * 65);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                delta = LeanGesture.GetScreenDelta(LeanTouch.Fingers);
                horizontalMove = (transform.right * delta.x * Time.deltaTime) / (Screen.width / 300);
                Vector3 vector = mainDragon.position + horizontalMove;
                mainDragon.position = Vector3.Lerp(mainDragon.position, vector, Time.deltaTime * 65);
            }
        }

        if (mainDragon.position.x > moveDistance)
        {
            mainDragon.position = new Vector3(moveDistance, mainDragon.position.y, mainDragon.position.z);
        }
        else if (mainDragon.position.x < -moveDistance)
        {
            mainDragon.position = new Vector3(-moveDistance, mainDragon.position.y, mainDragon.position.z);
        }
    }
}