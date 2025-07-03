using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialData", menuName = "ScriptableObjects/InitialDataSO", order = 2)]
public class InitialDataSO : ScriptableObject
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

    public ChestDataSO ChestRotation => chestRotation;
    public UpperLimbDataSO UpperLimbVertical => upperLimbVertical;
    public UpperLimbDataSO UpperLimbHorizontal => upperLimbHorizontal;
    public ElbowFlexionDataSO ElbowFlexion => elbowFlexion;
    public LowerLimbRotationDataSO LowerLimbRotation => lowerLimbRotation;
}
