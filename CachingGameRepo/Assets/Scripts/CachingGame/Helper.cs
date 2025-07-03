using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Helper class with methods utilized by other classes.
/// </summary>
public static class Helper
{
    /// <summary>
    /// Try to get UUID via the GameObject, checking also children for SpawnedObjectReference (which holds UUID).
    /// </summary>
    /// <param name="gameObject">The GameObject to be identified via SpawnedObjectReference.</param>
    /// <returns></returns>
    public static string TryGetUuid(GameObject gameObject)
    {
        // Directly try getting the UUID component
        if (gameObject.TryGetComponent<SpawnedObjectReference>(out var reference))
        {
            Debug.Log("[HELPER] UUID found directly: " + reference.uuid);
            return reference.uuid;
        }

        // Try getting it from children (if it's attached to a child)
        reference = gameObject.GetComponentInChildren<SpawnedObjectReference>();
        if (reference != null)
        {
            Debug.Log("[HELPER] UUID found in children: " + reference.uuid);
            return reference.uuid;
        }

        // Logging component names to verify what components are attached to the object
        Debug.LogWarning($"[HELPER] UUID NOT FOUND on {gameObject.name}. Components: " +
            string.Join(", ", gameObject.GetComponents<Component>().Select(c => c.GetType().Name)));

        return string.Empty;
    }

    /// <summary>
    /// Try to get UUID via the Collision, checking collision object for SpawnedObjectReference (which holds UUID).
    /// </summary>
    /// <param name="collision">The collided object to be identified via SpawnedObjectReference.</param>
    /// <returns></returns>
    public static string TryGetUuid(Collision collision)
    {
        // Try getting directly on the object
        if (collision.gameObject.TryGetComponent<SpawnedObjectReference>(out var reference))
        {
            Debug.Log("[HELPER] UUID found directly: " + reference.uuid);
            return reference.uuid;
        }

        Debug.LogWarning("[HELPER] No UUID found on or under the collided object.");
        return string.Empty;
    }
}
