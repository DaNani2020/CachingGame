using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;
using Wave.OpenXR;
using Wave.Essence.Events;
using Wave.Native;
using Wave.XR.Settings;
using Wave.Essence.Tracker;
using System.Runtime.InteropServices;
using System;


public class Logger : MonoBehaviour
{
    private string url = "http://192.168.178.24:8080/log"; // home ip
    //private string url = "http://10.20.78.192:8080/log"; // office ip - Headset ip
    //private string url = "http://10.20.78.102:8080/log"; // office ip - Laptop ip
    //public GameObject[] tracker;
    private bool isLogging = false; // Flag to check if logging coroutine is running


    void Start()
    {
        // Example of sending a log at the start
        StartCoroutine(SendLog("Application started"));
    }

    void Update()
    {
        if (!isLogging)
        {
            // Start the coroutine that handles the logging with delay
            //StartCoroutine(LogTrackerPositions());
        }

    }
    /*
    IEnumerator LogTrackerPositions()
    {
        isLogging = true; // Set the flag to indicate the coroutine is running
        Debug.Log("Starting LogTrackerPositions coroutine");

        for (int i = 0; i < tracker.Length; i++)
        {
            if (tracker[i] != null)
            {
                yield return StartCoroutine(SendLog($"{tracker[i].name} position: {tracker[i].transform.position}"));
            }
            else
            {
                StartCoroutine(SendLog($"Tracker {i} is null"));
            }
            
            yield return StartCoroutine(SendLog($"{tracker[i].name} data sent."));
        }

        StartCoroutine(SendLog("Finished LogTrackerPositions coroutine"));
        isLogging = false; // Reset the flag to indicate the coroutine has finished
    }
    */

    IEnumerator SendLog(string logMessage)
    {
        // Erstellen der Log-Daten
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(logMessage);

        // Erstellen der Anfrage
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        // request.SetRequestHeader("InsecureHttpOption", "AlwaysAllowed");

        // Senden der Anfrage und Warten auf Antwort
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Log sent successfully");
        }
    }

    IEnumerator sleepTimer(float sleepBetweenLogs = 0.5f)
    {
        yield return new WaitForSeconds(sleepBetweenLogs); // Delay between logs
    }
}
