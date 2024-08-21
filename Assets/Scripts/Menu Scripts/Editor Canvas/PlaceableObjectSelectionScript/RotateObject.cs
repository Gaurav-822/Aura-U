using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateObject : MonoBehaviour
{
    public Text uiText;

    private string[] messages = { "0°", "90°", "180°", "270°" };
    public int currentMessageIndex = 0;
    private GridBuildingSystem gridBuildingSystem;

    void Start()
    {
        gridBuildingSystem = GameObject.FindWithTag("GridBuildingSystem").GetComponent<GridBuildingSystem>();
        Button button = uiText.GetComponent<Button>();
        button.onClick.AddListener(ChangeText);

        // Initialize the text to "0°"
        uiText.text = messages[currentMessageIndex];
    }

    void ChangeText()
    {
        // rotate the object to be placed in the grid
        gridBuildingSystem.rotateOneCycle();

        // increment index and wrap around if necessary
        currentMessageIndex = (currentMessageIndex + 1) % messages.Length;

        // change text
        uiText.text = messages[currentMessageIndex];
    }
}
