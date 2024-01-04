using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnArea : MonoBehaviour {

    [Header("Texture Settings")]
    [SerializeField] private int textureWidth;
    [SerializeField] private int textureHeight;

    [Header("Spawn Settings")]
    [SerializeField] private int numPlayers;
    [SerializeField] private float spawnDistance;

    [Header("Circle Settings")]
    public int circleRadius = 10;
    public float blurScale = 0.1f;
    public Vector2[] circlePositions;


    Material material;

    // Start is called before the first frame update
    void Start() {

        // get material of attatched planes
        material = GetComponent<Renderer>().material;

        // calculate min/max spawn distance
        float minSpawnDistance = textureHeight / 10;
        float maxSpawnDistance = textureHeight / 4;

        float spawnDistance;

        if (numPlayers == 2) {
            spawnDistance = minSpawnDistance;
        } else if (numPlayers >= 3 && numPlayers <= 7) {
            float ratio = (float)(numPlayers - 3) / (7 - 3); // Calculate the ratio between min and max for 3 to 7 players
            spawnDistance = Mathf.Lerp(minSpawnDistance, maxSpawnDistance, ratio);
        } else {
            spawnDistance = maxSpawnDistance;
        }

        // create spawnPoints list
        Vector2[] spawnPoints = GenerateSpawnPoints(numPlayers, spawnDistance);


        // generate texture based on spawn points
        GenerateTexture(spawnPoints);
    }

    private void GenerateTexture(Vector2[] spawnPoints) {

        // create new texture and set its filter mode to point
        Texture2D mapTexture = new Texture2D(textureWidth, textureHeight);
        mapTexture.filterMode = FilterMode.Point;


        // create new array of colors for every pixel of texture
        // set them to white
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }

        // draw circle for every spawn point
        foreach (Vector2 position in spawnPoints) {
            DrawBlurredCircle(pixels, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), circleRadius, blurScale);
        }

        // set and apply pixels
        mapTexture.SetPixels(pixels);
        mapTexture.Apply();

        // update material component settings
        material.shader = Shader.Find("Standard");
        material.SetFloat("_Mode", 3);
        material.mainTexture = mapTexture;
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000; // Set a valid render queue value
    }

    private Vector2[] GenerateSpawnPoints(int numPlayers, float distance) {
        // initialize vector2 list of spawn points
        Vector2[] spawnPoints = new Vector2[numPlayers];


        // loop through number of players
        for (int i = 0;i < numPlayers; i++) {
            
            // calculate angle between each player
            float angle = (360f / numPlayers) * i;

            // calculate position of each player
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * distance;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * distance;

            spawnPoints[i] = new Vector2(textureWidth / 2 + x, textureHeight / 2 + y);
        }
        return spawnPoints;
    }

    private void DrawBlurredCircle(Color[] pixels, int centerX, int centerY, int radius, float blurScale) {

        // min/max range of blur in pixels
        float maxBlurDistance = radius + (blurScale * radius * 2);
        float minBlurDistance = radius - (blurScale * radius);


        // transparent color
        Vector4 transparent = new Vector4(0, 0, 0, 0);

        // loop through pixel in circle range
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                // position of current pixelx
                int xPos = centerX + x;
                int yPos = centerY + y;

                // distance from center circle
                float distance = Mathf.Sqrt(x * x + y * y);

                float alpha = 0.0f;

                // Inside the circle, set alpha to 1
                if (distance < radius) {
                    alpha = 1.0f;
                }

                // Within the blur range, fade from black to white
                if (distance >= minBlurDistance && distance <= maxBlurDistance) {
                    alpha = 1.0f - Mathf.Clamp01((distance - minBlurDistance) / (blurScale * radius));
                }

                pixels[yPos * textureWidth + xPos] = Color.Lerp(pixels[yPos * textureWidth + xPos], transparent, alpha);
            }
        }
    }
}
