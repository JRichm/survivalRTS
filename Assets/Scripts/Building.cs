using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Building : MonoBehaviour {

    [SerializeField] public string buildingName;
    [SerializeField] public string buildingDescription;
    [SerializeField] public int buildingPrice;
    [SerializeField] public int priceType;
    [SerializeField] public Sprite buildingImage;
    [SerializeField] public Vector3 centerOffset;
    [SerializeField] GameObject selectionBox;
    [SerializeField] Material mSelected;
    [SerializeField] Material mTransparent;

    private bool active;

    private void Start() {
        active = false;
    }

    public void SetActive() {
        active = true;
        selectionBox.GetComponent<MeshRenderer>().enabled = false;
    }
}
