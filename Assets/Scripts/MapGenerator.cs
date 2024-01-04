using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MapGenerator : MonoBehaviour {

    [Header("World Settings")]
    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;
    [SerializeField] int terrainScale;
    [SerializeField] int terrainHeight;

    [Header("Player Settings")]
    [SerializeField] private int numPlayers;

    [Header("Spawn Circle Settings")]
    [SerializeField] private int circleRadius;
    [SerializeField] private float blurScale;

    [Header("GameObjects")]
    [SerializeField] private GameObject plane;


    private Color[] pixels;
    private Texture2D spawnLocationsTexture;
    private Texture2D terrainNoiseTexture;

    private float spawnDistance;


    private void Start() {

        // initialize list of pixel colors;
        pixels = new Color[mapWidth * mapHeight];

        // calculate min/max spawn distance
        float minSpawnDistance = mapWidth / 6;
        float maxSpawnDistance = mapHeight / 3;

        if (numPlayers == 2) {
            spawnDistance = minSpawnDistance;
        } else if (numPlayers >= 3 && numPlayers <= 7) {
            float ratio = (float)(numPlayers - 3) / (7 - 3); // Calculate the ratio between min and max for 3 to 7 players
            spawnDistance = Mathf.Lerp(minSpawnDistance, maxSpawnDistance, ratio);
        } else {
            spawnDistance = maxSpawnDistance;
        }

        // create spawnPoints list
        Vector2[] spawnPoints = new Vector2[numPlayers];

        // loop through number of players
        for (int i = 0; i < numPlayers; i++) {
            // calculate angle between players
            float angle = (360f / numPlayers) * i;

            // calculate position of each player
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * spawnDistance;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * spawnDistance;

            spawnPoints[i] = new Vector2(mapWidth / 2 + x, mapHeight / 2 + y);
        }

        // generate texture based on spawn points
        spawnLocationsTexture = SpawnCircleTexture(spawnPoints);
        spawnLocationsTexture = BlurTexture(spawnLocationsTexture, 8);
        GameObject spawnPlane = Instantiate(plane);
        spawnPlane.transform.name = "Spawn Plane";
        spawnPlane.transform.localScale = new Vector3(mapWidth / 10, 1, mapHeight / 10);
        spawnPlane.GetComponent<Renderer>().material.mainTexture = spawnLocationsTexture;
        spawnPlane.transform.localScale = Vector3.one * 2;
        spawnPlane.transform.localPosition = new Vector3(-37, 0, 15);

        terrainNoiseTexture = TerrainNoise();
        GameObject terrainPlane = Instantiate(plane);
        terrainPlane.transform.name = "Terrain Plane";
        terrainPlane.transform.localScale = new Vector3(mapWidth / 10, 1, mapHeight / 10);
        terrainPlane.GetComponent<Renderer>().material.mainTexture = terrainNoiseTexture;
        terrainPlane.transform.localScale = Vector3.one * 2;
        terrainPlane.transform.localPosition = new Vector3(-13, 0, 15);

        Texture2D combinedTexture = CombineTexture(terrainNoiseTexture, InvertTexture(spawnLocationsTexture), "subtract");
        GameObject combinedPlane = Instantiate(plane);
        combinedPlane.transform.name = "Combined Plane";
        combinedPlane.transform.localScale = new Vector3(mapWidth / 10, 1, mapHeight / 10);
        combinedPlane.GetComponent<Renderer>().material.mainTexture = combinedTexture;
        combinedPlane.transform.localScale = Vector3.one * 2;
        combinedPlane.transform.localPosition = new Vector3(11, 0, 15);



    }

    private Texture2D CombineTexture(Texture2D mainTexture, Texture2D secondTexture, string combineFunction) {

        Texture2D combinedTexture = new Texture2D(mainTexture.width, mainTexture.width);

        switch (combineFunction) {
            case "subtract":
                for (int x = 0;x < mainTexture.width; x++) {
                    for (int y = 0; y < mainTexture.height; y++) {
                        pixels[y * mapWidth + x] = mainTexture.GetPixel(x, y) - secondTexture.GetPixel(x,y);
                    }
                }
                combinedTexture.SetPixels(pixels);
                combinedTexture.Apply();

                break;
        }

        return combinedTexture;
    }

    private Texture2D InvertTexture(Texture2D texture) {
        Texture2D invertedTexture = new Texture2D(texture.width, texture.height);
        for (int x = 0; x < texture.width; x++) {
            for (int y = 0; y < texture.height; y++) {
                pixels[y * mapWidth + x] = Color.white - texture.GetPixel(x, y);
            }
        }

        invertedTexture.SetPixels(pixels);
        return invertedTexture;
    }

    private Texture2D IncreaseContrast(Texture2D texture, float contrastFactor) {
        Texture2D adjustedTexture = new Texture2D(texture.width, texture.height);

        Color[] originalPixels = texture.GetPixels();
        Color[] adjustedPixels = new Color[originalPixels.Length];

        for (int i = 0; i < originalPixels.Length; i++) {
            float r = (originalPixels[i].r - 0.5f) * contrastFactor + 0.5f;
            float g = (originalPixels[i].g - 0.5f) * contrastFactor + 0.5f;
            float b = (originalPixels[i].b - 0.5f) * contrastFactor + 0.5f;

            adjustedPixels[i] = new Color(
                Mathf.Clamp01(r),
                Mathf.Clamp01(g),
                Mathf.Clamp01(b),
                originalPixels[i].a
            );
        }

        adjustedTexture.SetPixels(adjustedPixels);
        adjustedTexture.Apply();

        return adjustedTexture;
    }

    private Texture2D BlurTexture(Texture2D texture, int blurRadius) {
        Texture2D blurredTexture = new Texture2D(texture.width, texture.height);

        Color[] originalPixels = texture.GetPixels();
        Color[] blurredPixels = new Color[originalPixels.Length];

        for (int x = 0; x < texture.width; x++) {
            for (int y = 0; y < texture.height; y++) {
                int sampleCount = 0;
                Vector3 cumulativeColor = Vector3.zero;

                for (int dx = -blurRadius; dx <= blurRadius; dx++) {
                    for (int dy = -blurRadius; dy <= blurRadius; dy++) {
                        int sampleX = Mathf.Clamp(x + dx, 0, texture.width - 1);
                        int sampleY = Mathf.Clamp(y + dy, 0, texture.height - 1);

                        cumulativeColor.x += originalPixels[sampleY * texture.width + sampleX].r;
                        cumulativeColor.y += originalPixels[sampleY * texture.width + sampleX].g;
                        cumulativeColor.z += originalPixels[sampleY * texture.width + sampleX].b;
                        sampleCount++;
                    }
                }

                Color blurredColor = new Color(
                    cumulativeColor.x / sampleCount,
                    cumulativeColor.y / sampleCount,
                    cumulativeColor.z / sampleCount
                );

                blurredPixels[y * texture.width + x] = blurredColor;
            }
        }

        blurredTexture.SetPixels(blurredPixels);
        blurredTexture.Apply();

        return blurredTexture;
    }

    private Texture2D TerrainNoise() {
        // create new texture and set its filter mode to point
        Texture2D terrainTexture = new Texture2D(mapWidth, mapHeight);
        terrainTexture.filterMode = FilterMode.Point;

        // set pixel colors to white
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }

        float randomSeed = Random.value * 10;


        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                float xCoord = (float)x / mapWidth * terrainScale;
                float yCoord = (float)y / mapHeight * terrainScale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[y * mapWidth + x] = new Color(sample, sample, sample);
            }
        }

        terrainTexture.SetPixels(pixels);
        terrainTexture.Apply();

        return terrainTexture;
    }

    private Texture2D SpawnCircleTexture(Vector2[] spawnPoints) {

        // create new texture and set its filter mode to point
        Texture2D mapTexture = new Texture2D(mapWidth, mapHeight);
        mapTexture.filterMode = FilterMode.Point;

        // set pixel colors to white
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }


        Vector2 innerRange = new Vector2(spawnDistance - (int)(circleRadius / 3) - (blurScale * (circleRadius / 3)), spawnDistance - (int)(circleRadius / 3) + (blurScale * (circleRadius / 3)));
        Vector2 outterRange = new Vector2(spawnDistance + (int)(circleRadius / 3) - (blurScale * (circleRadius / 3)), spawnDistance + (int)(circleRadius / 3) + (blurScale * (circleRadius / 3)));

        // draw circle in-between players
        for (int x = -(int)spawnDistance - (int)(circleRadius / 3); x <= (int)spawnDistance + (int)(circleRadius / 3); x++) {
            for (int y = -(int)spawnDistance - (int)(circleRadius / 3); y <= (int)spawnDistance + (int)(circleRadius / 3); y++) {

                // position of current pixel
                int xPos = (mapWidth / 2) + x;
                int yPos = (mapHeight / 2) + y;

                float distance = Mathf.Sqrt(x * x + y * y);

                float alpha = 0.0f;

                if (distance > innerRange[1] && distance < outterRange[0]) alpha = 1.0f;
                else if (distance > innerRange[0] && distance < innerRange[1]) alpha = (distance - innerRange[0]) / (innerRange[1] - innerRange[0]);
                else if (distance > outterRange[0] && distance < outterRange[1]) alpha = 1 - (distance - outterRange[0]) / (outterRange[1] - outterRange[0]);


                pixels[yPos * mapWidth + xPos] = Color.Lerp(pixels[yPos * mapWidth + xPos], Vector4.zero, alpha);
            }
        }

        // calculate min/max blur distance from center
        float maxBlurDis = circleRadius + (blurScale * circleRadius * 2);
        float minBlurDis = circleRadius - (blurScale * circleRadius);

        // draw circle for every spawn point
        foreach (Vector2 position in spawnPoints) { 

            // loop through pixel in circle range
            for (int x = -circleRadius; x <= circleRadius; x++) {
                for (int y = -circleRadius; y <= circleRadius; y++) {

                    // position of current pixel
                    int xPos = Mathf.RoundToInt(position.x) + x;
                    int yPos = Mathf.RoundToInt(position.y) + y;

                    // distance form center circle
                    float distance = Mathf.Sqrt(x * x + y * y);

                    float alpha = 0.0f;

                    // inside the circle
                    if (distance < circleRadius) {
                        alpha = 1.0f;
                    }

                    // within the blur range, fade from black to white
                    if (distance >= minBlurDis && distance <= maxBlurDis) {
                        alpha = 1.0f - Mathf.Clamp01((distance - minBlurDis) / (blurScale * circleRadius));
                    }

                    pixels[yPos * mapWidth + xPos] = Color.Lerp(pixels[yPos * mapWidth + xPos], Vector4.zero, alpha);
                }
            }
        }
        mapTexture.SetPixels(pixels);
        mapTexture.Apply();

        return mapTexture;
    }
}