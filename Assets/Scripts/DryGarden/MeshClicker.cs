using System;
using System.Collections;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
using UnityEngine;

public class MeshClicker : Diggable
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

    private AphorismsUi aphorismsUi;

    // KDTree basically let's us find near vertices without looping through them all
    private KDT kdTree;
    private List<int> kdTreeResults = new List<int>();

    private KDQuery query = new KDQuery();

    private List<Vector3> drawnPoints = new List<Vector3>();

    private List<Tuple<float, Vector3>> historicalLines = new List<Tuple<float, Vector3>>();

    public float score = 0;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        kdTree = GetComponent<KDT>();
        aphorismsUi = FindObjectOfType<AphorismsUi>();
    }

    public override void TryDig(Vector3 vector)
    {
        Vector3 zeroedIntersectPoint = vector.Horizontal();
        if (!mesh.bounds.Contains(zeroedIntersectPoint))
        {
            drawnPoints.Clear();
            return;
        }

        if (drawnPoints.Count > 0 && Vector3.Distance(zeroedIntersectPoint, drawnPoints[^1]) < 0.1f)
            return;

        if (drawnPoints.Count == 0)
        {
            drawnPoints.Add(zeroedIntersectPoint);
        }
        else
        {
            int startIndex = drawnPoints.Count - 1;
            Vector3 diff = (zeroedIntersectPoint - drawnPoints[^1]);
            float distance = diff.magnitude;
            int numMidPoints = Mathf.FloorToInt(distance / (0.4f * brushLength));
            Vector3 nextPoint = drawnPoints[startIndex];
            for (int i = 0; i < numMidPoints; i++)
            {
                nextPoint += 0.4f * brushLength * diff.normalized;
                drawnPoints.Add(nextPoint);
            }
            drawnPoints.Add(zeroedIntersectPoint);

            for (int i = startIndex + 1; i < drawnPoints.Count; i++)
            {
                //Vector3 centre = (drawnPoints[^1] + drawnPoints[^2]) / 2;
                Vector3 centre = drawnPoints[i];

                int nearestVertexIndex = FindNearestVertex(centre);

                // TODO Rather than just face forward, face the direction the mouse is being dragged
                // Make sure it is normalised
                Vector3 forwardDirection = (drawnPoints[i] - drawnPoints[i - 1]).normalized;
                Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * forwardDirection.normalized;

                Brush brush = new Brush
                {
                    centre = mesh.vertices[nearestVertexIndex],
                    direction = forwardDirection * (brushLength / 2),
                    right = rightDirection * (brushWidth / 2)
                };
                RaiseVertices(brush);

                score += 5;
                Debug.Log(score);

                //sorry for this jank call in lol
                if (aphorismsUi != null)
                    aphorismsUi.ShowAphorism();
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

        for (int i = 0; i < kdTreeResults.Count; i++)
        {
            SetVertexHeight(kdTreeResults[i], brush);
        }

        /*for (int i = 0; i < kdTreeResults.Count; i++)
        {
            SmoothVertex(kdTreeResults[i]);
        }*/

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void SetVertexHeight(int vertexIndex, Brush brush)
    {
        Vector3 vertex = vertices[vertexIndex];
        Vector3 difference = vertex - brush.centre;

        float directionDistance = Vector3.Project(difference, brush.direction).magnitude;
        float sideDistance = Vector3.Project(difference, brush.right).magnitude;

        if (directionDistance <= brushLength / 2 && sideDistance <= brushWidth / 2)
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
            return Mathf.Min(prevHeight, heightNoBlend);
        else
            return Mathf.Max(prevHeight, heightNoBlend);
    }
    /*void SmoothVertex(int index)
{
   // Get the neighboring vertices of the vertex to be moved
   int[] neighborIndices = mesh.GetTriangles(index);
   Vector3[] neighbors = ;
   for (int i = 0; i < neighborIndices.Length; i++)
   {
       neighbors[i] = vertices[neighborIndices[i]];
   }

   // Calculate the average position of the neighboring vertices
   Vector3 averagePosition = Vector3.zero;
   for (int i = 0; i < neighbors.Length; i++)
   {
       averagePosition += neighbors[i];
   }
   averagePosition /= neighbors.Length;

   // Move the vertex towards the average position
   vertices[index] = Vector3.Lerp(vertices[index], averagePosition, 0.5f);
}*/
}
