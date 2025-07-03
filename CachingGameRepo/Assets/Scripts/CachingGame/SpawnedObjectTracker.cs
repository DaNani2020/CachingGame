using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// SpawnedObjectTracker manages all spawned objects, including registering, setting and getting the ObjectInteractionState.
/// </summary>
public class SpawnedObjectTracker : MonoBehaviour
{
    public static SpawnedObjectTracker instance;
    public Dictionary<string, SpawnedObjectInfo> spawnedObjects = new Dictionary<string, SpawnedObjectInfo>();


    private int spawnCounter = 0;

    /// <summary>
    /// Initializing this instance of SpawnedObjectTracker.
    /// </summary>
    private void Awake()
    {
        if (instance == null) instance = this;
        Debug.Log("[TRACKER] Component instance awakened ");
    }

    /// <summary>
    /// Register given GameObject with UUID and the spawn point number.
    /// </summary>
    /// <param name="obj">The GameObject itself.</param>
    /// <param name="uuid">The unique identifier for this object.</param>
    /// <param name="spawnPoint">The number of the spawn point.</param>
    public void Register(GameObject obj, string uuid, int spawnPoint)
    {
        var info = new SpawnedObjectInfo(obj, spawnCounter, uuid, spawnPoint);
        spawnedObjects[uuid] = info;
        spawnCounter++;
        Debug.Log("[SPAWN CONFIRM] Component assigned in Tracker with UUID: " + uuid);
    }

    /// <summary>
    /// SetStateByUuid updates the interactionState to newState and, if given, the position the change was triggered with the collision angle.
    /// </summary>
    /// <param name="uuid">The unique identifier for a spawned object.</param>
    /// <param name="newState">The new value of ObjectInteractionStates.</param>
    /// <param name="position">The x, y, and z position in a Vector3 format.</param>
    /// <param name="angle">The float value of the collision angle.</param>
    public void SetStateByUuid(string uuid, ObjectInteractionState newState, Vector3? position = null, float? angle = null)
    {
        if (uuid == string.Empty) Debug.LogError("Object with UUID: value given for set state: " + uuid + ".");
        if (spawnedObjects.TryGetValue(uuid, out var info))
        {
            if (angle != null)
            {
                info.SetState(newState, position ?? info.spawnPosition, (float)angle);
                Debug.Log($"Object with UUID: {uuid} state set to {newState} with angle {info.interactionAngle} set.");
            }
            else
            {
                info.SetState(newState, position ?? info.spawnPosition);
                Debug.Log($"Object with UUID: {uuid} state set to {newState}.");
            }
        }
        else
        {
            Debug.LogWarning($"Object with UUID: {uuid} not found for setting state.");
        }
    }

    /// <summary>
    /// GetStateByUuid provides the current ObjectInteractionStates of object with the given uuid.
    /// </summary> 
    /// <param name="uuid">The unique identifier for a spawned object.</param>
    /// <returns>The current ObjectInteractionStates.</returns>
    public ObjectInteractionState GetStateByUuid(string uuid)
    {
        if (spawnedObjects.TryGetValue(uuid, out var info))
        {
            return info.interactionState;
        }

        Debug.LogWarning($"Object with UUID: {uuid} not found for getting state.");
        return default;
    }

    /// <summary>
    /// ResetTracker deletes all entries of previously registered spawnedObjects and sets spawnCounter to 0.
    /// </summary>
    public void ResetTracker()
    {
        spawnedObjects.Clear();
        spawnCounter = 0;
    }

}
