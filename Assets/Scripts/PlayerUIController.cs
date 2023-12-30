using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{

    [SerializeField] GameObject[] gameObjects;
    [SerializeField] GameObject content;
    [SerializeField] GameObject pfMenuButton;

    [SerializeField] TextMeshProUGUI woodCountText;
    [SerializeField] TextMeshProUGUI stoneCountText;
    [SerializeField] TextMeshProUGUI goldCountText;

    // Start is called before the first frame update
    void Start()
    {
        LoadBuyMenu();
    }

    private void LoadBuyMenu() {

        Vector3 itemPos = new Vector3(0, -50, 0);

        foreach (var item in gameObjects)
        {
            Building buildingScript = item.GetComponent<Building>();

            GameObject menuItem = Instantiate(pfMenuButton);
            menuItem.GetComponent<MenuItem>().SetButtonDetails(buildingScript);
            menuItem.transform.SetParent(content.transform);
            menuItem.GetComponent<RectTransform>().transform.localPosition = itemPos;
            itemPos.y -= 75;
        }
    }

    public void UpdateCurrency(int wood, int stone, int gold) {
        woodCountText.text = wood.ToString();
        stoneCountText.text = stone.ToString();
        goldCountText.text = gold.ToString();
    }
}
