using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Digger : MonoBehaviour
{
    protected Diggable[] diggables;
    protected Obstacle[] obstacles;
    // Start is called before the first frame update
    public void Start()
    {
        diggables = FindObjectsOfType<MonoBehaviour>().OfType<Diggable>().ToArray();
        obstacles = FindObjectsOfType<MonoBehaviour>().OfType<Obstacle>().ToArray();
    }

    // Update is called once per frame
    public abstract void Dig();
}
