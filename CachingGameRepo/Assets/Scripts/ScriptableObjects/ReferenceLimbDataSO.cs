using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReferenceLimbData", menuName = "ScriptableObjects/ReferenceLimbDataSO", order = 4)]
public class ReferenceLimbDataSO : ScriptableObject
{
    [SerializeField]
    private ChestDataSO chest;

    [SerializeField]
    private UpperLimbDataSO upperLimbVertical;

    [SerializeField]
    private UpperLimbDataSO upperLimbHorizontal;

    [SerializeField]
    private ElbowFlexionDataSO elbowFlexion;

    [SerializeField]
    private LowerLimbRotationDataSO lowerLimbRotation;

    [SerializeField]
    private SpawningPointDataSO[] spawningPointData;

    public ChestDataSO Chest => chest;
    public UpperLimbDataSO UpperLimbVertical => upperLimbVertical;
    public UpperLimbDataSO UpperLimbHorizontal => upperLimbHorizontal;
    public ElbowFlexionDataSO ElbowFlexion => elbowFlexion;
    public LowerLimbRotationDataSO LowerLimbRotation => lowerLimbRotation;
    public SpawningPointDataSO[] SpawningPointData => spawningPointData;
}
