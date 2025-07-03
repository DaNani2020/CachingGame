using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Wave.Essence;
using Wave.Native;




public class MenuSelection : MonoBehaviour
{
    [SerializeField]
    private Button leftArmButton;
    [SerializeField]
    private Button rightArmButton;

    [SerializeField]
    private Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        startButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            Debug.Log("Left Grip Button Pressed");
            ChooseLeftArm();
        }

        if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            Debug.Log("Left Grip Button Pressed");
            ChooseRightArm();
        }

        // Check for Button A press on the left controller
        if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_X))
        {
            if ((PlayerPrefs.GetString("SelectedArm") == "Left" || PlayerPrefs.GetString("SelectedArm") == "Right") && startButton.interactable == true)
            {
                string playerPrefs = PlayerPrefs.GetString("SelectedArm");
                Debug.Log("Button A Pressed with left controller: "+ playerPrefs);
                LoadGame();
            }
        }
        // Check for Button X press on the right controller
        else if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_A))
        {
            if ((PlayerPrefs.GetString("SelectedArm") == "Left" || PlayerPrefs.GetString("SelectedArm") == "Right") && startButton.interactable == true)
            {
                string playerPrefs = PlayerPrefs.GetString("SelectedArm");
                Debug.Log("Button X Pressed with right controller: " + playerPrefs);
                LoadGame();

            }
        }
    }


    // Function to choose the left arm
    public void ChooseLeftArm()
    {
        // Set the selected arm to the left arm
        PlayerPrefs.SetString("SelectedArm", "Left");
        string playerPrefs = PlayerPrefs.GetString("SelectedArm");
        // print("Playerprefs: " + playerPrefs);
        leftArmButton.interactable = false;
        rightArmButton.interactable = !rightArmButton.IsInteractable() ? true : rightArmButton.interactable;
        if(startButton.interactable == false)
        {
            startButton.interactable = true;
        }
    }

    public void ChooseRightArm()
    {
        // Set the selected arm to the right arm
        PlayerPrefs.SetString("SelectedArm", "Right");
        string playerPrefs = PlayerPrefs.GetString("SelectedArm");
        // print("Playerprefs: " + playerPrefs);
        rightArmButton.interactable = false;
        leftArmButton.interactable = !leftArmButton.IsInteractable() ? true : leftArmButton.interactable;
        if (startButton.interactable == false)
        {
            startButton.interactable = true;
        }
    }

    // Function to load the game scene
    public void LoadGame()
    {
        try
        {
            // Load the game scene
            SceneManager.LoadScene("WizardDemo");
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex );
        }
    }
}
