using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Runtime.CompilerServices;

public class GridXZ<TGridObject>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private bool dotted = true;
    private TextMesh[,] debugTextArray1;
    private TextMesh[,] debugTextArray2;

    public GridXZ(int width, int height, float cellSize, Vector3 originPosition, Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];
        debugTextArray1 = new TextMesh[width, height];
        debugTextArray2 = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        bool showDebug = true;

        if (showDebug)
        {

            setTagText();


            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);


            /*
            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z]?.ToString();
            };
            */
        }
    }

    private void setTagText()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                TGridObject gridObject = gridArray[x, z];

                debugTextArray1[x, z] = UtilsClass.CreateWorldText(
                    dotted ? "." : string.Empty,
                    null,
                    GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * 0.5f, // Adjusted Y-coordinate by adding 10 units
                    100,
                    Color.white,
                    TextAnchor.MiddleCenter,
                    TextAlignment.Center
                );

                Color orange = new Color(1f, 0.647f, 0f);

                if (gridObject is GridBuildingSystem.GridObject gridObjWithTag)
                {
                    string temp = gridObjWithTag.GetTag();
                    if (temp == "None") temp = "";

                    debugTextArray2[x, z] = UtilsClass.CreateWorldText(
                    dotted ? temp : string.Empty,
                    null,
                    GetWorldPosition(x, z) + new Vector3(cellSize, 12.5f, cellSize) * 0.5f, // Adjusted Y-coordinate by adding 10 units
                    60,
                    orange,
                    TextAnchor.MiddleCenter,
                    TextAlignment.Center
                );
                }

                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
            }
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
        }
    }

    

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        return new Vector2Int(
            Mathf.Clamp(gridPosition.x, 0, width - 1),
            Mathf.Clamp(gridPosition.y, 0, height - 1)
        );
    }

    public void onDotted()
    {
        dotted = true;
        UpdateDebugTextArray();
    }

    public void offDotted()
    {
        dotted = false;
        UpdateDebugTextArray();
    }

    public void UpdateDebugTextArray()
    {
        if (debugTextArray2 == null) return;

        for (int x = 0; x < debugTextArray2.GetLength(0); x++)
        {
            for (int z = 0; z < debugTextArray2.GetLength(1); z++)
            {
                if (dotted == false)
                {
                    debugTextArray1[x, z].text = "";
                }
                else
                {
                    debugTextArray1[x, z].text = ".";
                }
                if (debugTextArray2[x, z] != null)
                {
                    TGridObject gridObject = gridArray[x, z];
                    if (gridObject is GridBuildingSystem.GridObject gridObjWithTag)
                    {
                        if (gridObjWithTag.GetTag() != "None") debugTextArray2[x, z].text = gridObjWithTag.GetTag();
                        else debugTextArray2[x, z].text = string.Empty;
                    }
                    else
                    {
                        debugTextArray2[x, z].text = string.Empty;
                    }
                }
            }
        }
    }
}
