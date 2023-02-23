using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLitterTray : MonoBehaviour
{
    public IntVariable poopsCleared_so;

    public int minNumberOfPoops = 5;
    public int maxNumberOfPoops = 20;
    public int minNumberOfRocks = 5;
    public int maxNumberOfRocks = 20;

    public float rotationRange = 180f;
    public Vector3 poopScaleRange = Vector3.one;
    public Vector3 rockScaleRange = Vector3.one;

    public float distanceFromOtherObjects = 0.5f; 

    public float yHeightOfMesh = 0.15f;

    public GameObject poopPrefab;
    public GameObject rockPrefab;

    private Mesh meshToPlacePoosOn;

    private List<GameObject> poops = new List<GameObject>();

    void Awake()
    {
        meshToPlacePoosOn = GetComponentInChildren<MeshFilter>().mesh;

        Generate();
    }

    private void Generate()
    {
        // Get the bounds of the mesh so we know its size.
        Bounds bounds = meshToPlacePoosOn.bounds;

        // Calculate the random position range from the size of the mesh.
        // Minus the inside edge of the tray
        float xRange = (bounds.size.x / 2f) -0.15f;
        float zRange = (bounds.size.z / 2f) -0.15f;

        int numberOfPoops = Random.Range(minNumberOfPoops, maxNumberOfPoops);
        poopsCleared_so.Value = numberOfPoops;

        //should check not too close to existing poops or rocks
        for (int i = 0; i < numberOfPoops; i++)
        {
            // Generate a random position on the mesh within the calculated range.
            float x = Random.Range(-xRange, xRange) + bounds.center.x;
            float z = Random.Range(-zRange, zRange) + bounds.center.z;
            float y = yHeightOfMesh;

            Vector3 position = new Vector3(x, y, z);

            PlacePoop(position, poopPrefab, poopScaleRange);
        }

        int numberOfRocks = Random.Range(minNumberOfRocks, maxNumberOfRocks);

        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 position = GetPosition(xRange, zRange, bounds);

            PlacePoop(position, rockPrefab, rockScaleRange);
        }
    }

    private Vector3 GetPosition(float xRange, float zRange, Bounds bounds)
    {
        int maxTries = 40;
        Vector3 position = Vector3.zero;
        
        for (int i = 0; i <= maxTries; i++)
        {
            float x = Random.Range(-xRange, xRange) + bounds.center.x;
            float z = Random.Range(-zRange, zRange) + bounds.center.z;
            float y = yHeightOfMesh;

            position = new Vector3(x, y, z);

            foreach (GameObject poop in poops)
            {
                if (Vector3.Distance(position, poop.transform.position) > distanceFromOtherObjects) 
                {
                    return position;
                }
            }
        }
        return position;
    }

    private void PlacePoop(Vector3 placedPosition, GameObject prefab, Vector3 scaleRange)
    {
        // Generate a random rotation around the y-axis.
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(-rotationRange, rotationRange), 0f);

        // Generate a random scale for the prefab.
        Vector3 scale = Vector3.Scale(scaleRange, new Vector3(Random.Range(0.5f, 1.2f), Random.Range(0.5f, 1.2f), Random.Range(0.5f, 1.2f)));

        // Create an instance of the prefab with the random position, rotation, and scale.
        GameObject rockOrPoop = Instantiate(prefab, placedPosition, rotation);
        rockOrPoop.transform.localScale = scale;
        poops.Add(rockOrPoop);
    }
}
