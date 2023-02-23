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

    public override void Dig()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float intersectDistance = default;
        new Plane(Vector3.up, Vector3.zero).Raycast(ray, out intersectDistance);
        Vector3 zeroedIntersectPoint = ray.origin + ray.direction * intersectDistance;

        foreach (Diggable diggable in diggables)
        {
            if (diggable != null)
                diggable.TryDig(zeroedIntersectPoint);
        }
    }
}
