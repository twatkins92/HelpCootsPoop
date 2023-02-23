using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Diggable : MonoBehaviour
{
    public abstract void TryDig(Vector3 vector);
    public abstract void FailDig();
}
