using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public InputSettings inputSettings;

    private void Awake()
    {
        Debug.Log("SettingsManager Awake called");
        // Ensure that only one instance of SettingsManager exists in the scene
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another instance of SettingsManager already exists. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
