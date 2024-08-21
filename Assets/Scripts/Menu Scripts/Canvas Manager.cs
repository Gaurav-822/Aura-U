using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Canvas editorCanvas;
    [SerializeField] private Canvas playerCanvas;

    void Start()
    {
        editorCanvas.gameObject.SetActive(true);
        playerCanvas.gameObject.SetActive(false);
    }

    

    

    public void ActivateEditorCanvas()
    {
        
        editorCanvas.gameObject.SetActive(true);
        playerCanvas.gameObject.SetActive(false);
    }

    public void ActivatePlayerCanvas()
    {

        editorCanvas.gameObject.SetActive(false);
        playerCanvas.gameObject.SetActive(true);
    }
}
