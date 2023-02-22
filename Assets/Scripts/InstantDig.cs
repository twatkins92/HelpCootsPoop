using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InstantDig : Digger
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Dig();
        }
    }
}
