using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private enum State {none,bomb,unicorn}
    public enum AdState { none, ad }
    [SerializeField]
    private  State state = State.none;
    public AdState adState = AdState.none;

    public void OnPointerDown(PointerEventData eventData)
    {
        TouchController.Instance.TakeControl = true;
        StopCoroutine(AddTime());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if ((adState == AdState.ad && SpecialDragons.Instance.bombSize == 0) || adState == AdState.ad && SpecialDragons.Instance.unicornSize == 0)
        {
            CrazyGames.CrazyAds.Instance.beginAdBreakRewarded(() => Reward()); ;
        }
        
        if (state == State.bomb && SpecialDragons.Instance.bombSize > 0)
        {
            SpecialDragons.Instance.CloseTutorial();
            MoveController.Instance.TakeBomb();
        }
        else if (state == State.unicorn && SpecialDragons.Instance.unicornSize > 0)
        {
            SpecialDragons.Instance.CloseTutorial();
            MoveController.Instance.TakeUnicorn();
        }
        else
            StartCoroutine(AddTime());
    }
    
    private void Reward()
    {
        if (state == State.unicorn)
            SpecialDragons.Instance.unicornSize += 1;
        else
            SpecialDragons.Instance.bombSize += 1;

        SpecialDragons.Instance.Refresh();
    }


    IEnumerator AddTime()
    {
        yield return new WaitForSeconds(0.25f);
        TouchController.Instance.TakeControl = false;
    }
}