using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private enum State {none,bomb,unicorn}
    [SerializeField]
    private  State state = State.none;

    public void OnPointerDown(PointerEventData eventData)
    {
        TouchController.Instance.TakeControl = true;
        StopCoroutine(AddTime());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (state == State.bomb && SpecialDragons.Instance.bombSize > 0)
            MoveController.Instance.TakeBomb();
        else if (state == State.unicorn && SpecialDragons.Instance.unicornSize > 0)
            MoveController.Instance.TakeUnicorn();
        else
            StartCoroutine(AddTime());
    }


    IEnumerator AddTime()
    {
        yield return new WaitForSeconds(0.25f);
        TouchController.Instance.TakeControl = false;
    }
}