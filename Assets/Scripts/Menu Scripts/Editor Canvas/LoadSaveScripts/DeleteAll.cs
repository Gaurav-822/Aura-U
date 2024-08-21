using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeleteAll : MonoBehaviour
{
    public Button deleteButton;

    private GridBuildingSystem gridBuildingSystem;

    void Start()
    {
        gridBuildingSystem = GameObject.FindWithTag("GridBuildingSystem").GetComponent<GridBuildingSystem>();
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        gridBuildingSystem.EmptyGrid();
    }
}
