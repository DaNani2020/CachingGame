using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class to manage the buzzer-like custom button, which has its own GameMode assigned to it in the Unity Editor Inspector.
/// </summary>
public class ButtonVR : MonoBehaviour
{

    public GameObject pressableButtonPart;
    public UnityEvent onPress;
    public UnityEvent onRelease;

    private GameObject presser;

    private AudioSource sound;
    private bool isPressed;

    ScoreManager gameManager;
    public ScoreManager.GameMode assignedGameMode;


    /// <summary>
    /// Start method adds game manager instance and sound, setting default state of button to isPressed = false.
    /// </summary>
    void Start()
    {
        gameManager = ScoreManager.instance;
        sound = GetComponent<AudioSource>();
        isPressed = false;
    }

    /// <summary>
    /// OnTriggerEnter is called, if object this script is attached to, collides with "other" object and invokes onPress.
    /// Sets current GameMode to assignedGameMode (which is set in Inspector.)
    /// </summary>
    /// <param name="other">Collider of other gameObject.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed)
        {
            ScoreManager.instance.SetCurrentGameMode(assignedGameMode);
            pressableButtonPart.transform.localPosition = new Vector3(0, 0.01f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            sound.Play();
            isPressed = true;
        }
    }

    /// <summary>
    /// OnTriggerExit is called, if object this script is attached to, collides with "other" object and invokes onRelease.
    /// Sets current GameMode to assignedGameMode (which is set in Inspector.)
    /// </summary>
    /// <param name="other">Collider of other gameObject.</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser && !gameManager.isPlaying)
        {
            ScoreManager.instance.SetCurrentGameMode(assignedGameMode);
            pressableButtonPart.transform.localPosition = new Vector3(0, 0.15f, 0);
            onRelease.Invoke();
            isPressed = false;
        }
    }
}
