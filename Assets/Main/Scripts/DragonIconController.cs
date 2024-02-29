using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonIconController : MonoBehaviour
{
    private float x;

    void Update()
    {
       if (transform.position.x >= 0)
       {
            CalculateScalePlusX();
            transform.localScale = new Vector3(x, x, x);
       }
       else if (transform.position.x < 0)
       {           
            CalculateScaleMinusX();
            transform.localScale = new Vector3(x, x, x);                  
       }
    }

    void CalculateScalePlusX()
    {
        x = (0.45f - transform.position.x * (0.45f / 0.3765f)) + 0.75f;
        x = Mathf.Max(0.75f, x);
        x = Mathf.Min(1.2f, x);
    }
    void CalculateScaleMinusX()
    {       
        x = (0.45f - transform.position.x * (0.45f / -0.3765f)) + 0.75f;
        x = Mathf.Max(0.75f, x);
        x = Mathf.Min(1.2f, x);
    }
}