using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Load : MonoBehaviour
{
    public Button loadButton;

    private GridBuildingSystem gridBuildingSystem;

    void Start()
    {
        gridBuildingSystem = GameObject.FindWithTag("GridBuildingSystem").GetComponent<GridBuildingSystem>();
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        gridBuildingSystem.LoadGridState(gridBuildingSystem.savedGridState);
    }
}
