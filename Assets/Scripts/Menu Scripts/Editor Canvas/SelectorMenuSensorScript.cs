using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SelectorMenuSensorScript : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private MenuScript menuScript;
    [SerializeField] private TogglePlaceableObjects togglePlaceableObjects;

    public void OnPointerDown(PointerEventData eventData)
    {
        // start coroutine to delay the setting of the item
        StartCoroutine(DelaySetPlaceableObject());
    }

    private IEnumerator DelaySetPlaceableObject()
    {
        // wait for 1 millisecond
        yield return new WaitForSeconds(0.001f);
        // set the item
        menuScript.setPlaceableObject();
        // start coroutine to delay the closing of the menu
        StartCoroutine(DelayToggleUIVisibility());
    }

    private IEnumerator DelayToggleUIVisibility()
    {
        // wait for 1 millisecond
        yield return new WaitForSeconds(0.1f);
        // close the menu
        togglePlaceableObjects.ToggleUIVisibility();
    }
}
