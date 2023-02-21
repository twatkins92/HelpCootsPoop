using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGarden : MonoBehaviour
{
    public IntVariable poopsCleared_so;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //calculate score
            //trigger coots animator
            //bring up ui for end/regen
            Debug.Log("score is : " + poopsCleared_so.Value);
            //resetting here but need to put this in the start/restart logic
            poopsCleared_so.Value = 0;
        }
    }
}
