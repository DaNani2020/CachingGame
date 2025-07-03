using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// PerformanceManager calculates statistics regarding player's performance based on the given number of objects (analyseWindowSize).
/// </summary>
public class PerformanceManager : MonoBehaviour
{
    TargetSpawner ballSpawner;
    SpawnedObjectTracker spawnedObjectTracker;

    [Header("Update Settings")]
    [Tooltip("Time interval in seconds to next update.")]
    public float interval = 10f;
    [Tooltip("Time counter, increasing until it hits interval value.")]
    private float nextTime = 0f;

    [Tooltip("Maximum number (of last spawned objects) to be analysed for performance.")]
    public int analyseWindowSize = 10;

    /// <summary>
    /// Awake sets up references to instance of TargetSpawner and SpawnedObjectTracker if null.
    /// </summary>
    void Awake()
    {
        ballSpawner = TargetSpawner.instance;
        if (ballSpawner == null)
        {
            Debug.LogWarning("BallSpawner not initialized.");
            return;
        }
        spawnedObjectTracker = SpawnedObjectTracker.instance;
        if (spawnedObjectTracker == null)
        {
            Debug.LogWarning("SpawnedObjectTracker not initialized.");
            return;
        }

    }


    /// <summary>
    /// Start sets up reference to instance of TargetSpawner if null and interval to spawning interval.
    /// </summary>
    void Start()
    {
        if (ballSpawner == null)
        {
            ballSpawner = TargetSpawner.instance;
            Debug.LogWarning("BallSpawner not initialized." + ballSpawner);
            return;
        }

        // Making sure, that least once between spawns statistics are updated
        interval = ballSpawner.spawningDistance;
    }

    // Update is called once per frame
    /// <summary>
    /// Update function calls in the chosen interval during the game UpdateStatistics to update performance calculations.
    /// </summary>
    void Update()
    {
        if (ScoreManager.instance.isPlaying && Time.time >= nextTime)
        {
            Debug.Log("RATIO performance Bundle: We are PLAYING " + Time.time + " with interval " + interval + " for  >= nextTime " + nextTime);

            if (spawnedObjectTracker != null)
            {
                UpdateStatistics();

            }
            else
            {
                //Debug.Log("PLAYING in effect but spawnedObjectTracker defined as null " + (spawnedObjectTracker == null) + " but actual " + SpawnedObjectTracker.instance);
                spawnedObjectTracker = SpawnedObjectTracker.instance;
                if (spawnedObjectTracker == null)
                {
                    Debug.LogWarning("SpawnedObjectTracker still not initialized.");
                    return;
                }
                //Debug.Log("PLAYING in effect but spawnedObjectTracker defined still as null " + (spawnedObjectTracker == null));
            }

            nextTime = Time.time + interval; 
        }
    }

    /// <summary>
    /// UpdateStatistics triggering all routines for analysing performance values and setting updated values in TargetSpawner instance.
    /// This includes the offset of uncached objects, as well as the success rate of caching and reaching objects based on types and per spawning point (source).
    /// </summary>
    public void UpdateStatistics()
    {
        var offsetXandY = CalculateUncachedTotalAngleXY(analyseWindowSize);
        ballSpawner.SetXandYPositionOffset(offsetXandY);

        var successRateOfTypes = CalculateCachedRatesOfTypes(analyseWindowSize);
        ballSpawner.SetSuccessRatesPerType(successRateOfTypes);

        var reachedRateOfTypes = CalculateReachedRatesOfTypes(analyseWindowSize);
        ballSpawner.SetReachedRatesPerType(reachedRateOfTypes);

        var reachedRateOfTypesBySource = CalculateReachedRatesOfTypesPerSource(analyseWindowSize);
        ballSpawner.SetReachedRatesPerSourceAndType(reachedRateOfTypesBySource);

        var offsetXandYperSource = CalculateUncachedTotalAngleXYPerSource(analyseWindowSize);
        ballSpawner.SetXYPositionOffsetPerSource(offsetXandYperSource);
    }


    /// <summary>
    /// Ratio calculation of the objects reached to total number of object type.
    /// </summary>
    /// <param name="windowSize">The number of objects included in the calculation.</param>
    /// <returns>Dictionary with types as key and ratio as value.</returns>
    /// <remarks>
    /// Reached includes ObjectInteractionState CACHED and POPPED.
    /// </remarks>
    public Dictionary<string, float> CalculateReachedRatesOfTypes(int windowSize)
    {
        var result = new Dictionary<string, float>();

        if (spawnedObjectTracker == null || spawnedObjectTracker.spawnedObjects.Count == 0)
        {
            Debug.LogWarning("SpawnedObjectTracker not initialized or has no spawned objects.");
            return result;
        }

        IEnumerable<IGrouping<string, SpawnedObjectInfo>> grouped = GetObjectsByTypeWithinWindow(windowSize);

        foreach (var group in grouped)
        {
            // Only consider entries with meaningful state (not NONE)
            var relevantInfos = group.Where(info => info.interactionState != ObjectInteractionState.NONE).ToList();

            int total = relevantInfos.Count;

            int reached = relevantInfos.Count(info =>
                info.interactionState == ObjectInteractionState.CACHED ||
                info.interactionState == ObjectInteractionState.POPPED);
            float ratio = total > 0 ? (float)reached / total : 0f;

            //DebugBreakDownOfLogSuccessRate(group, total, reached, ratio, "reached");

            result[group.Key] = ratio;
        }

        return result;
    }

    /// <summary>
    /// Ratio calculation of the objects reached to total number of object type but based on source.
    /// </summary>
    /// <param name="windowSize">The number of objects included in the calculation.</param>
    /// <returns>Dictionary ordered by source as key and Dictionary with types and ratio as value.</returns>
    /// <remarks>
    /// Reached includes ObjectInteractionState CACHED and POPPED.
    /// </remarks>
    public Dictionary<int, Dictionary<string, float>> CalculateReachedRatesOfTypesPerSource(int windowSize)
    {
        var result = new Dictionary<int, Dictionary<string, float>>();


        if (spawnedObjectTracker == null || spawnedObjectTracker.spawnedObjects.Count == 0)
        {
            Debug.LogWarning("SpawnedObjectTracker not initialized or has no spawned objects.");
            return result;
        }

        var groupedBySource = GetObjectsGroupedBySourceAndNameInWindow(windowSize);
        //LogGroupedObjectsForSource(windowSize);

        foreach (var spawnPointGroup in groupedBySource)
        {
            int spawnPointID = spawnPointGroup.Key;

            // Ensure the inner dictionary exists
            if (!result.ContainsKey(spawnPointID))
            {
                result[spawnPointID] = new Dictionary<string, float>();
            }

            foreach (var objectGroup in spawnPointGroup.Value)
            {
                string objectName = objectGroup.Key;
                List<SpawnedObjectInfo> infos = objectGroup.Value;

                var relevantInfos = infos
                    .Where(info => info.interactionState != ObjectInteractionState.NONE)
                    .ToList();

                int total = relevantInfos.Count;

                int reached = relevantInfos.Count(info =>
                    info.interactionState == ObjectInteractionState.CACHED ||
                    info.interactionState == ObjectInteractionState.POPPED);

                float ratio = total > 0 ? (float)reached / total : 0f;

                result[spawnPointID][objectName] = ratio;
            }
        }

        return result;
    }


    /// <summary>
    /// Ratio calculation of the objects cached to total number of object type.
    /// </summary>
    /// <param name="windowSize">The number of objects included in the calculation.</param>
    /// <returns>Dictionary with types as key and ratio as value.</returns>
    /// <remarks>
    /// Cached includes ObjectInteractionState CACHED.
    /// </remarks>
    public Dictionary<string, float> CalculateCachedRatesOfTypes(int windowSize)
    {
        var result = new Dictionary<string, float>();

        if (spawnedObjectTracker == null || spawnedObjectTracker.spawnedObjects.Count == 0)
        {
            Debug.LogWarning("SpawnedObjectTracker not initialized or has no spawned objects.");
            return result;
        }

        IEnumerable<IGrouping<string, SpawnedObjectInfo>> grouped = GetObjectsByTypeWithinWindow(windowSize);

        foreach (var group in grouped)
        {
            // Only consider entries with meaningful state (not NONE)
            var relevantInfos = group.Where(info => info.interactionState != ObjectInteractionState.NONE).ToList();

            int total = relevantInfos.Count;
            int cached = relevantInfos.Count(info => info.interactionState == ObjectInteractionState.CACHED);
            float ratio = total > 0 ? (float)cached / total : 0f;

            //DebugBreakDownOfLogSuccessRate(group, total, cached, ratio, "cached");
            result[group.Key] = ratio;
        }

        return result;
    }


    /// <summary>
    /// Groups the last added objects (number based on windowSize) by source (spawnPointID) and type (objectName).
    /// </summary>
    /// <param name="windowSize">The number of objects included in the calculation.</param>
    /// <returns>Dictionary ordered by source as key and Dictionary with types and List of SpawnedObjectInfo as value.</returns>
    private Dictionary<int, Dictionary<string, List<SpawnedObjectInfo>>> GetObjectsGroupedBySourceAndNameInWindow(int windowSize)
    {
        var allInfos = spawnedObjectTracker.spawnedObjects.Values;
        int maxIndex = allInfos.Max(info => info.spawnIndex);
        int minIndex = Math.Max(0, maxIndex - windowSize + 1);

        var recentInfos = allInfos
            .Where(info => info.spawnIndex >= minIndex); // includes all available if fewer than windowSize

        Debug.Log("OFFSET WINDOW BaseLINE " + recentInfos + " number of Objects " + recentInfos.Count());

        var grouped = recentInfos
                .Where(info => !info.objectName.ToLower().Contains("bomb"))
                //.GroupBy(info => info.spawnPointID);
                .GroupBy(info => info.spawnPointID)
                .ToDictionary(
                    g => g.Key, // spawnPointID
                    g => g.GroupBy(info => info.objectName) // group by object name within spawn point
                        .ToDictionary(subGroup => subGroup.Key, subGroup => subGroup.ToList()));

        return grouped;
    }


    /*
    /// <summary>
    /// Logs grouped objects of number windowSize for debugging purposes.
    /// </summary>
    /// <param name="windowSize">The number of grouped objects.</param>
    private void LogGroupedObjectsForSource(int windowSize)
    {
        // Get the grouped objects using the method you provided
        var groupedObjects = GetObjectsGroupedBySourceAndNameInWindow(windowSize);

        // Iterate through each spawnPointID group
        foreach (var spawnPointGroup in groupedObjects)
        {
            // Log spawnPointID
            Debug.Log($"Spawn Point ID: {spawnPointGroup.Key}");

            // Iterate through each object type (grouped by objectName) within the spawn point
            foreach (var objectGroup in spawnPointGroup.Value)
            {
                // Log the object name and how many objects of this type
                Debug.Log($" Spawn Point Object Name: {objectGroup.Key}, Count: {objectGroup.Value.Count}");

                // Optionally, log each object's details (if you want more info on the objects)
                foreach (var info in objectGroup.Value)
                {
                    Debug.Log($" Spawn Point   SpawnedObjectInfo: {info.objectName}, SpawnPointID: {info.spawnPointID}");
                }
            }
        }
    }
    */


    /// <summary>
    /// Groups the last added objects (number based on windowSize) by type (objectName).
    /// </summary>
    /// <param name="windowSize">The number of objects included in the calculation.</param>
    /// <returns>Collection of groups with types as key and SpawnedObjectInfo as value.</returns>
    private IEnumerable<IGrouping<string, SpawnedObjectInfo>> GetObjectsByTypeWithinWindow(int windowSize)
    {
        var allInfos = spawnedObjectTracker.spawnedObjects.Values;
        int maxIndex = allInfos.Max(info => info.spawnIndex);
        int minIndex = Math.Max(0, maxIndex - windowSize + 1);

        var recentInfos = allInfos
            .Where(info => info.spawnIndex >= minIndex); // includes all available if fewer than windowSize

        Debug.Log("OFFSET WINDOW BaseLINE " + recentInfos + " number of Objects " + recentInfos.Count());

        var grouped = recentInfos
                .Where(info => !info.objectName.ToLower().Contains("bomb"))
                .GroupBy(info => info.objectName);

        return grouped;
    }

    /*
    /// <summary>
    /// Log of success rate of chosen actionType for debugging purposes.
    /// </summary>
    /// <param name="group">The given group.</param>
    /// <param name="total">The number of total items.</param>
    /// <param name="cached">The number of items.</param>
    /// <param name="ratio">The given ratio.</param>
    /// <param name="actionType">The given actionType referring to successfully cached or reached.</param>
    private static void DebugBreakDownOfLogSuccessRate(IGrouping<string, SpawnedObjectInfo> group, int total, int cached, float ratio, string actionType)
    {
        Debug.Log($"[{actionType.ToUpper()} RATIO] Object: {group.Key}, actionType: {actionType} and number {cached}, Total: {total}, Success Rate: {ratio:P1}");

        // Display breakdown of states
        var stateBreakdown = group
            .GroupBy(info => info.interactionState)
            .Select(stateGroup => $"{stateGroup.Key}: {stateGroup.Count()}")
            .ToList();

        string breakdownMessage = string.Join(", ", stateBreakdown);
        Debug.Log($"[{actionType.ToUpper()} RATIO STATE BREAKDOWN] {group.Key}: {breakdownMessage}");
    }
    */


    /// <summary>
    /// Sum calculation of uncached object's angles x and y of all object types (excluding Bombs) per source.
    /// </summary>
    /// <param name="windowSize">The number of objects included in the calculation.</param>
    /// <returns>Dictionary with source as key and Vector2 with x and y angle sum as value.</returns>
    public Dictionary<int, (float, float)> CalculateUncachedTotalAngleXYPerSource(int windowSize)
    {
        var result = new Dictionary<int, (float, float)>();

        if (spawnedObjectTracker == null || spawnedObjectTracker.spawnedObjects.Count == 0)
        {
            Debug.LogWarning("SpawnedObjectTracker not initialized or has no spawned objects. Spawned objects count: " + spawnedObjectTracker.spawnedObjects.Count);
            return result;
        }

        var groupedBySource = GetObjectsGroupedBySourceAndNameInWindow(windowSize);
        //LogGroupedObjectsForSource(windowSize);

        foreach (var spawnPointGroup in groupedBySource)
        {
            int spawnPointID = spawnPointGroup.Key;

            // Ensure the inner dictionary exists
            if (!result.ContainsKey(spawnPointID))
            {
                result[spawnPointID] = (0f, 0f);
            }

            var groupedInfos = spawnPointGroup.Value
                .SelectMany(kvp => kvp.Value)
                .GroupBy(info => info.objectName); // Group by the same key

            result[spawnPointID] = SumOfAnglesXY(groupedInfos);
        }
        return result;
    }


    /// <summary>
    /// Sum calculation of uncached object's angles x and y of all object types (excluding Bombs).
    /// </summary>
    /// <param name="windowSize">The number of objects included in the calculation.</param>
    /// <returns>Vector2 with x and y angle sum.</returns>
    public (float overallAngleSumOfX, float overallAngleSumOfY) CalculateUncachedTotalAngleXY(int windowSize)
    {
        var result = (0f, 0f);

        if (spawnedObjectTracker == null || spawnedObjectTracker.spawnedObjects.Count == 0)
        {
            Debug.LogWarning("SpawnedObjectTracker not initialized or has no spawned objects. Spawned objects count: " + spawnedObjectTracker.spawnedObjects.Count);
            return result;
        }

        IEnumerable<IGrouping<string, SpawnedObjectInfo>> grouped = GetObjectsByTypeWithinWindow(windowSize);

        result = SumOfAnglesXY(grouped);
        return result;
    }


    /// <summary>
    /// Sum calculation of not reached object's angles x and y of the given collection of groups (weighted based on quadrant restriction).
    /// </summary>
    /// <param name="grouped">Collection of groups with types as key and SpawnedObjectInfo as value.</param>
    /// <returns>Vector2 with x and y angle sum.</returns>
    private (float, float) SumOfAnglesXY(IEnumerable<IGrouping<string, SpawnedObjectInfo>> grouped)
    {
        float overallAngleSumOfX = 0f;
        float overallAngleSumOfY = 0f;
        (float, float) result;
        foreach (var group in grouped)
        {
            // Only consider entries with meaningful state (not NONE)
            var relevantInfos = group.Where(info => info.interactionState == ObjectInteractionState.EXPIRED).ToList();

            float angleSumOfX = 0f;
            float angleSumOfY = 0f;

            Debug.Log($"OFFSET SpawnedObject EXPIRED {relevantInfos} has size {relevantInfos.Count}");

            foreach (var item in relevantInfos)
            {
                (float angleX, float angleY) = RebaseRotationAngles(item.spawnRotation);

                float offsetToBeModifiedX = angleX;
                float offsetToBeModifiedY = angleY;
                //Debug.Log($"OFFSET SpawnedObject {item.objectName} has x {offsetToBeModifiedX} and y {offsetToBeModifiedY}");

                if (ballSpawner.quadrantRestrictions.ContainsKey(item.objectName))
                {
                    if (ballSpawner.quadrantRestrictions[item.objectName] == "Third")
                    {
                        if (angleX > 0) offsetToBeModifiedX = angleX / 2;
                        if (angleY > 0) offsetToBeModifiedY = angleY / 2;
                    }
                    else if (ballSpawner.quadrantRestrictions[item.objectName] == "Fourth")
                    {
                        if (angleX < 0) offsetToBeModifiedX += angleY / 2;
                        if (angleY > 0) offsetToBeModifiedY = angleY / 2;
                    }
                }

                angleSumOfX += offsetToBeModifiedX;
                angleSumOfY += offsetToBeModifiedY;
            }

            overallAngleSumOfX += angleSumOfX;
            overallAngleSumOfY += angleSumOfY;
            Debug.Log($"[OFFSET ROTATION] {group} x: {overallAngleSumOfX} and y: {overallAngleSumOfY}");
        }
        result = (overallAngleSumOfX, overallAngleSumOfY);
        return result;
    }


    /// <summary>
    /// Eliminates offset (rebase) of angle rotation, converting Quaterion to Euler angles in the process.
    /// </summary>
    /// <param name="rotationOfObject">The gimbal-lock-free rotation of an object in 3D space.</param>
    /// <returns>Vector2 of x and y angle values.</returns>
    public (float, float) RebaseRotationAngles(Quaternion rotationOfObject)
    {
        // Convert the rotation to Euler angles (in degrees)
        float xAngle = rotationOfObject.eulerAngles.x;
        float yAngle = rotationOfObject.eulerAngles.y;
        float zAngle = rotationOfObject.eulerAngles.z;

        if (xAngle > 180)
        {
            xAngle = xAngle - 360;
        }
        if (yAngle > 90)
        {
            yAngle = yAngle - 180;
        }
        Debug.Log(" Angle Rotations tidied up x " + xAngle + " y " + yAngle + " z " + zAngle);
        return (xAngle, yAngle);
    }

}