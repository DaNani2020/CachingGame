using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LimbData", menuName = "ScriptableObjects/LimbDataSO", order = 10)]
public class LimbDataSO : ScriptableObject
{
    [SerializeField]
    private Vector3 trackerPosition;

    [SerializeField]
    private float angleAtSpawningPointCollection;

    public Vector3 TrackerPosition => trackerPosition;
    public float AngleAtSpawningPointCollection => angleAtSpawningPointCollection;
}
