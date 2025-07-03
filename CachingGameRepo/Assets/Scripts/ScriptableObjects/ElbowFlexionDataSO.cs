using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElbowFlexionData", menuName = "ScriptableObjects/ElbowFlexionDataSO", order = 8)]
public class ElbowFlexionDataSO : ScriptableObject
{
    [SerializeField]
    private LimbDataSO right;

    [SerializeField]
    private LimbDataSO left;

    public LimbDataSO Right => right;
    public LimbDataSO Left => left;
}
