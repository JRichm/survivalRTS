using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject[] menuTabs;
    [SerializeField] private GameObject[] menuButtons;


    private void Start() {
        SwitchMenuTabs("Map");

        foreach (GameObject button in menuButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(delegate { SwitchMenuTabs(button.GetComponentInChildren<TMP_Text>().text); });
        }
    }

    private void SwitchMenuTabs(string tabName) {
        Debug.Log(tabName);
        foreach (GameObject tabObject in menuTabs) {
            tabObject.SetActive(false);
        }
        foreach (GameObject buttonObj in menuButtons) {
            buttonObj.GetComponent<Image>().color = new Color(0.196f, 0.196f, 0.196f);
            buttonObj.GetComponentInChildren<TMP_Text>().color = Color.white;
        }

        switch (tabName) {

            case "World":
                menuTabs[0].SetActive(true);
                menuButtons[0].GetComponent<Image>().color = Color.white;
                menuButtons[0].GetComponentInChildren<TMP_Text>().color = Color.black;
                break;
            case "Player":
                menuTabs[1].SetActive(true);
                menuButtons[1].GetComponent<Image>().color = Color.white;
                menuButtons[1].GetComponentInChildren<TMP_Text>().color = Color.black;
                break;
        }
    }
}
