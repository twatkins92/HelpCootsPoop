using System.Collections;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
using UnityEngine;

public class KDT : MonoBehaviour
{
    public KDTree kdTree;

    public void Init()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] horzPoints = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            horzPoints[i] = vertices[i].Horizontal();
        }

        kdTree = new KDTree(horzPoints);
    }
}
