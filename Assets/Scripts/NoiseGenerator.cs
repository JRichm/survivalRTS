using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour {// Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;

    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    public GameObject spawnObj;
    public GameObject parentObj;

    void Start() {
        rend = GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;
        CalcNoise();
    }

    private void CalcNoise() {
        // For each pixel in the texture...
        float y = 0.0F;

        while (y < noiseTex.height) {
            float x = 0.0F;
            while (x < noiseTex.width) {
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();

        SpawnBlocks();
    }

    private void SpawnBlocks() {
        Debug.Log("spawning blocks");
        for (int x = 0; x < pixWidth; x++) {
            for (int y = 0; y < pixHeight; y++) {

                Color color = pix[(int)x + (int)y];
                Debug.Log(color);

                Vector3 spawnPosition = new Vector3(x, 0, y);
                //Debug.DrawLine(spawnPosition, new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z), Color.red);


                Vector3 centerOffset = new Vector3(transform.localScale.x * 5, 0, transform.localScale.z * 5);


                GameObject newObj = Instantiate(spawnObj);
                newObj.transform.parent = parentObj.transform;
                newObj.transform.localScale = Vector3.one / 2;
                newObj.transform.localPosition = spawnPosition - centerOffset;

            }
        }
    }
}
