using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] GameObject hoverSquare;

    private int wood;
    private int stone;
    private int gold;

    private GameObject selectedObjectToPlace = null;

    private bool userSelecting = false;
    private Vector3 clickPosition = Vector3.zero;
    private Vector3 selectSquareSize = Vector3.one;

    private PlayerUIController playerUIController;

    private void Start() {
        playerUIController = gameObject.GetComponentInChildren<PlayerUIController>();
        wood = 500;
        playerUIController.UpdateCurrency(wood, stone, gold);
    }

    private void Update() {
        HandleMouse();
    }


    private void HandleMouse() {

        // create a ray from camera to mouse pos in world space
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        // define a layer mask to filter only the 'ground' layer
        int groundLayerMask = 3;

        // define the maximum distance for the ray
        float maxRayDistance = 1000f;

        // cast the ray and get all hits
        RaycastHit[] hits = Physics.RaycastAll(ray, maxRayDistance);

        // get mouse1 input (true/false)
        userSelecting = Input.GetMouseButton(0);

        if (selectedObjectToPlace && Input.GetKeyDown(KeyCode.R)) RotatePlaceSelection();
        if (selectedObjectToPlace && Input.GetMouseButtonDown(0)) PlaceSelection();

        // find the first hit on the 'ground' layer
        foreach (var hit in hits) {
            if (hit.collider.gameObject.layer == groundLayerMask) {
                // get mouse1 input (true/false)
                userSelecting = Input.GetMouseButton(0);

                // get mouse pos when user clicks
                if (Input.GetMouseButtonDown(0)) {
                    clickPosition = hit.point;
                }

                if (Input.GetMouseButtonUp(0)) {
                    ResetHoverSquare();
                }

                // Use the accurate hit position for all calculations
                Vector3 accurateMousePosition = hit.point;

                if (selectedObjectToPlace) {
                    ShowPlaceSelection(accurateMousePosition);
                } else if (userSelecting) {
                    ShowSelectionSquare(accurateMousePosition);
                } else {
                    ShowHoverSquare(accurateMousePosition);
                }

                // Break out of the loop after processing the first hit
                break;
            }
        }
    }

    private void ShowHoverSquare(Vector3 mousePosition) {
        Vector3 mouseSquarePos = GetBlockPos(mousePosition);
        hoverSquare.transform.position = mouseSquarePos;
    }

    private void ShowSelectionSquare(Vector3 mousePosition) {
        // click square pos (RED)
        Vector3 clickBlockPos = GetBlockPos(clickPosition);


        // mouse block pos (GREEN)
        Vector3 mouseSquarePos = GetBlockPos(mousePosition);

        Vector3 selectSquareScale = new Vector3(Mathf.Abs(clickBlockPos.x - mouseSquarePos.x + 1), 1, Mathf.Abs(clickBlockPos.z - mouseSquarePos.z + 1));
        Vector3 center = new Vector3(mouseSquarePos.x + ((clickBlockPos.x - mouseSquarePos.x) / 2), -0.475f, mouseSquarePos.z + ((clickBlockPos.z - mouseSquarePos.z) / 2));

        hoverSquare.transform.position = center;
        hoverSquare.transform.localScale = selectSquareScale;
    }

    private void ShowPlaceSelection(Vector3 mousePosition) {
        Building buildingComponent = selectedObjectToPlace.GetComponent<Building>();
        Vector3 mouseSquarePos = GetBlockPos(mousePosition);
        Vector3 buildingPos = new Vector3(mouseSquarePos.x + buildingComponent.centerOffset.x, 0, mouseSquarePos.z + buildingComponent.centerOffset.z);
        selectedObjectToPlace.transform.position = buildingPos;
    }

    private void ResetHoverSquare() {
        hoverSquare.transform.localScale = Vector3.one;
    }

    private Vector3 GetBlockPos(Vector3 pos) { 
        Vector3 blockPs = new Vector3(Mathf.Round(pos.x + 0.5f) - .5f, -0.475f, Mathf.Round(pos.z + 0.5f) - .5f);
        return blockPs;
    }

    public void SetPlaceSelection(GameObject objectToPlace) {
        if (objectToPlace != null) Destroy(selectedObjectToPlace);

        selectedObjectToPlace = Instantiate(objectToPlace);
        
        Debug.Log("selecting object to place:" +  selectedObjectToPlace.GetComponent<Building>().name);
    }

    private void RotatePlaceSelection() {
        if (selectedObjectToPlace != null) {
            Debug.Log("rotating object");
            selectedObjectToPlace.transform.Rotate(0, 90, 0);
        }
    }

    private void PlaceSelection() {
        if (selectedObjectToPlace != null) {

            Building building = selectedObjectToPlace.GetComponent<Building>();
            int priceType = building.priceType;
            if (priceType == 0) {
                if (wood - building.buildingPrice < 0) return;
                wood -= building.buildingPrice;
            } else if (priceType == 1) {
                if (stone - building.buildingPrice < 0) return;
                stone -= building.buildingPrice;
            } else if (priceType == 2) {
                if (gold - building.buildingPrice < 0) return;
                gold -= building.buildingPrice;
            }


            building.SetActive();
            selectedObjectToPlace = null;
            playerUIController.UpdateCurrency(wood, stone, gold);
        }
    }
}
