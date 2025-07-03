using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleComputeDebugging : MonoBehaviour
{
    public UpperLimbAngleCalculator upperLimbAngleCalculator;

    public Transform trackerChest; // The reference point, e.g., the spine or chest
    public Transform trackerElbowLeft; // The right elbow tracker
    public Transform controllerLeft; // The right controller
    public Transform trackerElbowRight; // The right elbow tracker
    public Transform controllerRight; // The right controller

    public bool isExecuted = false;


    public bool horizontalRight = false;
    public bool verticalRight = false;
    public bool rotationRight = false;
    public bool extensionRight = false;
    public bool supinationRight = false;
    public bool horizontalLeft = false;
    public bool verticalLeft = false;
    public bool rotationLeft = false;
    public bool extensionLeft = false;
    public bool supinationLeft = false;

    public bool chestRotation = false;

    void Start()
    {
        if (trackerChest == null)
        {
            trackerChest = GameObject.FindGameObjectWithTag("ChestTracker").transform;
        }
        if (trackerElbowRight == null)
        {
            trackerElbowRight = GameObject.FindGameObjectWithTag("ElbowRightTracker").transform;
        }
        if (controllerRight == null)
        {
            controllerRight = GameObject.FindGameObjectWithTag("DominantTracker").transform;
        }
        if (trackerElbowLeft == null)
        {
            trackerElbowLeft = GameObject.FindGameObjectWithTag("ElbowLeftTracker").transform;
        }
        if (controllerLeft == null)
        {
            controllerLeft = GameObject.FindGameObjectWithTag("NonDominantTracker").transform;
        }
    }

    void Update()
    {
        if(!isExecuted)
        {
            isExecuted = true;
            DebugAll();
        }

        if (horizontalRight)
        {

            upperLimbAngleCalculator.angleShoulderHorizontalRight(trackerElbowRight, trackerChest); // calculate shoulder angle - moving on z-axis (horizontal)
        }
        if (verticalRight)
        {
            upperLimbAngleCalculator.angleShoulderVerticalRight(trackerElbowRight, trackerChest); // calculate shoulder angle - moving on y-axis (vertical) - implemented
        }
        if (rotationRight)
        {

            upperLimbAngleCalculator.shoulderRotationRight(trackerElbowRight, trackerChest); // calculate shoulder rotation angle
        }
        if (extensionRight)
        {

            upperLimbAngleCalculator.angleElbowExtensionRight(trackerElbowRight, controllerRight); // calculate elbow angle
        }
        if (supinationRight)
        {

            upperLimbAngleCalculator.elbowSupinationRight(trackerElbowRight, controllerRight); // calculate elbow supination angle (rotation of the lower arm around the upper arm)
        }
        if (horizontalLeft)
        {

            upperLimbAngleCalculator.angleShoulderHorizontalLeft(trackerElbowLeft, trackerChest); // calculate shoulder angle - moving on z-axis (horizontal)
        }
        if (verticalLeft)
        {

            upperLimbAngleCalculator.angleShoulderVerticalLeft(trackerElbowLeft, trackerChest); // calculate shoulder angle - moving on y-axis (vertical) - implemented
        }
        if (rotationLeft)
        {

            upperLimbAngleCalculator.shoulderRotationLeft(trackerElbowLeft, trackerChest); // calculate shoulder rotation angle
        }
        if (extensionLeft)
        {

            upperLimbAngleCalculator.angleElbowExtensionLeft(trackerElbowLeft, controllerLeft); // calculate elbow angle
        }
        if (supinationLeft)
        {

            upperLimbAngleCalculator.elbowSupinationLeft(trackerElbowLeft, controllerLeft); // calculate elbow supination angle (rotation of the lower arm around the upper arm)
        }
        if (chestRotation)
        {

            upperLimbAngleCalculator.ChestRotationMeasurement(trackerChest); // calculate chest rotation
        }

    }

    public void DebugAll()
    {
        upperLimbAngleCalculator.angleShoulderHorizontalRight(trackerElbowRight, trackerChest); // calculate shoulder angle - moving on z-axis (horizontal)
        upperLimbAngleCalculator.angleShoulderVerticalRight(trackerElbowRight, trackerChest); // calculate shoulder angle - moving on y-axis (vertical) - implemented
        upperLimbAngleCalculator.shoulderRotationRight(trackerElbowRight, trackerChest); // calculate shoulder rotation angle
        upperLimbAngleCalculator.angleElbowExtensionRight(trackerElbowRight, controllerRight); // calculate elbow angle
        upperLimbAngleCalculator.elbowSupinationRight(trackerElbowRight, controllerRight); // calculate elbow supination angle (rotation of the lower arm around the upper arm)
        upperLimbAngleCalculator.angleShoulderHorizontalLeft(trackerElbowLeft, trackerChest); // calculate shoulder angle - moving on z-axis (horizontal)
        upperLimbAngleCalculator.angleShoulderVerticalLeft(trackerElbowLeft, trackerChest); // calculate shoulder angle - moving on y-axis (vertical) - implemented
        upperLimbAngleCalculator.shoulderRotationLeft(trackerElbowLeft, trackerChest); // calculate shoulder rotation angle
        upperLimbAngleCalculator.angleElbowExtensionLeft(trackerElbowLeft, controllerLeft); // calculate elbow angle
        upperLimbAngleCalculator.elbowSupinationLeft(trackerElbowLeft, controllerLeft); // calculate elbow supination angle (rotation of the lower arm around the upper arm)
        upperLimbAngleCalculator.ChestRotationMeasurement(trackerChest); // calculate chest rotation
    }



}
