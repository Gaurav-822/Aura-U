using FlutterUnityIntegration;  // Ensure this namespace matches your Unity-Flutter integration plugin
using UnityEngine;

public class SendGridData : MonoBehaviour
{
    private UnityMessageManager messageManager;

    void Start()
    {
        // Get reference to UnityMessageManager
        messageManager = GetComponent<UnityMessageManager>();
    }

    // Method to send custom data to Flutter
    public void SendCustomDataToFlutter(string customData)
    {
        // Example: Send custom data to Flutter
        Debug.Log(customData);
        messageManager.SendMessageToFlutter(customData);
    }

    
}
