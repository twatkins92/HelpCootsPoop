using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopController : MonoBehaviour
{
    public IntVariable poopsCleared_so;

    public void ClearPoop()
    {
       poopsCleared_so.Value += 1; 
       //trigger poop cleared animation
       StartCoroutine(WaitFor(2, () => { Destroy(this.gameObject); }));
    }

    private static IEnumerator WaitFor(int seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }
}
