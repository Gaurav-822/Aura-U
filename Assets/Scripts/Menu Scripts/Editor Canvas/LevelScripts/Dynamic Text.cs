using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using static GridBuildingSystem;
using System;
using System.Collections;

public class DynamicText : MonoBehaviour
{
    public GameObject textPrefab;  // Assign the TextPrefab in the Inspector
    public Transform content;      // Assign the Content GameObject of the Scroll View in the Inspector
    public Button previousButton;  // Assign the PreviousButton in the Inspector
    public Button nextButton;      // Assign the NextButton in the Inspector
    public RawImage nextButtonRawImage;  // Assign the RawImage component on the NextButton in the Inspector
    public Texture nextTexture;    // Assign the "Next" texture in the Inspector
    public Texture addTexture;     // Assign the "Add" texture in the Inspector
    public Color highlightColor = Color.yellow; // Color to highlight the selected text
    public TextInputScript textInputScript;
    public GridBuildingSystem gridBuildingSystem;
    public RawImage removeTextImage;  // Serialized RawImage for removing text

    private GameObject selectedText = null;
    private List<GameObject> textObjects = new List<GameObject>(); // To store references to instantiated text objects
    private int currentIndex = 0;
    private Dictionary<string, GridState> gridStates = new Dictionary<string, GridState>();

    [SerializeField] private PlayerCameraScript playerCameraScript;

    // This is the list of strings you want to display
    public List<string> strings = new List<string>
    {
    };

    void Start()
    {
        PopulateScrollView();
        HighlightDefaultText();
        SetupNavigationButtons();
        UpdateNextButtonImage();

        // Setup remove text image
        if (removeTextImage != null)
        {
            removeTextImage.GetComponent<Button>().onClick.AddListener(OnRemoveTextImageClicked);
        }
        else
        {
            Debug.LogError("RemoveTextImage RawImage is not assigned.");
        }
        AddNewText("GROUND");

    }

    void PopulateScrollView()
    {
        if (textPrefab == null)
        {
            Debug.LogError("TextPrefab is not assigned.");
            return;
        }

        if (content == null)
        {
            Debug.LogError("Content is not assigned.");
            return;
        }

        foreach (string str in strings)
        {
            // Instantiate a new text object
            GameObject newText = Instantiate(textPrefab, content);

            // Set the text to the string from the list
            Text textComponent = newText.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = str;
            }
            else
            {
                Debug.LogError("Text component not found on the instantiated prefab.");
            }

            // Add click event listener
            Button buttonComponent = newText.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => OnTextClicked(newText));
            }
            else
            {
                Debug.LogError("Button component not found on the instantiated prefab.");
            }

            // Store reference to the instantiated text object
            textObjects.Add(newText);
        }
    }

    void HighlightDefaultText()
    {
        if (textObjects.Count > 0)
        {
            OnTextClicked(textObjects[0]);
        }
    }

    void OnTextClicked(GameObject clickedText)
    {
        // Save the current grid state
        if (selectedText != null)
        {
            Text selectedTextComponent = selectedText.GetComponentInChildren<Text>();
            if (selectedTextComponent != null)
            {
                selectedTextComponent.color = Color.white; // Reset color to default

                // Save the grid state
                gridStates[selectedTextComponent.text] = gridBuildingSystem.SaveGridState();
            }
        }

        // Deselect previous text
        if (selectedText != null)
        {
            Text selectedTextComponent = selectedText.GetComponentInChildren<Text>();
            if (selectedTextComponent != null)
            {
                selectedTextComponent.color = Color.white; // Reset color to default
            }
        }

        // Select new text
        selectedText = clickedText;
        Text clickedTextComponent = selectedText.GetComponentInChildren<Text>();
        if (clickedTextComponent != null)
        {
            clickedTextComponent.color = highlightColor; // Set color to highlight

            // Load the grid state if it exists
            if (gridStates.TryGetValue(clickedTextComponent.text, out GridState gridState))
            {
                gridBuildingSystem.LoadGridState(gridState);
            }
            else
            {
                gridBuildingSystem.EmptyGrid();
            }
        }

        // Update current index
        currentIndex = textObjects.IndexOf(clickedText);
        UpdateNextButtonImage();
        playerCameraScript.MoveToSpawnPoint();
    }

    void SetupNavigationButtons()
    {
        if (previousButton != null)
        {
            previousButton.onClick.AddListener(OnPreviousButtonClicked);
        }
        else
        {
            Debug.LogError("PreviousButton is not assigned.");
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
        else
        {
            Debug.LogError("NextButton is not assigned.");
        }
    }

    void OnPreviousButtonClicked()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            OnTextClicked(textObjects[currentIndex]);
        }
    }

    void OnNextButtonClicked()
    {
        if (currentIndex < textObjects.Count - 1)
        {
            currentIndex++;
            OnTextClicked(textObjects[currentIndex]);
        }
        else
        {
            if (textInputScript == null)
            {
                Debug.LogError("TextInputScript is not assigned.");
                return;
            }

            textInputScript.Show();
        }
    }

    public void AddNewText(string newItemName)
    {
        /*
        for (int i = 0; i < strings.Count; i++) {
            if (strings[i] == newItemName)
            {
                return;
            }
        }
        */

        // Instantiate a new text object
        GameObject newText = Instantiate(textPrefab, content);

        // Set the text to the new string
        Text textComponent = newText.GetComponentInChildren<Text>();
        if (textComponent != null)
        {
            textComponent.text = newItemName;
        }
        else
        {
            Debug.LogError("Text component not found on the instantiated prefab.");
            return;
        }

        // Add click event listener
        Button buttonComponent = newText.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnTextClicked(newText));
        }
        else
        {
            Debug.LogError("Button component not found on the instantiated prefab.");
            return;
        }

        // Store reference to the instantiated text object
        textObjects.Add(newText);

        // Add the new string to the list of strings
        strings.Add(newItemName);

        // Update the index and select the new text
        currentIndex = textObjects.Count - 1;
        OnTextClicked(newText);
    }

    void UpdateNextButtonImage()
    {
        if (nextButtonRawImage != null)
        {
            RectTransform rectTransform = nextButtonRawImage.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (currentIndex >= textObjects.Count - 1)
                {
                    nextButtonRawImage.texture = addTexture;
                    rectTransform.sizeDelta = new Vector2(100, 100); // Set to 50x50 for "Add" image
                }
                else
                {
                    nextButtonRawImage.texture = nextTexture;
                    rectTransform.sizeDelta = new Vector2(100, 100); // Set to 100x100 for "Next" image
                }
            }
        }
        else
        {
            Debug.LogError("RawImage component not found on the NextButton.");
        }
    }

    // Function to log the gridStates dictionary as JSON
    public string GetGridStatesAsJson()
    {
        AddNewText("END");
        string json = JsonConvert.SerializeObject(gridStates, Formatting.Indented);
        DeleteAll();
        return json;
    }

    public void LoadGridStatesFromJson(string json)
    {
        try
        {
            // Deserialize the JSON string into a dictionary of GridState objects
            Dictionary<string, GridState> loadedGridStates = JsonConvert.DeserializeObject<Dictionary<string, GridState>>(json);

            if (loadedGridStates == null)
            {
                Debug.LogError("Failed to deserialize JSON. The JSON may be malformed or does not match the GridState structure.");
                return;
            }

            // Clear current grid states, UI elements, and text objects
            gridStates.Clear();
            textObjects.Clear();
            strings.Clear();

            // Destroy all instantiated text objects
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            // Assign loaded grid states to the gridStates dictionary
            gridStates = loadedGridStates;

            // Update strings list with keys from the loaded grid states
            foreach (var item in gridStates)
            {
                strings.Add(item.Key);
            }

            // Repopulate the ScrollView with the updated strings
            PopulateScrollView();

            // Highlight the first text in the list, if available
            if (textObjects.Count > 0)
            {
                HighlightDefaultText();
            }
            else
            {
                selectedText = null;
                Debug.LogWarning("No text objects found after loading grid states from JSON.");
            }

            // Update the next button image to reflect the current state
            UpdateNextButtonImage();

            Debug.Log("Successfully loaded grid states from JSON.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred while loading grid states from JSON: {e.Message}");
        }
    }





    public void DeleteAll()
    {
        Debug.Log("DeleteAll function called.");

        // Destroy all instantiated text objects
        foreach (GameObject textObject in textObjects)
        {
            Destroy(textObject);
        }

        // Clear the lists and dictionary
        textObjects.Clear();
        strings.Clear();
        gridStates.Clear();

        // Reset the selectedText and currentIndex
        selectedText = null;
        currentIndex = 0;

        // Optionally, reset the UI content (if needed)
        if (content != null)
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }

        // Update the UI
        UpdateNextButtonImage();

        // Confirm deletion
        Debug.Log("All text objects deleted.");
    }




    public void RemoveText(string itemName)
    {
        // Find the text object to remove based on the itemName
        GameObject textToRemove = textObjects.Find(textObject =>
        {
            Text textComponent = textObject.GetComponentInChildren<Text>();
            return textComponent != null && textComponent.text == itemName;
        });

        // If the text object is found and it is not "GROUND", proceed to remove it
        if (textToRemove != null && itemName != "GROUND")
        {
            // Remove the text object from the list and destroy it
            textObjects.Remove(textToRemove);
            Destroy(textToRemove);

            // Remove the item from the strings list
            strings.Remove(itemName);

            // Remove the grid state associated with the item
            gridStates.Remove(itemName);

            // Update the index and select a new text
            if (currentIndex >= textObjects.Count)
            {
                currentIndex = textObjects.Count - 1;
            }

            if (textObjects.Count > 0)
            {
                OnTextClicked(textObjects[Mathf.Clamp(currentIndex, 0, textObjects.Count - 1)]);
            }
            else
            {
                selectedText = null;
                currentIndex = 0;
            }

            // Update the Next button image
            UpdateNextButtonImage();
        }
        else if (itemName == "GROUND")
        {
            Debug.LogWarning("Cannot remove 'GROUND' text.");
        }
        else
        {
            Debug.LogWarning($"No text object found with the name '{itemName}'.");
        }
    }

    void OnRemoveTextImageClicked()
    {
        if (selectedText != null)
        {
            Text selectedTextComponent = selectedText.GetComponentInChildren<Text>();
            if (selectedTextComponent != null)
            {
                RemoveText(selectedTextComponent.text);
            }
        }
    }

    public string GetSelectedText()
    {
        if (selectedText != null)
        {
            Text selectedTextComponent = selectedText.GetComponentInChildren<Text>();
            if (selectedTextComponent != null)
            {
                return selectedTextComponent.text;
            }
            else
            {
                Debug.LogError("Text component not found on the selected text object.");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("No text is currently selected.");
            return null;
        }
    }

}
