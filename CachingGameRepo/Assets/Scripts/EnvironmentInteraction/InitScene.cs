using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Wave.Essence.InputModule;
using Wave.Essence;
using Wave.Native;
using UnityEngine.EventSystems;
using System;

public class InitScene : MonoBehaviour
{
    [SerializeField]
    private GameObject dominantController = null;

    [SerializeField]
    private GameObject dominantTracker = null;

    [SerializeField]
    private GameObject nonDominantController = null;

    [SerializeField]
    private GameObject nonDominantTracker = null;

    [SerializeField]
    private GameObject leftWand = null;

    [SerializeField]
    private GameObject rightWand = null;

    // [SerializeField]
    // private Button startButton = null;

    // This is for disabling and activating the start button - not needed usually
    // [SerializeField]
    // private GameObject startButtonObject = null;

    [SerializeField]
    private GameObject instructionPanel = null;

    [SerializeField]
    private Button referenceStartButton = null;

    [SerializeField]
    private GameObject referenceStartButtonObject = null;

    [SerializeField]
    private GameObject referenceInstructionPanel = null;

    [SerializeField]
    private bool referenceInstructionPanelIsShown = false;

    [SerializeField]
    private GameObject conclusionPanel = null;

    [SerializeField]
    private bool conclusionPanelIsShown = false;

    [SerializeField]
    private EventSystem eventSystem;

    private ControllerInputModule controllerInputModule;

    private string playerPrefs;

    private bool spellRouteIsRunning = false;
    
    private bool isStarted = false;

    
    void Start()
    {
        playerPrefs = PlayerPrefs.GetString("SelectedArm");
        Debug.Log("PlayerPrefs in WizardScene: " + playerPrefs);

        controllerInputModule = eventSystem.GetComponent<ControllerInputModule>();

        // if (startButton != null)
        // {
        //     startButton.onClick.AddListener(StartSpellRouting);
        // }
        if (referenceStartButton != null)
        {
            referenceStartButton.onClick.AddListener(StartSpellRouting);
        }
        instructionPanel.SetActive(true);
        referenceInstructionPanel.SetActive(false);
        conclusionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if((dominantController != null || nonDominantController != null) && (dominantTracker != null || nonDominantTracker != null))
        {
            if (playerPrefs == "Left")
            {
                dominantController.SetActive(false);
                dominantTracker.SetActive(false);
                nonDominantController.SetActive(true);
                nonDominantTracker.SetActive(true);
                controllerInputModule.DominantEvent = false;
                controllerInputModule.NonDominantEvent = true;
                if (leftWand != null)
                {
                    leftWand.GetComponent<SpellRouting>().enabled = true;
                    rightWand.GetComponent<SpellRouting>().enabled = false;
                }
            }
            else if (playerPrefs == "Right")
            {
                dominantController.SetActive(true);
                dominantTracker.SetActive(true);
                nonDominantController.SetActive(false);
                nonDominantTracker.SetActive(false);
                controllerInputModule.NonDominantEvent = false;
                controllerInputModule.DominantEvent = true;
                if(rightWand != null)
                {
                    rightWand.GetComponent<SpellRouting>().enabled = true;
                    leftWand.GetComponent<SpellRouting>().enabled = false;
                }
            }
        }

        if(instructionPanel.activeSelf == true)
        {
            // Check for Button A press on the left controller
            if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_X))
            {
                if (nonDominantController.activeSelf == true && nonDominantTracker.activeSelf == true && IsCurrentScene("WizardDemo") && !spellRouteIsRunning) // startButton != null && 
                {
                    StartSpellRouting();
                }
            }
            // Check for Button X press on the right controller
            else if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_A))
            {
                if (dominantController.activeSelf == true && dominantTracker.activeSelf == true && IsCurrentScene("WizardDemo") && !spellRouteIsRunning) // startButton != null && 
                {
                    StartSpellRouting();
                }
            }
        }
        else if(referenceInstructionPanel.activeSelf == true)
        {
            // Check for Button A press on the left controller
            if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_X))
            {
                if (referenceStartButton != null && nonDominantController.activeSelf == true && nonDominantTracker.activeSelf == true && IsCurrentScene("WizardDemo") && !spellRouteIsRunning)
                {
                    StartSpellRouting();
                }
            }
            // Check for Button X press on the right controller
            else if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_A))
            {
                if (referenceStartButton != null && dominantController.activeSelf == true && dominantTracker.activeSelf == true && IsCurrentScene("WizardDemo") && !spellRouteIsRunning)
                {
                    StartSpellRouting();
                }
            }
        }
    }

    private void StartSpellRouting()
    {
        spellRouteIsRunning = true;
        instructionPanel.SetActive(false);
        referenceInstructionPanel.SetActive(false);
        if (playerPrefs == "Left")
        {
            nonDominantController.GetComponent<EventControllerSetter>().activeBeam = false;
            nonDominantController.GetComponent<EventControllerSetter>().activePointer = false;
            leftWand.GetComponent<SpellRouting>().StartSpellRouting();
        }
        else if (playerPrefs == "Right")
        {
            dominantController.GetComponent<EventControllerSetter>().activeBeam = false;
            dominantController.GetComponent<EventControllerSetter>().activePointer = false;
            rightWand.GetComponent<SpellRouting>().StartSpellRouting();
        }
        if(!isStarted)
        {
            DataWriter.Instance.WriteInitialLimbData();
            isStarted = true;
        }
        
    }

    private void ChangeControllerInput()
    {
        string referenceArm = (playerPrefs == "Left") ? "Right" : "Left";
        playerPrefs = referenceArm;
        // ShowMessage("Reference Arm: " + playerPrefs);
        if (playerPrefs == "Left")
        {
            nonDominantController.GetComponent<EventControllerSetter>().activeBeam = true;
            nonDominantController.GetComponent<EventControllerSetter>().activePointer = true;

        }
        else if (playerPrefs == "Right")
        {
            dominantController.GetComponent<EventControllerSetter>().activeBeam = true;
            dominantController.GetComponent<EventControllerSetter>().activePointer = true;
        }
    }

    public void SetReferenceInstructionPanelShownState(bool value)
    {
        spellRouteIsRunning = false;
        ChangeControllerInput();
        referenceInstructionPanel.SetActive(value);
        referenceInstructionPanelIsShown = value;
    }

    public bool ReferenceInstructionPanelAlreadyShown()
    {
        return referenceInstructionPanelIsShown;
    }

    public void SetConclusionPanelShownState(bool value)
    {
        spellRouteIsRunning = false;
        conclusionPanel.SetActive(value);
        conclusionPanelIsShown = value;

        // Just display to controller beam and the controller pointer at the end of the spell route scenario
        if (playerPrefs == "Left")
        {
            nonDominantController.GetComponent<EventControllerSetter>().activeBeam = true;
            nonDominantController.GetComponent<EventControllerSetter>().activePointer = true;

        }
        else if (playerPrefs == "Right")
        {
            dominantController.GetComponent<EventControllerSetter>().activeBeam = true;
            dominantController.GetComponent<EventControllerSetter>().activePointer = true;
        }
    }

    public bool ConclusionPanelAlreadyShown()
    {
        return conclusionPanelIsShown;
    }

    public bool IsCurrentScene(string sceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        return currentScene.name == sceneName;
    }

    void ShowMessage(string message)
    {
        Debug.Log(message);
        DebugText.Instance.AppendLine(message);
    }
}
