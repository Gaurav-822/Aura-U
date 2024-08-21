using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public Vector2 normalisedMousePosition;
    public float currentAngle;
    public int selection;
    public int previousSelection;

    public GameObject[] menuItems;

    private MenuItemScript menuItemSc;
    private MenuItemScript previousMenuItemSc;

    private GridBuildingSystem gridBuildingSystem;


    // Start is called before the first frame update
    void Start()
    {
        gridBuildingSystem = GameObject.FindWithTag("GridBuildingSystem").GetComponent<GridBuildingSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        normalisedMousePosition = new Vector2(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2);
        currentAngle = Mathf.Atan2(normalisedMousePosition.y, normalisedMousePosition.x)*Mathf.Rad2Deg;

        currentAngle = (currentAngle + 360 - 45) % 360;

        selection = (int)currentAngle / 90;

        Debug.Log(selection);

        if (selection != previousSelection)
        {
            previousMenuItemSc = menuItems[previousSelection].GetComponent<MenuItemScript>();
            previousMenuItemSc.Deselect();
            previousSelection = selection;
            menuItemSc = menuItems[selection].GetComponent<MenuItemScript>();
            menuItemSc.Select();
        }
    }

    public void setPlaceableObject()
    {
        gridBuildingSystem.ChangePlaceableObject(selection);
    }
}
