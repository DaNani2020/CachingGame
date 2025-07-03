using System;
using UnityEngine;


/// <summary>
/// Class for providing object information, including unique identifier (UUID), even after game object is deleted.
/// </summary>
[System.Serializable]
public class SpawnedObjectInfo
{
    public string UUID;
    public int spawnIndex;
    public int spawnPointID;
    public string objectName;
    public Vector3 spawnPosition;
    public Quaternion spawnRotation;
    public ObjectInteractionState interactionState = ObjectInteractionState.NONE;

    [Tooltip("Position of collision with cacher.")]
    public Vector3? interactionPosition = null;

    [Tooltip("Collision angle with cacher.")]
    public float? interactionAngle = null;


    /// <summary>
    /// SpawnedObjectInfo is filled with object information at time of spawning this object.
    /// </summary>
    /// <param name="obj">The GameObject itself.</param>
    /// <param name="index">The number of spawned objects.</param>
    /// <param name="uuid">The unique identifier for this object.</param>
    /// <param name="spawnPoint">The number of the spawn point.</param>
    public SpawnedObjectInfo(GameObject obj, int index, string uuid, int spawnPoint)
    {
        UUID = uuid;
        spawnIndex = index;
        spawnPointID = spawnPoint;
        objectName = obj.name;
        spawnPosition = obj.transform.position;
        spawnRotation = obj.transform.rotation;
    }

    /// <summary>
    /// SetState updates the interactionState to newState and the position the change was triggered.
    /// </summary>
    /// <param name="newState">The new value of ObjectInteractionStates.</param>
    /// <param name="position">The x, y, and z position in a Vector3 format.</param>
    public void SetState(ObjectInteractionState newState, Vector3 position)
    {
        interactionState = newState;
        interactionPosition = position;
    }

    /// <summary>
    /// SetState updates the interactionState to newState, the position the change was triggered and the collision angle.
    /// </summary>
    /// <param name="newState">The new value of ObjectInteractionStates.</param>
    /// <param name="position">The x, y, and z position in a Vector3 format.</param>
    /// <param name="collisionAngle">The float value of the collision angle.</param>
    public void SetState(ObjectInteractionState newState, Vector3 position, float collisionAngle)
    {
        interactionState = newState;
        interactionPosition = position;
        interactionAngle = collisionAngle;
    }

    /// <summary>
    /// GetState provides the current ObjectInteractionStates.
    /// </summary>
    /// <returns>The current ObjectInteractionStates.</returns>
    public ObjectInteractionState GetState()
    {
        return interactionState;
    }
}
