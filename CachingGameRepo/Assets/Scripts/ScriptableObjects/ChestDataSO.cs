using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestData", menuName = "ScriptableObjects/ChestDataSO", order = 6)]
public class ChestDataSO : ScriptableObject
{
    [SerializeField]
    private Vector3 trackerPosition;

    [SerializeField]
    private Vector3 rotation;

    public Vector3 TrackerPosition => trackerPosition;
    public Vector3 Rotation => rotation;
}

