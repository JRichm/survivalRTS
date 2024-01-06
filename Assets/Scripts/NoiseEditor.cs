using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Windows;
using Unity.VisualScripting;

public class NoiseEditor : MonoBehaviour
{
    [SerializeField] private GameObject displayImageObject;


    [Header("World Menu Elements")]
    [SerializeField] private GameObject seedInput;
    [SerializeField] private Slider mapSizeSlider;
    [SerializeField] private Slider noiseSlider;
    [SerializeField] private Slider biomeSlider;

    [Header("World Menu Values")]
    [SerializeField] private TextMeshProUGUI displaySeed;
    [SerializeField] private TextMeshProUGUI mapSizeValue;
    [SerializeField] private TextMeshProUGUI noiseScaleValue;
    [SerializeField] private TextMeshProUGUI biomesValue;

    [Header("Player Menu Elements")]
    [SerializeField] private Slider playerCountSlider;
    [SerializeField] private Slider distanceSlider;
    [SerializeField] private Slider spawnSizeSlider;
    [SerializeField] private Slider circleBlurSlider;

    [Header("Player Menu Values")]
    [SerializeField] private TextMeshProUGUI playerCountValue;
    [SerializeField] private TextMeshProUGUI distanceValue;
    [SerializeField] private TextMeshProUGUI spawnSizeValue;
    [SerializeField] private TextMeshProUGUI circleBlurValue;

    private Color[] pixels;
    private string seedValue;
    private int mapSize;
    private int noiseScaleInt;
    private int numBiomes;

    private int numPlayers;
    private float distance;
    private int spawnSize;
    private float blurScale;

    private TMP_InputField seedInputComponent;

    private Texture2D terrainTexture;
    private Texture2D spawnCircleTexture;

    private bool inputChanged;
    private bool isChanging;


    private void Start() {

        inputChanged = false;
        isChanging = false;

        // get seed input element
        seedInputComponent = seedInput.GetComponent<TMP_InputField>();

        // genereate random seed if no seed is input
        string inputSeed = seedInputComponent.text;
        if (string.IsNullOrEmpty(inputSeed) || inputSeed == "Seed...") {
            inputSeed = UnityEngine.Random.value.ToString();
            seedInputComponent.text = inputSeed;
        }

        // capture and set variables
        seedValue = seedInputComponent.text;
        mapSize = (int)mapSizeSlider.value;
        noiseScaleInt = (int)noiseSlider.value;
        numBiomes = (int)biomeSlider.value;

        // add event listenser to user inputs
        seedInputComponent.onValueChanged.AddListener(delegate { inputChanged = true; });
        mapSizeSlider.onValueChanged.AddListener(delegate { inputChanged = true; });
        noiseSlider.onValueChanged.AddListener(delegate { inputChanged = true; });
        biomeSlider.onValueChanged.AddListener(delegate { inputChanged = true; });

        playerCountSlider.onValueChanged.AddListener(delegate { inputChanged = true; });
        distanceSlider.onValueChanged.AddListener(delegate { inputChanged = true; });
        spawnSizeSlider.onValueChanged.AddListener(delegate { inputChanged = true; });
        circleBlurSlider.onValueChanged.AddListener(delegate { inputChanged = true; });


        // create first generation
        UpdateMapTexture();
    }


    private void Update() {

        isChanging = UnityEngine.Input.GetMouseButton(0);

        if (!isChanging && inputChanged ) {
            HandleUserInput();
        }
    }


    private void HandleUserInput() {
        inputChanged = false;

        bool updateWorld = false;
        bool updatePlayers = false;

        if (seedValue != seedInputComponent.text) {
            updateWorld = true;
            seedValue = seedInputComponent.text;
        }

        if (mapSize != (int)mapSizeSlider.value) {
            updateWorld = true;
            mapSize = (int)mapSizeSlider.value;
        }

        if (noiseScaleInt != (int)noiseSlider.value) {
            updateWorld = true;
            noiseScaleInt = (int)noiseSlider.value;
        }

        if (numBiomes != (int)biomeSlider.value) {
            updateWorld = true;
            numBiomes = (int)biomeSlider.value;
        }

        if (updateWorld) {
            terrainTexture = UpdateTerrain();
        }

        if (numPlayers != (int)playerCountSlider.value) {
            updatePlayers = true;
            numPlayers = (int)playerCountSlider.value;
        }

        if (distance != distanceSlider.value) {
            updatePlayers = true;
            distance = distanceSlider.value;
        }

        if (spawnSize != spawnSizeSlider.value) {
            updatePlayers = true;
            spawnSize = (int)spawnSizeSlider.value;
        }

        if (blurScale != circleBlurSlider.value) {
            updatePlayers = true;
            blurScale = circleBlurSlider.value;
        }

        if (updatePlayers) {
            spawnCircleTexture = UpdateSpawnCircle();
        }

        if (updateWorld || updatePlayers) {
            UpdateMapTexture();
        }
    }

    private void UpdateMapTexture() {


        if (!terrainTexture) {
            terrainTexture = UpdateTerrain();
        }

        if (!spawnCircleTexture) {
            spawnCircleTexture = UpdateSpawnCircle();
        }

        Sprite convertedSprite = Sprite.Create(terrainTexture, new Rect(0, 0, mapSize, mapSize), Vector2.zero);
        convertedSprite.name = "ConvertedSprite";
        convertedSprite.texture.filterMode = FilterMode.Point;
        displayImageObject.GetComponent<Image>().sprite = convertedSprite;

    }

    private Texture2D UpdateTerrain() {

        // update variables with user input
        mapSize = (int)mapSizeSlider.value;
        noiseScaleInt = (int)noiseSlider.value;
        numBiomes = (int)biomeSlider.value;

        // update display text
        mapSizeValue.text = mapSize.ToString() + "x" + mapSize.ToString();
        noiseScaleValue.text = noiseScaleInt.ToString();
        biomesValue.text = numBiomes.ToString();

        // Reinitialize pixels array with the new mapSize
        pixels = new Color[mapSize * mapSize];

        // Generate and apply the new texture
        Texture2D newTexture = GenerateNoiseMap(mapSize, noiseScaleInt, Color.green, Color.red, seedValue);

        return newTexture;
    }

    private Texture2D GenerateNoiseMap(int size, float scale, Color color1, Color color2, string seed) {
        Texture2D noiseTexture = new Texture2D(size, size);
        noiseTexture.filterMode = FilterMode.Point;

        // set pixel colors to first color
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = color1;
        }

        noiseTexture.SetPixels(pixels);
        noiseTexture.Apply();

        Texture2D topLayer = new Texture2D(size, size);

        // Calculate the center of the texture
        float centerX = size / 2;
        float centerY = size / 2;

        string inputSeed = seedInput.GetComponent<TMP_InputField>().text;
        int seedHashCode = seed.GetHashCode();
        System.Random random = new System.Random(seedHashCode);

        for (int x = 0; x < mapSize; x++) {
            for (int y = 0; y < mapSize; y++) {
                // Calculate coordinates relative to the center with different scaling for x and y
                float xCord = ((float)x - centerX) / mapSize * (noiseSlider.value + 1) * 1;
                float yCord = ((float)y - centerY) / mapSize * (noiseSlider.value + 1f) * 1;

                // Incorporate seed into Perlin noise calculations
                float sample = Mathf.PerlinNoise(xCord + random.Next(0, 1) - centerX, yCord + random.Next(0, 1) - centerY);

                pixels[y * mapSize + x] = new Color(color2.r, color2.g, color2.b, sample);
            }
        }

        topLayer.SetPixels(pixels);
        topLayer.Apply();

        noiseTexture = AddTextures(noiseTexture, topLayer);

        return noiseTexture;
    }

    private Texture2D UpdateSpawnCircle() {
        // create spawnPoints list
        Vector2[] spawnPoints = new Vector2[numPlayers];

        // loop through number of players
        for (int i = 0; i < numPlayers; i++) {
            // calculate angle between players
            float angle = (360f / numPlayers) * i;

            // calculate position of each player
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * distance;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * distance;

            spawnPoints[i] = new Vector2(mapSize / 2 + x, mapSize / 2 + y);
        }


        // create new texture and set its filter mode to point
        Texture2D mapTexture = new Texture2D(mapSize, mapSize);
        mapTexture.filterMode = FilterMode.Point;

        // set pixel colors to transparent
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = new Color(0,0,0,0);
        }

        Vector2 innerRange = new Vector2(distance - (int)(spawnSize / 3) - (blurScale * (spawnSize / 3)), distance - (int)(spawnSize / 3) + (blurScale * (spawnSize / 3)));
        Vector2 outterRange = new Vector2(distance + (int)(spawnSize / 3) - (blurScale * (spawnSize / 3)), distance + (int)(spawnSize / 3) + (blurScale * (spawnSize / 3)));
        // draw circle in-between players
        for (int x = -(int)distance - (int)(spawnSize / 3); x <= (int)distance + (int)(spawnSize / 3); x++) {
            for (int y = -(int)distance - (int)(spawnSize / 3); y <= (int)distance + (int)(spawnSize / 3); y++) {

                // position of current pixel
                int xPos = (mapSize / 2) + x;
                int yPos = (mapSize / 2) + y;

                float distance = Mathf.Sqrt(x * x + y * y);

                float alpha = 0.0f;

                if (distance > innerRange[1] && distance < outterRange[0]) alpha = 1.0f;
                else if (distance > innerRange[0] && distance < innerRange[1]) alpha = (distance - innerRange[0]) / (innerRange[1] - innerRange[0]);
                else if (distance > outterRange[0] && distance < outterRange[1]) alpha = 1 - (distance - outterRange[0]) / (outterRange[1] - outterRange[0]);


                pixels[yPos * mapSize + xPos] = Color.Lerp(pixels[yPos * mapSize + xPos], Vector4.zero, alpha);
            }
        }

        // calculate min/max blur distance from center
        float maxBlurDis = spawnSize + (blurScale * spawnSize * 2);
        float minBlurDis = spawnSize - (blurScale * spawnSize);

        // draw circle for every spawn point
        foreach (Vector2 position in spawnPoints) {

            // loop through pixel in circle range
            for (int x = -spawnSize; x <= spawnSize; x++) {
                for (int y = -spawnSize; y <= spawnSize; y++) {

                    // position of current pixel
                    int xPos = Mathf.RoundToInt(position.x) + x;
                    int yPos = Mathf.RoundToInt(position.y) + y;

                    // distance form center circle
                    float distance = Mathf.Sqrt(x * x + y * y);

                    float alpha = 0.0f;

                    // inside the circle
                    if (distance < spawnSize) {
                        alpha = 1.0f;
                    }

                    // within the blur range, fade from black to white
                    if (distance >= minBlurDis && distance <= maxBlurDis) {
                        alpha = 1.0f - Mathf.Clamp01((distance - minBlurDis) / (blurScale * spawnSize));
                    }

                    pixels[yPos * mapSize + xPos] = Color.Lerp(pixels[yPos * mapSize + xPos], Vector4.zero, alpha);
                }
            }
        }
        mapTexture.SetPixels(pixels);
        mapTexture.Apply();

        return mapTexture;
    }

    private Texture2D AddTextures(Texture2D firstTexture, Texture2D secondTexture) {
        // Ensure both textures have the same dimensions
        if (firstTexture.width != secondTexture.width || firstTexture.height != secondTexture.height) {
            Debug.LogError("Textures must have the same dimensions for blending.");
            return null;
        }

        int width = firstTexture.width;
        int height = firstTexture.height;

        Texture2D resultTexture = new Texture2D(width, height);

        Color[] pixels1 = firstTexture.GetPixels();
        Color[] pixels2 = secondTexture.GetPixels();
        Color[] resultPixels = new Color[width * height];

        for (int i = 0; i < pixels1.Length; i++) {
            // Blend using alpha channel
            resultPixels[i] = Color.Lerp(pixels1[i], pixels2[i], pixels2[i].a);
        }

        resultTexture.SetPixels(resultPixels);
        resultTexture.Apply();

        return resultTexture;
    }

    private string GenerateRandom64Digits(string seed) {
        // Ensure seed is not null
        if (seed == null) {
            throw new ArgumentNullException(nameof(seed));
        }

        // Convert the seed to a hash code
        int seedHashCode = seed.GetHashCode();

        // Initialize the random object with the seed hash code
        System.Random random = new System.Random(seedHashCode);

        // Generate a 64-digit long string of random numbers
        string randomNumber = "";
        for (int i = 0; i < 32; i++) {
            randomNumber = randomNumber += random.Next(0, 10);
        }

        displaySeed.text = randomNumber;
        return randomNumber;
    }
}
