using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Save : MonoBehaviour
{
    public Button saveButton;
    public SendGridData sendGridData;

    public DynamicText dynamicText;

    void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        string res = dynamicText.GetGridStatesAsJson();
        sendGridData.SendCustomDataToFlutter(res);
    }
}
