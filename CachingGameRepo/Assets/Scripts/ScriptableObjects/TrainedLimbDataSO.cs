using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainedLimbData", menuName = "ScriptableObjects/TrainedLimbDataSO", order = 3)]
public class TrainedLimbDataSO : ScriptableObject
{
    [SerializeField]
    private ChestDataSO chestRotation;

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

    public ChestDataSO ChestRotation => chestRotation;
    public UpperLimbDataSO UpperLimbVertical => upperLimbVertical;
    public UpperLimbDataSO UpperLimbHorizontal => upperLimbHorizontal;
    public ElbowFlexionDataSO ElbowFlexion => elbowFlexion;
    public LowerLimbRotationDataSO LowerLimbRotation => lowerLimbRotation;
    public SpawningPointDataSO[] SpawningPointData => spawningPointData;
}
