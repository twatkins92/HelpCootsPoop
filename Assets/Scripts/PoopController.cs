using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopController : Diggable
{
    public IntVariable poopsCleared_so;
    public float clearRange = 0.1f;

    public Animator animator;

    private bool cleaned = false;

    public void ClearPoop()
    {
        if (cleaned)
            return;
        cleaned = true;
        poopsCleared_so.Value -= 1;
        SoundManager.PlaySound("PoopJingle");
        animator.Play("PoopCleared");
        //trigger poop cleared animation
        this.DoAfter(2, () => Destroy(this.gameObject));
    }

    public override void FailDig()
    {
        //nothing lol
    }

    public override void TryDig(Vector3 vector)
    {
        if (Vector3.Distance(vector.Horizontal(), transform.position.Horizontal()) < clearRange)
        {
            ClearPoop();
        }
    }
}
