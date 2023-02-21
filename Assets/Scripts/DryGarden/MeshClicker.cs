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
            var side1 = centre - direction - right;
            var side2 = centre - direction + right;
            var side3 = centre + direction - right;
            var side4 = centre + direction + right;
            return new Vector3(
                Mathf.Min(side1.x, side2.x, side3.x, side4.x),
                -10,
                Mathf.Min(side1.z, side2.z, side3.z, side4.z)
            );
        }

        //Brush is a rotated cube but we need the bounding box in world space
        public Vector3 MaxBound()
        {
            var side1 = centre - direction - right;
            var side2 = centre - direction + right;
            var side3 = centre + direction - right;
            var side4 = centre + direction + right;
            return new Vector3(
                Mathf.Max(side1.x, side2.x, side3.x, side4.x),
                10,
                Mathf.Max(side1.z, side2.z, side3.z, side4.z)
            );
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

    private List<Vector3> drawnPoints = new List<Vector3>();

    void Start()
    {
        // Set the shared mesh of the mesh collider with the generated mesh
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        kdTree = GetComponent<KDT>();
    }

    // TODO replace with OnMouse; event to raise vertices should only happen when the mouse moves a certain distance though



    void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float intersectDistance = default;
            new Plane(Vector3.up, Vector3.zero).Raycast(ray, out intersectDistance);
            Vector3 zeroedIntersectPoint = ray.origin + ray.direction * intersectDistance;

            if (
                drawnPoints.Count > 0
                && Vector3.Distance(zeroedIntersectPoint, drawnPoints[^1]) < 0.1f
            )
                return;

            drawnPoints.Add(zeroedIntersectPoint);
            if (drawnPoints.Count > 1)
            {
                Vector3 centre = (drawnPoints[^1] + drawnPoints[^2]) / 2;

                int nearestVertexIndex = FindNearestVertex(centre);

                // TODO Rather than just face forward, face the direction the mouse is being dragged
                // Make sure it is normalised
                Vector3 forwardDirection = (drawnPoints[^1] - drawnPoints[^2]).normalized;
                Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * forwardDirection.normalized;

                Debug.DrawRay(centre, forwardDirection, Color.blue, 10f);
                Debug.DrawRay(centre, rightDirection, Color.green, 10f);

                Brush brush = new Brush
                {
                    centre = mesh.vertices[nearestVertexIndex],
                    direction = forwardDirection * (brushLength / 2),
                    right = rightDirection * (brushWidth / 2)
                };
                RaiseVertices(brush);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            drawnPoints.Clear();
        }
    }

    int FindNearestVertex(Vector3 point)
    {
        kdTreeResults.Clear();
        query.ClosestPoint(kdTree.kdTree, point, kdTreeResults);
        return kdTreeResults[0];
    }

    void RaiseVertices(Brush brush)
    {
        vertices = mesh.vertices;

        kdTreeResults.Clear();

        query.Interval(kdTree.kdTree, brush.MinBound(), brush.MaxBound(), kdTreeResults);

        Debug.DrawRay(brush.centre + Vector3.up * 3, brush.direction, Color.red, 10f);
        Debug.DrawRay(brush.centre + Vector3.up * 3, -brush.direction, Color.red, 10f);
        Debug.DrawRay(brush.centre + Vector3.up * 3, brush.right, Color.red, 10f);
        Debug.DrawRay(brush.centre + Vector3.up * 3, -brush.right, Color.red, 10f);
        DrawBounds(new Bounds() { min = brush.MinBound(), max = brush.MaxBound() }, 1f);
        //Debug.DrawRay(brush.MinBound().Horizontal(), Vector3.up, Color.blue, 10f);
        //Debug.DrawRay(brush.MaxBound().Horizontal(), Vector3.up, Color.blue, 10f);

        for (int i = 0; i < kdTreeResults.Count; i++)
        {
            SetVertexHeight(kdTreeResults[i], brush);
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void DrawBounds(Bounds b, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, Color.blue, delay);
        Debug.DrawLine(p2, p3, Color.red, delay);
        Debug.DrawLine(p3, p4, Color.yellow, delay);
        Debug.DrawLine(p4, p1, Color.magenta, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, Color.blue, delay);
        Debug.DrawLine(p6, p7, Color.red, delay);
        Debug.DrawLine(p7, p8, Color.yellow, delay);
        Debug.DrawLine(p8, p5, Color.magenta, delay);

        // sides
        Debug.DrawLine(p1, p5, Color.white, delay);
        Debug.DrawLine(p2, p6, Color.gray, delay);
        Debug.DrawLine(p3, p7, Color.green, delay);
        Debug.DrawLine(p4, p8, Color.cyan, delay);
    }

    void SetVertexHeight(int vertexIndex, Brush brush)
    {
        Vector3 vertex = vertices[vertexIndex];
        Vector3 difference = vertex - brush.centre;

        float directionDistance = Vector3.Project(difference, brush.direction).magnitude;
        float sideDistance = Vector3.Project(difference, brush.right).magnitude;

        if (directionDistance <= brushLength && sideDistance <= brushWidth)
        {
            vertices[vertexIndex] = new Vector3(
                vertex.x,
                GetVertexHeight(vertex.y, directionDistance, sideDistance),
                vertex.z
            );
        }
    }

    float GetVertexHeight(float prevHeight, float directionDistance, float sideDistance)
    {
        float directionRatio = directionDistance / (brushLength / 2);
        float sideRatio = sideDistance / (brushWidth / 2);

        float sideHeight = brushSideFallOff.Evaluate(sideRatio);
        float directionHeight = brushDirectionFallOff.Evaluate(directionRatio);

        float heightNoBlend = sideHeight * directionHeight;

        if (heightNoBlend < 0)
            return heightNoBlend;
        else
            return Mathf.Max(prevHeight, heightNoBlend);
    }
}
