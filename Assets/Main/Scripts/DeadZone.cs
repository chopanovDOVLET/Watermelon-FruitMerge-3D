using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public Dragon dr ;

    private void OnTriggerStay(Collider other)
    {
        dr = other.GetComponent<Dragon>();

        if (dr != null)
        {
            if (!dr.isMain)
            {
                TouchController.Instance.TakeControl = true;
                MoveController.Instance.gameObject.GetComponent<TouchController>().enabled = false;
                MoveController.Instance.enabled = false;
                DragonIndicator.Instance.isOn = true;
                StartCoroutine(UIManager.Instance.EnableLosePanel());
            }
        }
    }

}
