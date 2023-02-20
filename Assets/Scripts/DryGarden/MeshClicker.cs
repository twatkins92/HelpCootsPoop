using System;
using System.Collections;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
using UnityEngine;

public class MeshClicker : MonoBehaviour
{
    public struct Brush
    {
        public Vector3 centre;
        public Vector3 direction;

        public Vector3 right;

        //Brush is a rotated cube but we need the bounding box in world space
        public Vector3 MinBound()
        {
            return new Vector3((centre - direction - right).x, -10, (centre - direction + right).z);
        }

        //Brush is a rotated cube but we need the bounding box in world space
        public Vector3 MaxBound()
        {
            return new Vector3((centre + direction + right).x, 10, (centre + direction - right).z);
        }
    }

    public float brushLength = 1f;
    public float brushWidth = 1f;

    public AnimationCurve brushDirectionFallOff;
    public AnimationCurve brushSideFallOff;

    private Mesh mesh;
    private MeshCollider meshCollider;
    private Vector3[] vertices;

    // KDTree basically let's us find near vertices without looping through them all
    private KDT kdTree;
    private List<int> kdTreeResults = new List<int>();

    private KDQuery query = new KDQuery();

    void Start()
    {
        // Set the shared mesh of the mesh collider with the generated mesh
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        kdTree = GetComponent<KDT>();
    }

    // TODO replace with OnMouse; event to raise vertices should only happen when the mouse moves a certain distance though
    void OnMouseDown()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
        {
            int nearestVertexIndex = FindNearestVertex(hit.point);

            // TODO Rather than just face forward, face the direction the mouse is being dragged
            // Make sure it is normalised
            Vector3 forwardDirection = Vector3.forward;
            Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * forwardDirection.normalized;

            Brush brush = new Brush
            {
                centre = mesh.vertices[nearestVertexIndex],
                direction = Vector3.forward * (brushLength / 2),
                right = rightDirection * (brushWidth / 2)
            };
            RaiseVertices(brush);
        }
    }

    int FindNearestVertex(Vector3 point)
    {
        kdTreeResults.Clear();
        query.ClosestPoint(kdTree.kdTree, point, kdTreeResults);
        Debug.Log("Closest: " + kdTreeResults[0]);
        return kdTreeResults[0];
    }

    void RaiseVertices(Brush brush)
    {
        vertices = mesh.vertices;

        kdTreeResults.Clear();
        query.Interval(kdTree.kdTree, brush.MinBound(), brush.MaxBound(), kdTreeResults);
        Debug.Log(brush.MinBound());
        Debug.Log(brush.MaxBound());
        Debug.Log(kdTreeResults.Count);
        Debug.DrawRay(brush.MinBound().Horizontal(), Vector3.up, Color.blue, 10f);
        Debug.DrawRay(brush.MaxBound().Horizontal(), Vector3.up, Color.blue, 10f);

        for (int i = 0; i < kdTreeResults.Count; i++)
        {
            SetVertexHeight(kdTreeResults[i], brush);
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void SetVertexHeight(int vertexIndex, Brush brush)
    {
        Debug.Log(vertexIndex);
        Vector3 vertex = vertices[vertexIndex];
        Vector3 difference = vertex - brush.centre;

        float directionDistance = Vector3.Project(difference, brush.direction).magnitude;
        float sideDistance = Vector3.Project(difference, brush.right).magnitude;

        if (directionDistance <= brushLength / 2f && sideDistance <= brushWidth / 2f)
        {
            vertices[vertexIndex] = new Vector3(
                vertex.x,
                GetVertexHeight(directionDistance, sideDistance),
                vertex.z
            );
        }
    }

    float GetVertexHeight(float directionDistance, float sideDistance)
    {
        float directionRatio = directionDistance / brushLength;
        float sideRatio = sideDistance / brushWidth;

        return brushDirectionFallOff.Evaluate(directionRatio)
            * brushSideFallOff.Evaluate(sideRatio);
    }
}
