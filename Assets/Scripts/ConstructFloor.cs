using System.Collections.Generic;
using UnityEngine;

public class FloorConstructor : MonoBehaviour
{
    public GameObject floorParent;
    public GameObject cubePrefab;
    public List<GameObject> gameObjects = new List<GameObject>();
    public int top = 1;
    public float distanceBetweenCubes;
    public int radius;
    public float height;

    void Start()
    {
        ConstructFloor();
        floorParent = gameObject;
    }

    [ContextMenu("Construct Floor")]
    public void ConstructFloor()
    {
        for (float i = -radius; i <= radius; i += distanceBetweenCubes)
        {
            for (float j = -radius; j <= radius; j += distanceBetweenCubes) { gameObjects.Add(Instantiate(cubePrefab, new Vector3(i, height, j), Quaternion.identity, floorParent.transform)); }
        }
    }

    [ContextMenu("Deconstruct Floor")]
    public void DeconstructFloor()
    {
        foreach (GameObject obj in gameObjects)
        {
            DestroyImmediate(obj);
        }
        gameObjects.Clear(); // Clear the list after destroying all objects
    }
}
