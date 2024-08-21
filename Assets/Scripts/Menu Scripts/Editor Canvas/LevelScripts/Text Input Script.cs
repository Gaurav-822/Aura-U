using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextInputScript : MonoBehaviour
{
    [SerializeField] private Button Accept;
    [SerializeField] private Button Reject;
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        Hide();

        // Ensure Reject button is assigned
        if (Reject != null)
        {
            // Add listener for button click
            Reject.onClick.AddListener(OnRejectButtonClick);
        }
        else
        {
            Debug.LogError("Reject button is not assigned in the inspector!");
        }

        if (Accept != null)
        {
            Accept.onClick.AddListener(OnAcceptButtonClick);
        }

        // Ensure TMP_InputField is assigned
        if (inputField != null)
        {
            // Force mobile devices to open keyboard on input field activation
            inputField.keyboardType = TouchScreenKeyboardType.Default;
            inputField.characterValidation = TMP_InputField.CharacterValidation.None; // or as needed
        }
        else
        {
            Debug.LogError("TMP_InputField is not assigned in the inspector!");
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (inputField != null)
        {
            inputField.ActivateInputField(); // Activate the input field and open keyboard
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (inputField != null)
        {
            inputField.DeactivateInputField(); // Deactivate the input field to hide the keyboard
        }
    }

    private void OnRejectButtonClick()
    {
        // This method will be called when Reject button is clicked
        Hide();
    }

    private void OnAcceptButtonClick()
    {
        string newItemName = GetInputFieldText();
        if (!string.IsNullOrEmpty(newItemName))
        {
            DynamicText dynamicTextScript = FindObjectOfType<DynamicText>(); // Assuming DynamicText is in the scene
            if (dynamicTextScript != null)
            {
                dynamicTextScript.AddNewText(newItemName);
            }
        }
        // Start the coroutine to hide the UI after a delay
        StartCoroutine(HideAfterDelay(0.5f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Hide the UI
        Hide();
    }

    public string GetInputFieldText()
    {
        if (inputField != null)
        {
            return inputField.text;
        }
        else
        {
            Debug.LogError("TMP_InputField is not assigned in the inspector!");
            return null;
        }
    }
}
