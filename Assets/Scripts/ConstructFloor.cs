using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorConstructor : MonoBehaviour
{
    public GameObject floorParent;
    public GameObject cubePrefab;
    public List<GameObject> gameObjects = new List<GameObject>();
    public float lowestSaturation = 0.3f;
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
        float distanceBetweenCubeCenters = cubePrefab.transform.localScale.x + distanceBetweenCubes;

        for (float i = -radius; i <= radius; i += distanceBetweenCubeCenters)
        {
            for (float j = -radius; j <= radius; j += distanceBetweenCubeCenters) { if (i * i + j * j < radius * radius)
            {
                gameObjects.Add(Instantiate(
                    cubePrefab,
                    floorParent.transform.position + (floorParent.transform.rotation * new Vector3(i, height, j)),
                    Quaternion.identity,
                    floorParent.transform
                ));

                float hue = (Vector2.SignedAngle(Vector2.up, new Vector2(i, j)) + 180) / 360;
                Color color = Color.HSVToRGB(hue, 1 - new Vector2(i, j).magnitude / radius + lowestSaturation, 1f);
                gameObjects.Last().GetComponent<HighlightGameObject>().color = color;
            }}
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
