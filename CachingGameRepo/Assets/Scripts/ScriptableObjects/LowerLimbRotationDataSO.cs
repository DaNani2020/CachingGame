using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LowerLimbRotationData", menuName = "ScriptableObjects/LowerLimbRotationDataSO", order = 9)]
public class LowerLimbRotationDataSO : ScriptableObject
{
    [SerializeField]
    private LimbDataSO right;

    [SerializeField]
    private LimbDataSO left;

    public LimbDataSO Right => right;
    public LimbDataSO Left => left;
}

