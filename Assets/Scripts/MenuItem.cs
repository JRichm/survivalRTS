using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ButtonName;
    [SerializeField] TextMeshProUGUI ButtonDescription;
    [SerializeField] TextMeshProUGUI ButtonPrice;
    [SerializeField] Image PriceTypeImage;
    [SerializeField] Image ItemImage;

    private PlayerUIController playerUIController;
    private Button itemButton;
    private GameObject itemPrefab;
    private Player playerScript;

    private void Start() {
        playerScript = gameObject.GetComponentInParent<Player>();

        playerUIController = GetComponentInParent<PlayerUIController>();
        itemButton =GetComponent<Button>();
        itemButton.onClick.AddListener(MenuItemClick);
    }

    public void SetButtonDetails(Building buildingScript) {
        ButtonName.text = buildingScript.buildingName;
        ButtonDescription.text = buildingScript.buildingDescription;
        ButtonPrice.text = buildingScript.buildingPrice.ToString();
        ItemImage.sprite = buildingScript.buildingImage;
        itemPrefab = buildingScript.gameObject;
    }
    private void MenuItemClick() {
        playerScript.SetPlaceSelection(itemPrefab);
    }
}
