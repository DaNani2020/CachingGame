using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The CacherRing is the outer edge of the cacher, which interacts with Bombs and pops object determined for other layers (the other hand).
/// </summary>
public class CacherRing : MonoBehaviour
{

    public int layerNumberOfCacher;
    public Rigidbody rb;
    private AudioSource sound;

    public GameObject popEffect;
    private ParticleSystem popEffectSystem;

    public UnityEvent<string, Quaternion> onBallPopped;


    /// <summary>
    /// Start method adds AudioSource, ParticleSystem, and saves layer of parent object.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        sound = GetComponent<AudioSource>();
        popEffectSystem = popEffect.GetComponent<ParticleSystem>();

        layerNumberOfCacher = transform.parent.gameObject.layer;
    }
    

    /// <summary>
    /// OnCollisionEnter the collided object is screened for its tag and layer, either popping the Collectible object or ending the game with a Bomb.
    /// </summary>
    /// <param name="collision">Collider of other gameObject.</param>
    void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log("EFFECT KABOOM CacherRing");

        if (collision.gameObject.CompareTag("Collectible"))
        {
            int layer = collision.gameObject.layer;
            bool sameLayers = layerNumberOfCacher == layer;
            UnityEngine.Debug.Log("EFFECT KABOOM CacherRing with collectible" + layer + " is same layer " + sameLayers + " as " + layerNumberOfCacher);

            collision.gameObject.SetActive(false);


            string uuid = Helper.TryGetUuid(collision);
            if (uuid == string.Empty) UnityEngine.Debug.LogError("TryGetUuid in collision collectible NOT WORKING: " + uuid + ".");

            UnityEngine.Debug.Log("EFFECT KABOOM CacherRing KILLING with collectible" + layer + " is same layer " + sameLayers + " as " + layerNumberOfCacher);

            if (!sameLayers)
            {
                popEffectSystem.Play();
                sound.Play();
            }
            SpawnedObjectTracker.instance.SetStateByUuid(uuid, ObjectInteractionState.POPPED, collision.gameObject.transform.position);

            onBallPopped.Invoke(collision.gameObject.name, collision.gameObject.transform.rotation); // Notify others that the ball was cached
            UnityEngine.Debug.Log("Position at popping " + collision.gameObject.transform.position);
            Destroy(collision.gameObject); // Destroy the game object  
        }

        if (collision.gameObject.CompareTag("Bomb"))
        {
            // Does not show cacher anymore
            ScoreManager.instance.GameOver();
            UnityEngine.Debug.Log("BatRING set to not active");
        }
    }

}
