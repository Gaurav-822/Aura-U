using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;  // For InputField and Button components
using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private List<Transform> placedObjects;
    [SerializeField] private RawImage tagToggleButton;  // The RawImage used as a button to toggle tag mode

    private int placeObject;
    private GridXZ<GridObject> grid;
    private Vector2 touchStartPos;
    private bool isDragging;
    private bool isTagMode = false;  // Whether you are in tag mode or not
    public TagInput textInputScript;

    private RectTransform tagToggleButtonRectTransform;
    private Vector3 originalScale;
    private float scaleMultiplier = 1.5f;

    private int currentX, currentZ;
    [SerializeField] private CameraManager cameraManager;

    private void Awake()
    {
        int gridWidth = 30;
        int gridHeight = 30;
        float cellSize = 10f;
        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
        placeObject = 0;

        if (tagToggleButton != null)
        {
            tagToggleButtonRectTransform = tagToggleButton.GetComponent<RectTransform>();
            originalScale = tagToggleButtonRectTransform.localScale;
            tagToggleButton.GetComponent<Button>().onClick.AddListener(ToggleTagMode);
        }

    }



    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private Transform transform;
        private int objectIndex;
        private Dir dir;
        private string tag;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
            this.objectIndex = -1; // -1 means no object placed
            this.tag = "None";
        }

        public void SetTransform(Transform transform, int objectIndex, Dir dir)
        {
            this.transform = transform;
            this.objectIndex = objectIndex;
            this.dir = dir;
            grid.UpdateDebugTextArray();
        }

        public void ClearTransform()
        {
            if (transform != null)
            {
                GameObject.Destroy(transform.gameObject);
                transform = null;
            }
            objectIndex = -1;
            tag = "None";
            grid.UpdateDebugTextArray();
        }

        public bool CanBuild()
        {
            return transform == null;
        }

        public int GetObjectIndex()
        {
            return objectIndex;
        }

        public Dir GetDirection()
        {
            return dir;
        }

        public string GetTag()
        {
            return tag;
        }

        public void SetTag(string newTag)
        {
            tag = newTag;
            grid.UpdateDebugTextArray();
        }

        public override string ToString()
        {
            return x + ", " + z + "\n" + transform + "\nTag: " + tag;
        }
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isDragging = false;
                    break;

                case TouchPhase.Moved:
                    if (Vector2.Distance(touch.position, touchStartPos) > 10f) // Threshold for detecting drag
                    {
                        isDragging = true;
                    }
                    break;

                case TouchPhase.Ended:
                    Vector2 touchPosition = touch.position;
                    if (!isDragging && !IsPointerOverUIObject(touchPosition))
                    {
                        HandleTouch(touchPosition);
                    }
                    break;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject(Input.mousePosition))
            {
                HandleTouch(Input.mousePosition);
            }
        }

        // Rotate the object if the 'R' key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            rotateOneCycle();
        }

        // Change the Placeable Object
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            placeObject = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            placeObject = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            placeObject = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            placeObject = 3;
        }

        // Empty the grid if the 'Esc' key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EmptyGrid();
        }

        // Save the grid state if the 'S' key is pressed
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveGridState();
        }

        // Load the grid state if the 'L' key is pressed
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGridState(savedGridState);
        }
    }

    public void rotateOneCycle()
    {
        dir = GetNextDir(dir);
        rotationAngle = GetRotationAngle(dir);
    }

    public enum Dir
    {
        Down,
        Left,
        Up,
        Right,
    }

    private GridBuildingSystem.Dir dir = Dir.Down;
    private int rotationAngle = 0;

    public Dir GetNextDir(Dir currentDir)
    {
        switch (currentDir)
        {
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
            default: return Dir.Down;
        }
    }

    public int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
            case Dir.Down: return 0;
            default: return 0;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, 1);
            case Dir.Up: return new Vector2Int(1, 1);
            case Dir.Right: return new Vector2Int(1, 0);
        }
    }

    private void HandleTouch(Vector2 touchPosition)
    {
        if (cameraManager.isEditorCamActive == false) return;

        grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out currentX, out currentZ);


        if (currentZ < 0)
        {
            currentZ = 0;
        }

        if (currentX < 0)
        {
            currentX = 0;
        }

        Debug.Log($"Handling touch at ({currentX}, {currentZ})");

        GridObject gridObject = grid.GetGridObject(currentX, currentZ);

        // Add debug statements to confirm values
        Debug.Log($"GridObject at ({currentX}, {currentZ}): {gridObject}");


        Vector2Int rotationOffset = GetRotationOffset(dir);
        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(currentX, currentZ) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

        try
        {
            if (isTagMode)
            {
                if (!gridObject.CanBuild())
                {
                    Debug.Log($"Showing text input for tag at ({currentX}, {currentZ})");
                    textInputScript.Show();
                    //ChangeGridObjectTag("ExampleTag");  // Ensure you're calling ChangeGridObjectTag with the correct parameters
                }
            }
            else
            {
                if (gridObject.CanBuild())
                {
                    if (placeObject >= 0 && placeObject < placedObjects.Count)
                    {
                        Transform builtTransform = Instantiate(placedObjects[placeObject],
                            placedObjectWorldPosition,
                            Quaternion.Euler(0, rotationAngle, 0)
                        );
                        gridObject.SetTransform(builtTransform, placeObject, dir);
                    }
                    else
                    {
                        Debug.LogError("placeObject index is out of bounds.");
                    }
                }
                else
                {
                    gridObject.ClearTransform();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception in HandleTouch: {e.Message}");
        }
    }


    private void ToggleTagMode()
    {
        isTagMode = !isTagMode;

        if (tagToggleButtonRectTransform != null)
        {
            // Animate scale change
            if (isTagMode)
            {
                tagToggleButtonRectTransform.localScale = originalScale * scaleMultiplier;
            }
            else
            {
                tagToggleButtonRectTransform.localScale = originalScale;
            }
        }
    }

    public void ChangePlaceableObject(int objectId)
    {
        placeObject = objectId;
    }

    public class GridState
    {
        public List<GridObjectState> gridObjectStates = new List<GridObjectState>();
    }

    public class GridObjectState
    {
        public int x;
        public int z;
        public int objectIndex;
        public Dir dir;
        public string tag = "None";
    }

    public GridState savedGridState;

    public GridState SaveGridState()
    {
        savedGridState = new GridState();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                GridObject gridObject = grid.GetGridObject(x, z);
                if (!gridObject.CanBuild())
                {
                    GridObjectState gridObjectState = new GridObjectState
                    {
                        x = x,
                        z = z,
                        objectIndex = gridObject.GetObjectIndex(),
                        dir = gridObject.GetDirection(),
                        tag = gridObject.GetTag()  // Save the tag information
                    };
                    savedGridState.gridObjectStates.Add(gridObjectState);
                }
            }
        }

        string json = JsonConvert.SerializeObject(savedGridState, Formatting.Indented);
        Debug.Log(json);

        return savedGridState;
    }


    public void LoadGridState(GridState gridState)
    {
        EmptyGrid();

        foreach (GridObjectState gridObjectState in gridState.gridObjectStates)
        {
            GridObject gridObject = grid.GetGridObject(gridObjectState.x, gridObjectState.z);

            // Calculate the world position based on the object's orientation
            Vector2Int rotationOffset = GetRotationOffset(gridObjectState.dir);
            Vector3 worldPosition = grid.GetWorldPosition(gridObjectState.x, gridObjectState.z) +
                                    new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

            // Determine the correct rotation
            Quaternion rotation = Quaternion.Euler(0, GetRotationAngle(gridObjectState.dir), 0);

            // Debug logs to check calculated values
            Debug.Log($"Loading object at ({gridObjectState.x}, {gridObjectState.z}), Index: {gridObjectState.objectIndex}, Direction: {gridObjectState.dir}, Tag: {gridObjectState.tag}");
            Debug.Log($"Calculated World Position: {worldPosition}, Rotation: {rotation.eulerAngles}");

            Transform builtTransform = Instantiate(placedObjects[gridObjectState.objectIndex],
                worldPosition,
                rotation
            );

            gridObject.SetTransform(builtTransform, gridObjectState.objectIndex, gridObjectState.dir);
            gridObject.SetTag(gridObjectState.tag);  // Apply the saved tag
        }

        grid.UpdateDebugTextArray();
    }



    public void LoadGridStateFromJson(string json)
    {
        try
        {
            GridState gridState = JsonConvert.DeserializeObject<GridState>(json);
            LoadGridState(gridState);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }


    public void EmptyGrid()
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                GridObject gridObject = grid.GetGridObject(x, z);
                gridObject.ClearTransform();
            }
        }
    }

    private bool IsPointerOverUIObject(Vector2 position)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        return results.Count > 0;
    }

    public List<Vector3> GetPlacedObjectPositions()
    {
        List<Vector3> placedObjectPositions = new List<Vector3>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                GridObject gridObject = grid.GetGridObject(x, z);
                if (!gridObject.CanBuild()) // Object is placed
                {
                    placedObjectPositions.Add(grid.GetWorldPosition(x, z));
                }
            }
        }
        return placedObjectPositions;
    }

    public void onDot()
    {
        grid.onDotted();
    }

    public void offDot()
    {
        grid.offDotted();
    }

    public void ChangeGridObjectTag(string newTag)
    {
        // Use the stored x and z coordinates
        if (currentX >= 0 && currentX < grid.GetWidth() && currentZ >= 0 && currentZ < grid.GetHeight())
        {
            GridObject gridObject = grid.GetGridObject(currentX, currentZ);

            // Check if there's an object placed at the grid position
            if (!gridObject.CanBuild())
            {
                gridObject.SetTag(newTag);
                Debug.Log($"Tag changed to '{newTag}' at position ({currentX}, {currentZ})");
            }
            else
            {
                Debug.LogWarning($"No object placed at position ({currentX}, {currentZ}), unable to change tag.");
            }
        }
        else
        {
            Debug.LogError($"Grid position ({currentX}, {currentZ}) is out of bounds.");
        }
    }

}
