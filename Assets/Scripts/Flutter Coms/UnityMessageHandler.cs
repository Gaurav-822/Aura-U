using FlutterUnityIntegration;
using System.Collections;
using UnityEngine;

public class UnityMessageHandler : MonoBehaviour
{
    [SerializeField]private CameraManager cameraManager;
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private DynamicText dynamicText;
    [SerializeField] private GridBuildingSystem gridBuildingSystem;
    [SerializeField] private EditorCameraScript editorCamera;
    [SerializeField] private PlayerCameraScript playerCamera;

    void Start()
    {
        UnityMessageManager.Instance.OnMessage += OnUnityMessage;
    }

    // for testing only
    private void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            
            playerCamera.MoveToSpawnPoint();
            editorCamera.ResetToInitialPosition();
            gridBuildingSystem.EmptyGrid();
            dynamicText.DeleteAll();
            cameraManager.SwitchCameraToEditor();
            canvasManager.ActivateEditorCanvas();
            gridBuildingSystem.onDot();
            

        }
        if (Input.GetKey(KeyCode.V))
        {

            string check = $"{{\r\n  \"GROUND\": {{\r\n    \"gridObjectStates\": [\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 0,\r\n        \"objectIndex\": 1,\r\n        \"dir\": 0,\r\n        \"tag\": \"start of ground floor \"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 1,\r\n        \"objectIndex\": 1,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 2,\r\n        \"objectIndex\": 1,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 3,\r\n        \"objectIndex\": 3,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 4,\r\n        \"objectIndex\": 2,\r\n        \"dir\": 0,\r\n        \"tag\": \"Sam lives here\"\r\n      }},\r\n      {{\r\n        \"x\": 15,\r\n        \"z\": 3,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 1,\r\n        \"tag\": \"stair to 1st floor\"\r\n      }},\r\n      {{\r\n        \"x\": 15,\r\n        \"z\": 4,\r\n        \"objectIndex\": 3,\r\n        \"dir\": 1,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 16,\r\n        \"z\": 3,\r\n        \"objectIndex\": 2,\r\n        \"dir\": 2,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 16,\r\n        \"z\": 4,\r\n        \"objectIndex\": 2,\r\n        \"dir\": 1,\r\n        \"tag\": \"Juliet lives here\"\r\n      }}\r\n    ]\r\n  }},\r\n  \"1st floor\": {{\r\n    \"gridObjectStates\": [\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 0,\r\n        \"objectIndex\": 3,\r\n        \"dir\": 0,\r\n        \"tag\": \"start of 1st floor \"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 1,\r\n        \"objectIndex\": 2,\r\n        \"dir\": 0,\r\n        \"tag\": \"Gugu lives here\"\r\n      }},\r\n      {{\r\n        \"x\": 15,\r\n        \"z\": 0,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 1,\r\n        \"tag\": \"stair to 2nd floor\"\r\n      }},\r\n      {{\r\n        \"x\": 15,\r\n        \"z\": 1,\r\n        \"objectIndex\": 2,\r\n        \"dir\": 1,\r\n        \"tag\": \"None\"\r\n      }}\r\n    ]\r\n  }},\r\n  \"2nd floor\": {{\r\n    \"gridObjectStates\": [\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 0,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"start of 2nd floor\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 1,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 15,\r\n        \"z\": 0,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 15,\r\n        \"z\": 1,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"end of building \"\r\n      }}\r\n    ]\r\n  }}\r\n}}";
            SwitchToPlayer(check);
            
        }
    }

    private void SwitchToPlayer(string check)
    {

        StartCoroutine(ResetAndDelay(check));
    }

    private IEnumerator ResetAndDelay(string check)
    {
        cameraManager.SwitchCameraToPlayer();
        canvasManager.ActivatePlayerCanvas();
        gridBuildingSystem.offDot();
        playerCamera.MoveToSpawnPoint();
        editorCamera.ResetToInitialPosition();

        yield return new WaitForSeconds(0.0001f);

        dynamicText.DeleteAll();
        gridBuildingSystem.EmptyGrid();
        //string check = $"{{\r\n  \"ground\": {{\r\n    \"gridObjectStates\": [\r\n      {{\r\n        \"x\": 12,\r\n        \"z\": 2,\r\n        \"objectIndex\": 1,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 12,\r\n        \"z\": 3,\r\n        \"objectIndex\": 2,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 12,\r\n        \"z\": 4,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 13,\r\n        \"z\": 2,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 13,\r\n        \"z\": 4,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 0,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"start of ground\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 1,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 2,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 4,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"end of ground\"\r\n      }}\r\n    ]\r\n  }},\r\n  \"1st floor\": {{\r\n    \"gridObjectStates\": [\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 0,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"start of 1st floor\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 1,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 2,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 14,\r\n        \"z\": 3,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 15,\r\n        \"z\": 3,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 16,\r\n        \"z\": 1,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"destination \"\r\n      }},\r\n      {{\r\n        \"x\": 16,\r\n        \"z\": 2,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }},\r\n      {{\r\n        \"x\": 16,\r\n        \"z\": 3,\r\n        \"objectIndex\": 0,\r\n        \"dir\": 0,\r\n        \"tag\": \"None\"\r\n      }}\r\n    ]\r\n  }}\r\n}}";
        dynamicText.LoadGridStatesFromJson(check);
    }

    void OnDestroy()
    {
        UnityMessageManager.Instance.OnMessage -= OnUnityMessage;
    }

    public void OnUnityMessage(string message)
    {
        if (cameraManager != null)
        {
            if (message == "editor")
            {
                // if editor
                playerCamera.MoveToSpawnPoint();
                editorCamera.ResetToInitialPosition();
                gridBuildingSystem.EmptyGrid();
                dynamicText.DeleteAll();
                cameraManager.SwitchCameraToEditor();
                canvasManager.ActivateEditorCanvas();
                gridBuildingSystem.onDot();
            }
            else
            {
                // if player 
                SwitchToPlayer(message);
            }
        }
    }
}
