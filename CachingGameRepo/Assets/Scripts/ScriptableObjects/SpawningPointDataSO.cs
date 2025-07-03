using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawningPointData", menuName = "ScriptableObjects/SpawningPointDataSO", order = 5)]
public class SpawningPointDataSO : ScriptableObject
{
    [SerializeField]
    private Vector3 position;

    [SerializeField]
    private float timestamp;

    [SerializeField]
    private float spawningPointAuraSize;

    [SerializeField]
    private int collectedInIteration;

    public Vector3 Position => position;
    public float Timestamp => timestamp;
    public float SpawningPointAuraSize => spawningPointAuraSize;
    public int CollectedInIteration => collectedInIteration;
}
