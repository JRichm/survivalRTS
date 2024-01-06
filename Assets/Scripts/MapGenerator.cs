using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject canvasObject;
    [SerializeField] GameObject planeObject;
    [SerializeField] GameObject cameraObject;


    [SerializeField] GameObject[] treeObjects;

    public void GenerateMap(Texture2D mapTexture) {
        Debug.Log("Generating map");
        Debug.Log(mapTexture.name);

        canvasObject.SetActive(false);

        GameObject displayPlane = Instantiate(planeObject);
        Destroy(planeObject);
        displayPlane.transform.localScale = new Vector3(mapTexture.width / 10, 1, mapTexture.height / 10);
        displayPlane.GetComponent<Renderer>().material.mainTexture = mapTexture;
        cameraObject.transform.position = new Vector3(0, mapTexture.width, 0);

        Color[] pixels = mapTexture.GetPixels();

        Vector3 min = Vector3.one;
        Vector3 max = Vector3.zero;

        Color grass = new Color(0.30f, 0.40f, 0.25f);

        for (int x = 0; x < mapTexture.width; x++) {
            for (int y = 0; y < mapTexture.height; y++) {
                Color color = pixels[x * mapTexture.width + y];
                if (color.r > max.x) max.x = color.r;
                if (color.g > max.y) max.y = color.g;
                if (color.b > max.z) max.z = color.b;
                if (color.r < min.x) min.x = color.r;
                if (color.g < min.y) min.y = color.g;
                if (color.b < min.z) min.z = color.b;

                Debug.Log(color);
                if (color.g > 0) {
                    GameObject spawnObject = null;

                    int biggestTree = 0;

                    Vector3 spawnLocation = new Vector3(-mapTexture.width / 2 + x, 0, -mapTexture.height / 2 + y);

                    if (color.g > 0.1 && color.g < 0.2) biggestTree = 1;
                    else if (color.g > 0.2 && color.g < 0.4) biggestTree = 2;

                    for (int i = biggestTree; i > 0; i--) {
                        if (CanSpawnTree(spawnLocation, treeObjects[biggestTree])) {
                            spawnObject = treeObjects[biggestTree];
                            i = 0;
                        }
                    }

                    if (spawnObject != null) {
                        GameObject newTree = Instantiate(spawnObject);
                        newTree.transform.position = spawnLocation;
                        float randomRotationY = Random.value * 360;
                        Quaternion randomRotation = Quaternion.Euler(-90, randomRotationY, 0);
                        newTree.transform.rotation = randomRotation;
                    }
                }
            }
        }
    }

    private bool CanSpawnTree(Vector3 position, GameObject treePrefab) {
        Collider[] colliders = Physics.OverlapBox(position, treePrefab.GetComponent<BoxCollider>().size / 2f, Quaternion.identity);

        return colliders.Length == 0;
    }
}
