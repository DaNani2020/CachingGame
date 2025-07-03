using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The CacheScript interacts with Bombs and Collectible objects, activating script based on GameMode and object's layer.
/// </summary>
public class CacheScript : MonoBehaviour
{
    public GameObject collectionEffect;
    public GameObject popEffect;
    private ParticleSystem collectEffectSystem;
    private ParticleSystem popEffectSystem;

    public AudioClip poppingSound;
    public AudioClip collectingSound;
    private AudioSource audioSource;


    private string destructionReason = "unknown";


    public UnityEvent<int> extraPointsModifier;
    public UnityEvent<float> extraSpeedModifier;

    private int objectPoints = 1;

    public float acceptableCollisionDeviation = 30;


    /// <summary>
    /// Start method adds AudioSource and ParticleSystems for visual effects..
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        collectEffectSystem = collectionEffect.GetComponent<ParticleSystem>();
        popEffectSystem = popEffect.GetComponent<ParticleSystem>();
    }


    /// <summary>
    /// OnDestroy triggers a Debug LogError and its destruction cause for debugging purposes.
    /// </summary>
    public void OnDestroy()
    {
        Debug.LogError("Cacher is destroyed while game is active: " + ScoreManager.instance.isPlaying + "; Gameobject killed it: " + gameObject.name + "\nBecause of " + destructionReason + " Stack trace:\n" + System.Environment.StackTrace);
    }


    /// <summary>
    /// OnCollisionEnter the collided object is screened for its tag, and based with the calculated collision angle, triggers certain responses.
    /// </summary>
    /// <param name="collision">Collider of other gameObject.</param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collectible")) collision.gameObject.SetActive(false);
        destructionReason = " unknown for  now ";

        // Other object's forward in *your* local space
        Vector3 otherForwardInMyLocalSpace = transform.InverseTransformDirection(collision.transform.forward);

        // Angle between Z (0, 0, 1) and their forward in local space
        float collisionAngle = Vector3.Angle(Vector3.forward, otherForwardInMyLocalSpace);


        bool collisionAngleAcceptable = false;
        if (collision.gameObject.CompareTag("Collectible"))
        {
            if (collisionAngle > (90 - acceptableCollisionDeviation) && collisionAngle < (90 + acceptableCollisionDeviation))
            {
                collisionAngleAcceptable = true;
                Debug.Log("EFFECT Collision angle acceptable: " + collisionAngle + " degrees with " + otherForwardInMyLocalSpace + " and " + Vector3.forward);
            }
            else
            {
                Debug.Log("EFFECT Collision angle NOT acceptable: " + collisionAngle + " degrees");
            }

        }

        if (collision.gameObject.CompareTag("Cacher"))
        {
            Debug.LogError("Cacher collides with itself - Cacher");
            destructionReason = " Collision with Cacher ";
        }

        if (collision.gameObject.CompareTag("Collectible") && collisionAngleAcceptable)
        {
            string uuid = Helper.TryGetUuid(collision);
            if (uuid == string.Empty) UnityEngine.Debug.LogError("TryGetUuid in Colision Collectible NOT WORKING: " + uuid + ".");

            Debug.Log("EFFECT KABOOM Cacher with collectible and acceptable angle");
            collectEffectSystem.Emit(5);
            collectEffectSystem.Play();
            audioSource.PlayOneShot(collectingSound);

            if (collision.gameObject.name.Contains("PointsExtra"))
            {
                objectPoints = 5;
                extraPointsModifier.Invoke(objectPoints);
                //UnityEngine.Debug.Log("EFFECT Count down invoked with Cacher " + objectPoints);
            }
            else if (collision.gameObject.name.Contains("SpeedExtra"))
            {
                objectPoints = 10;
                extraSpeedModifier.Invoke(0.2f);
                //UnityEngine.Debug.Log("EFFECT SPEED invoked with Cacher, points: " + objectPoints);
            }
            else
            {
                objectPoints = 1;
            }
            ScoreManager.instance.AddPoints(objectPoints);

            SpawnedObjectTracker.instance.SetStateByUuid(uuid, ObjectInteractionState.CACHED, collision.gameObject.transform.position, collisionAngle);

            Debug.Log("Changed interaction State based on UUID " + uuid + " to " + SpawnedObjectTracker.instance.GetStateByUuid(uuid));
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Bomb"))
        {
            // Does not show chacher anymore
            gameObject.SetActive(false);
            ScoreManager.instance.GameOver();
        }
        else
        {
            if (collision.gameObject.CompareTag("Collectible") && !collisionAngleAcceptable)
            {
                Debug.Log("EFFECT KABOOM Cacher with collectible and NOT sacceptable angle");

                popEffectSystem.Play();
                audioSource.PlayOneShot(poppingSound);

                string uuid = Helper.TryGetUuid(collision);
                if (uuid == string.Empty) Debug.LogError("TryGetUuid NOT WORKING in colision Angle not acceptable   : " + uuid + ".");

                SpawnedObjectTracker.instance.SetStateByUuid(uuid, ObjectInteractionState.POPPED, collision.gameObject.transform.position, collisionAngle);
                Debug.Log("Changed interaction State POPPED based on UUID " + uuid + " to " + SpawnedObjectTracker.instance.GetStateByUuid(uuid));
                Destroy(collision.gameObject);
            }
        }
    }
}

