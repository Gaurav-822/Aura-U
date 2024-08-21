using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Security.Cryptography;

public class PlayerCameraScript : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float fieldOfViewMax = 50;
    [SerializeField] private float fieldOfViewMin = 10;
    [SerializeField] private DynamicText dynamicText;
    private Vector3 followOffset;

    private bool isTouching = false;

    public FixedJoystick joystick;
    public float SpeedMove = 2f;

    private GridBuildingSystem gridBuildingSystem;
    private List<Vector3> placedObjectPositions;
    private Vector3 minPosition;
    private Vector3 maxPosition;

    private Vector2 lastTouchPosition;

    [SerializeField] private CharacterAnimation characterAnimation;

    private Vector3 initialSpawnPoint; // Store the initial spawn point
    public SendGridData sendGridData;

    private Vector2 lastSentGridPosition; // Store the last sent grid position
    private string lastFloor;

    private void Awake()
    {
        followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
        UpdatePlacedObjectBounds();

        // Save the initial position as the spawn point
        initialSpawnPoint = transform.position;

        lastSentGridPosition = Vector2.zero; // Initialize the last sent grid position
        lastFloor = dynamicText.GetSelectedText();
    }

    void Update()
    {
        UpdatePlacedObjectBounds();
        CameraMovement();
        CameraRotation();

        // Example usage
        float gridCellSize = 10f; // Replace with your actual grid cell size
        Vector2 gridPosition = GetCurrentGridPosition(transform.position, gridCellSize);
        //Debug.Log("Current Grid Position: " + gridPosition.ToString());

        // Only send data if the grid position has changed
        if (cinemachineVirtualCamera.Priority == 20 && (gridPosition != lastSentGridPosition || dynamicText.GetSelectedText() != lastFloor))
        {
            sendGridData.SendCustomDataToFlutter("[" + gridPosition.ToString() + ", " + dynamicText.GetSelectedText() + "]");
            lastSentGridPosition = gridPosition; // Update the last sent grid position
            lastFloor = dynamicText.GetSelectedText();
        }
    }

    private void CameraMovement()
    {
        Vector3 moveDir = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        if (moveDir.magnitude > 0.1f) // Only rotate if there is significant input
        {
            // Get the camera's forward direction in the horizontal plane (x-z plane)
            Vector3 cameraForward = cinemachineVirtualCamera.transform.forward;
            cameraForward.y = 0; // Zero out the y component to work in the x-z plane
            cameraForward.Normalize();

            // Calculate the movement direction relative to the camera
            Vector3 moveDirectionRelativeToCamera = cameraForward * moveDir.z + cinemachineVirtualCamera.transform.right * moveDir.x;
            moveDirectionRelativeToCamera.y = 0; // Keep the movement on the horizontal plane
            moveDirectionRelativeToCamera.Normalize();

            // Move the player in the direction they are facing
            Vector3 newPosition = transform.position + moveDirectionRelativeToCamera * Time.deltaTime * SpeedMove;

            // Assuming the size of each grid cell is known, say gridCellSize
            float gridCellSize = 10f; // Replace with your actual grid cell size

            // Variable to check if the position update is allowed
            bool positionUpdated = false;

            // Check if the new position is over any placed object position
            foreach (Vector3 pos in placedObjectPositions)
            {
                Vector3 gridCenterPosition = pos + new Vector3(gridCellSize / 2, 0, gridCellSize / 2);

                if (Mathf.Abs(newPosition.x - gridCenterPosition.x) < gridCellSize / 2 &&
                    Mathf.Abs(newPosition.z - gridCenterPosition.z) < gridCellSize / 2)
                {
                    transform.position = newPosition;
                    positionUpdated = true;

                    break; // Exit the loop once the position is updated
                }
            }

            // Set animation states based on movement
            if (positionUpdated)
            {
                characterAnimation.moveCharacter();
            }
        }
        else
        {
            characterAnimation.idleCharacter();
        }
    }

    private bool isRotating = false;
    private Quaternion targetRotation;

    private void CameraRotation()
    {
        if (Input.touchCount == 1 && !isRotating)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 delta = touch.deltaPosition;
                float rotationSpeed = 0.25f; // Adjust this value to control the rotation speed

                // Calculate the new rotation based on the delta position
                float desiredRotation = delta.x * rotationSpeed;
                Quaternion currentRotation = transform.rotation;

                // Create a target rotation by rotating around the Y axis
                Quaternion incrementalRotation = Quaternion.Euler(0, desiredRotation, 0);
                targetRotation = currentRotation * incrementalRotation;

                // Smoothly interpolate towards the target rotation
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * 10f);

                // Update the last touch position for continuous rotation
                lastTouchPosition = touch.position;

                // Threshold for detecting a top-to-bottom drag
                float verticalDragThreshold = 25f; // Adjust this value to set sensitivity

                // Check if the touch is a significant top-to-bottom drag
                if (delta.y < -verticalDragThreshold && Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                {
                    // Trigger smooth 180-degree rotation
                    targetRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
                    isRotating = true; // Start smooth rotation
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }

        // Smoothly rotate towards the target rotation if in rotation state
        if (isRotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f); // Adjust speed as needed

            // Stop rotating when the rotation is nearly complete
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation; // Snap to final rotation
                isRotating = false;
            }
        }
    }

    private void UpdatePlacedObjectBounds()
    {
        placedObjectPositions = gridBuildingSystem.GetPlacedObjectPositions();

        if (placedObjectPositions.Count > 0)
        {
            minPosition = placedObjectPositions[0];
            maxPosition = placedObjectPositions[0];

            foreach (Vector3 pos in placedObjectPositions)
            {
                minPosition = Vector3.Min(minPosition, pos);
                maxPosition = Vector3.Max(maxPosition, pos);
            }
        }
        else
        {
            minPosition = new Vector3(14, 10, 0);
            maxPosition = new Vector3(14, 10, 0);
        }
    }

    // Function to move the camera back to the initial spawn point
    public void MoveToSpawnPoint()
    {
        transform.position = initialSpawnPoint;
    }

    private Vector2 GetCurrentGridPosition(Vector3 playerPosition, float gridCellSize)
    {
        // Calculate the grid cell coordinates based on the player's position
        float x = Mathf.Round((playerPosition.x + 5) / gridCellSize) * gridCellSize;
        float z = Mathf.Round((playerPosition.z + 5) / gridCellSize) * gridCellSize;

        // Return the grid position as a Vector3
        return new Vector2((x / 10) - 1, (z / 10) - 1);
    }

    // for later purpose
    public void JumpToPosition(float x, float z)
    {
        // Calculate the new position using the provided x and z, and keep the current y
        Vector3 newPosition = new Vector3(x, transform.position.y, z);

        // Set the player's position to the new position
        transform.position = newPosition;

        // Optionally, you can update the lastSentGridPosition and lastFloor here if necessary
        float gridCellSize = 10f; // Use your actual grid cell size
        lastSentGridPosition = GetCurrentGridPosition(newPosition, gridCellSize);
        lastFloor = dynamicText.GetSelectedText();
    }
}
