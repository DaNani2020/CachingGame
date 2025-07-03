using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "ScriptableObjects/UserDataSO", order = 1)]
public class UserDataSO : ScriptableObject
{
    // [SerializeField]
    // private int userID; // given by the database

    [SerializeField]
    private DateTime simulationStartTime;

    [SerializeField]
    private DateTime simulationEndTime;

    [SerializeField]
    private TrainedLimb trainedLimb;

    [SerializeField]
    private InitialDataSO initialData;

    [SerializeField]
    private TrainedLimbDataSO trainedLimbData;

    [SerializeField]
    private ReferenceLimbDataSO referenceLimbData;

    // public int UserID => userID; // given by the database
    public DateTime SimulationStartTime => simulationStartTime;
    public DateTime SimulationEndTime => simulationEndTime;
    public TrainedLimb TrainedLimb => trainedLimb;
    public InitialDataSO InitialData => initialData;
    public TrainedLimbDataSO TrainedLimbData => trainedLimbData;
    public ReferenceLimbDataSO ReferenceLimbData => referenceLimbData;
}

public enum TrainedLimb
{
    Left,
    Right
}