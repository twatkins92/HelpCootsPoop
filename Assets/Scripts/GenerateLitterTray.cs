using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLitterTray : MonoBehaviour
{
    public IntVariable poopsCleared_so;

    public int minNumberOfPoops = 5;
    public int maxNumberOfPoops = 20;
    public float rotationRange = 180f;
    public Vector3 scaleRange = Vector3.one;

    public float yHeightOfMesh = 0.15f;

    public GameObject poopPrefab;

    private Mesh meshToPlacePoosOn;


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

        for (int i = 0; i < numberOfPoops; i++)
        {
            // Generate a random position on the mesh within the calculated range.
            float x = Random.Range(-xRange, xRange) + bounds.center.x;
            float z = Random.Range(-zRange, zRange) + bounds.center.z;
            float y = yHeightOfMesh;

            Vector3 position = new Vector3(x, y, z);

            PlacePoop(position);
        }
    }

    private void PlacePoop(Vector3 placedPosition)
    {
        // Generate a random rotation around the y-axis.
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(-rotationRange, rotationRange), 0f);

        // Generate a random scale for the prefab.
        Vector3 scale = Vector3.Scale(scaleRange, new Vector3(Random.Range(0.5f, 1.2f), Random.Range(0.5f, 1.2f), Random.Range(0.5f, 1.2f)));

        // Create an instance of the prefab with the random position, rotation, and scale.
        Instantiate(poopPrefab, placedPosition, rotation).transform.localScale = scale;
    }
}
