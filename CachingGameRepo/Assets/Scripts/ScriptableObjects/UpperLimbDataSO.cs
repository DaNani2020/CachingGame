using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpperLimbData", menuName = "ScriptableObjects/UpperLimbDataSO", order = 7)]
public class UpperLimbDataSO : ScriptableObject
{
    [SerializeField]
    private LimbDataSO right;

    [SerializeField]
    private LimbDataSO left;

    public LimbDataSO Right => right;
    public LimbDataSO Left => left;
}

