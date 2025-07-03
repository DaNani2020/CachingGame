using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Wave.Essence.InputModule;

public class PointerPosition : MonoBehaviour
{

    public LayerMask interactableLayer; // Assign the layer of the plane/UI elements
    public int childIndexofPointer = 2; // Assign the index of the pointer object in the inspector
    public GameObject dominantController; // Assign the pointer object in the inspector
    public GameObject nonDominantController; // Assign the pointer object in the inspector
    public Vector3 pointerSize = new Vector3(0.1f, 0.1f, 0.1f); // This is the size of the pointer
    public Vector3 pointerOffset = new Vector3(0f, 0f, 0f); // This is that the pointer will exactly fit the UI element such as a button

    private EventControllerSetter dominantEventControllerSetter;
    private EventControllerSetter nonDominantEventControllerSetter;
    private Transform dominantPointer = null; // Assign the pointer object in the inspector
    private Transform leftPointer = null; // Assign the pointer object in the inspector

    void Start()
    {
        // Retrieve the EventControllerSetter component
        dominantEventControllerSetter = dominantController.GetComponent<EventControllerSetter>();
        nonDominantEventControllerSetter = nonDominantController.GetComponent<EventControllerSetter>();
        dominantEventControllerSetter.activePointer = false;
        nonDominantEventControllerSetter.activePointer = false;

    }

    void Update()
    {
        if (dominantPointer == null)
        {
            dominantPointer = dominantController.transform.GetChild(childIndexofPointer);
            dominantPointer.transform.localScale = pointerSize;
            // ShowMessage("Dominant Pointer found: "+dominantPointer.name);
            
        }
        else if(leftPointer == null)
        {
            leftPointer = nonDominantController.transform.GetChild(childIndexofPointer);
            leftPointer.transform.localScale = pointerSize;
            // ShowMessage("Left Pointer found: "+leftPointer.name);
        }

        // Update the position of the dominant pointer
        SetPointerPosition(dominantPointer, leftPointer);
    }

    void SetPointerPosition(Transform dominantPointer, Transform leftPointer)
    {
        RaycastHit hit;
        if (Physics.Raycast(dominantPointer.position, dominantPointer.forward, out hit, Mathf.Infinity, interactableLayer))
        {
            // Enable the pointer
            // dominantPointer.gameObject.SetActive(true);
            dominantEventControllerSetter.activePointer = true;

            // Position the pointer at the hit point
            dominantPointer.position = hit.point; // + pointerOffset;

            // Optionally, align the pointer with the surface normal
            dominantPointer.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
        }

        if (Physics.Raycast(leftPointer.position, leftPointer.forward, out hit, Mathf.Infinity, interactableLayer))
        {   
            // Enable the pointer
            // leftPointer.gameObject.SetActive(true);
            nonDominantEventControllerSetter.activePointer = true;

            // Position the pointer at the hit point
            leftPointer.position = hit.point; // + pointerOffset;

            // Optionally, align the pointer with the surface normal
            leftPointer.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
        }
    }

        public string GetCurrentSceneName()
    {
        // Retrieve the active scene
        Scene activeScene = SceneManager.GetActiveScene();
        
        // Return the name of the active scene
        return activeScene.name;
    }

    void ShowMessage(string message)
    {
        Debug.Log(message);
        DebugText.Instance.AppendLine(message);
    }
}
