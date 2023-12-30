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

    // Start is called before the first frame update
    void Start()
    {

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


        Vector2[] spawnPoints = GenerateSpawnPoints(numPlayers, spawnDistance);

        GenerateTexture(spawnPoints);

    }

    private void GenerateTexture(Vector2[] spawnPoints) {
        Texture2D mapTexture = new Texture2D(textureWidth, textureHeight);
        mapTexture.filterMode = FilterMode.Point;

        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = Color.white;
        }

        foreach (Vector2 position in spawnPoints) {
            DrawBlurredCircle(pixels, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), circleRadius, blurScale, Color.black);
        }

        mapTexture.SetPixels(pixels);
        mapTexture.Apply();

        GetComponent<Renderer>().material.mainTexture = mapTexture;
    }

    private Vector2[] GenerateSpawnPoints(int numPlayers, float distance) {
        Vector2[] spawnPoints = new Vector2[numPlayers];

        for (int i = 0;i < numPlayers; i++) {
            float angle = (360f / numPlayers) * i;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * distance;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * distance;

            spawnPoints[i] = new Vector2(textureWidth / 2 + x, textureHeight / 2 + y);
        }

        return spawnPoints;
    }

    private void DrawBlurredCircle(Color[] pixels, int centerX, int centerY, int radius, float blurScale, Color color) {

        // min/max range of blur in pixels
        float maxBlurDistance = radius + (blurScale * radius);
        float minBlurDistance = radius - (blurScale * radius);

        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                int xPos = centerX + x;
                int yPos = centerY + y;

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


                pixels[yPos * textureWidth + xPos] = Color.Lerp(pixels[yPos * textureWidth + xPos], color, alpha);
            }
        }
    }
}
