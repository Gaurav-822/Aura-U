using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorCameraScript : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float fieldOfViewMax = 50;
    [SerializeField] private float fieldOfViewMin = 10;
    [SerializeField] private RawImage rotationToggle;  // Reference to the RawImage acting as a toggle switch
    private Vector3 followOffset;
    private Vector3 initialPosition;  // Variable to store the initial position

    private Vector2 lastTouchPosition;
    private bool isTouching = false;
    private bool isRotatingToggle = false;

    private Vector3 originalScale;

    private void Awake()
    {
        followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        rotationToggle.GetComponent<Button>().onClick.AddListener(ToggleRotationAndZoom);
        originalScale = rotationToggle.rectTransform.localScale; // Store the original scale of the RawImage

        initialPosition = transform.position; // Store the initial position of the camera
        ResetToInitialPosition(); // Set the camera to the initial position
    }

    private void ToggleRotationAndZoom()
    {
        isRotatingToggle = !isRotatingToggle;

        // Scale the RawImage based on the toggle state
        if (isRotatingToggle)
        {
            rotationToggle.rectTransform.localScale = originalScale * 1.5f;
        }
        else
        {
            rotationToggle.rectTransform.localScale = originalScale;
        }
    }

    void Update()
    {
        if (isRotatingToggle)
        {
            CameraRotation();
        }
        else
        {
            CameraMovement();
        }
        CameraZoom();
    }

    private void CameraMovement()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 50f;
        transform.position += moveDir * Time.deltaTime * moveSpeed;

        // Phone input for movement
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (IsPointerOverUIObject(touch)) return;

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 delta = touch.deltaPosition;
                Vector3 phoneMoveDir = new Vector3(-delta.x, 0, -delta.y) * 0.1f;
                transform.position += transform.forward * phoneMoveDir.z + transform.right * phoneMoveDir.x;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
    }

    private void CameraRotation()
    {
        if (Input.touchCount == 1)
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
                float rotationSpeed = 0.1f;
                transform.Rotate(Vector3.up, -delta.x * rotationSpeed);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
    }

    private void CameraZoom()
    {
        // Just for PC
        float zoomAmount = 3f;

        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += zoomAmount;
        }

        // Pinch to zoom for touch screen
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            // Find the magnitude of the vector (distance) between the touches in each frame.
            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Zoom based on the difference in distance
            followOffset.y += deltaMagnitudeDiff * 0.1f;
        }

        // Clamp the Zoom
        followOffset.y = Mathf.Clamp(followOffset.y, fieldOfViewMin - 15, float.PositiveInfinity);

        float zoomSpeed = 5f;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
    }

    private bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(touch.position.x, touch.position.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    public void ResetToInitialPosition()
    {
        transform.position = initialPosition;
    }
}
