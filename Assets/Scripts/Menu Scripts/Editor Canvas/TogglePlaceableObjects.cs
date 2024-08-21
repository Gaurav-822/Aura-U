using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePlaceableObjects : MonoBehaviour
{
    public GameObject uiElement; // The UI element to show/hide
    private bool isUIVisible = true; // Tracks the current visibility state

    private void Start()
    {
        ToggleUIVisibility();
    }

    public void ToggleUIVisibility()
    {
        isUIVisible = !isUIVisible; // Toggle the visibility state
        uiElement.SetActive(isUIVisible); // Show or hide the UI element based on the state
    }
}
