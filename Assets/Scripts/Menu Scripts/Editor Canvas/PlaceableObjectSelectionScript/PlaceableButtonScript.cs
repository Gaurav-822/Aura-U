using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableButtonScript : MonoBehaviour
{
    public Button myButton;
    public int newObjectId = 0;

    private GridBuildingSystem gridBuildingSystem;

    void Start()
    {
        gridBuildingSystem = GameObject.FindWithTag("GridBuildingSystem").GetComponent<GridBuildingSystem>();
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        gridBuildingSystem.ChangePlaceableObject(newObjectId);
    }

    void Update()
    {

    }
}
