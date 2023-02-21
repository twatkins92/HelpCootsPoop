using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InstantDig : MonoBehaviour
{
    private Diggable[] diggables;
    // Start is called before the first frame update
    void Start()
    {
        diggables = FindObjectsOfType<MonoBehaviour>().OfType<Diggable>().ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
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
}
