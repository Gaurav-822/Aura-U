using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera editorCam;
    [SerializeField] private CinemachineVirtualCamera playerCam;

    public bool isEditorCamActive = true;

    

    public void SwitchCameraToEditor()
    {
        editorCam.Priority = 20;
        playerCam.Priority = 10;
        Debug.Log("Switched to Editor Camera");

        isEditorCamActive=true;
    }

    public void SwitchCameraToPlayer()
    {
        editorCam.Priority = 10;
        playerCam.Priority = 20;
        Debug.Log("Switched to Player Camera");

        isEditorCamActive = false;
    }

   
}
