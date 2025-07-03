using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Add using directive for System.Linq
using UnityEngine;

public class SpellRouting : MonoBehaviour
{
    public event Action<SpawningPointData> OnSpawningPointDataCollected;

    public GameObject[] spellRoutes; // Transform of the spell route
    
    public GameObject spherePrefab; // Prefab of the sphere to instantiate

    public AudioSource SphereCollisionSound;

    public bool auraIsGrowing = true;

    public InitScene initScene;

    private Dictionary<int, List<Transform>> routes = new Dictionary<int, List<Transform>>();

    private List<KeyValuePair<int, List<Transform>>> sortedRoutes;

    private int currentRouteIndex = 0; // Increasing by 1 for each spell route - needed as index to access the correct spell route in the sortedRoutes list

    private int spellRouteCount = 1; // Increasing by 1 for each spell route

    private int currentLandmarkIndex = 0;

    private GameObject aura;

    private String landmarkNamePrefix = "lm_";

    private int totalRoutes;

    private string landMarkName;

    private SpawningPointData spawningPointData;

    private void Awake()
    {
        
        // Ensure the landmarks array is initialized
        if (spellRoutes.Length > 0)
        {
            for (int i = 0; i < spellRoutes.Length; i++)
            {
                // Get all child transforms of the SpellRoute GameObject
                Transform[] allChildren = spellRoutes[i].GetComponentsInChildren<Transform>();

                // Filter the child transforms based on the naming convention or other criteria
                Transform[] landmarks = Array.FindAll(allChildren, t => t.name.StartsWith(landmarkNamePrefix) && t != spellRoutes[i].transform);
                
                // ShowMessage(landmarks.Length + " landmarks found in the spell route "+spellRoutes[i].name);

                routes.Add(i, landmarks.ToList());
            }
            // Sort routes by their tags
            sortedRoutes = routes.OrderBy(r => GetRoutePriority(r.Value)).ToList();
            totalRoutes = sortedRoutes.Count;
        }
    }

    private void Update()
    {
        if(aura != null && auraIsGrowing)
        {
            aura.transform.localScale += new Vector3(0.3f, 0.3f, 0.3f) * Time.deltaTime;
        }
    }

    private int GetRoutePriority(List<Transform> landmarks)
    {
        // Determine the priority based on the tags of the landmarks
        if (landmarks.Any(l => l.CompareTag("LandmarkSmall")))
        {
            return 1;
        }
        else if (landmarks.Any(l => l.CompareTag("LandmarkMedium")))
        {
            return 2;
        }
        else if (landmarks.Any(l => l.CompareTag("LandmarkLarge")))
        {
            return 3;
        }
        return int.MaxValue; // Default high priority for unexpected cases
    }

    public void StartSpellRouting()
    {
        if (sortedRoutes.Count > 0)
        {
            // InstantiateSphereAtLandmark(routes[currentRouteIndex][currentLandmarkIndex]); // currentLandmarkIndex
            InstantiateSphereAtLandmark(sortedRoutes[currentRouteIndex].Value[currentLandmarkIndex]);
            landMarkName = sortedRoutes[currentRouteIndex].Value[currentLandmarkIndex].name;
        }
    }

    private void InstantiateSphereAtLandmark(Transform position) // int index
    {
        // ShowMessage("Route: "+sortedRoutes[currentRouteIndex]+" and Landmark: "+sortedRoutes[currentRouteIndex].Value[currentLandmarkIndex]);
        GameObject spawningPoint = Instantiate(spherePrefab, position.position, Quaternion.identity); // landmarks[index].position
        Transform auraTransform = spawningPoint.transform.GetChild(0);

        if(auraTransform != null)
        {
            aura = auraTransform.gameObject;
        }
        else
        {
            print("Aura not found");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is a sphere
        if (other.gameObject.CompareTag("Sphere"))
        {
            
            currentLandmarkIndex++;
            if (currentLandmarkIndex < sortedRoutes[currentRouteIndex].Value.Count)
            {
                spawningPointData = new SpawningPointData();
                // Data Serialization
                spawningPointData.SetSpawningPointData(landMarkName, new SerializableVector3(other.transform.position), DateTime.Now , new SerializableVector3(aura.transform.localScale));
                // Notify observers about the collected SpawningPointData
                OnSpawningPointDataCollected?.Invoke(spawningPointData);
                // if (DataWriter.Instance != null)
                // {
                //     DataWriter.Instance.WriteTrainedLimbData();
                //     DataWriter.Instance.WriteReferenceLimbData();
                // }
                // else
                // {
                //     ShowMessage("DataWriter.Instance is null! Make sure DataWriter is initialized and present in the scene.");
                // }
                // Data Serialization End

                Destroy(other.transform.parent.gameObject);
                InstantiateSphereAtLandmark(sortedRoutes[currentRouteIndex].Value[currentLandmarkIndex]);
                landMarkName = sortedRoutes[currentRouteIndex].Value[currentLandmarkIndex].name;
                PlayAudio();
            }
            else if (currentLandmarkIndex == sortedRoutes[currentRouteIndex].Value.Count)
            {
                spawningPointData = new SpawningPointData();
                // Data Serialization
                spawningPointData.SetSpawningPointData(landMarkName, new SerializableVector3(other.transform.position), DateTime.Now , new SerializableVector3(aura.transform.localScale));
                // Notify observers about the collected SpawningPointData
                OnSpawningPointDataCollected?.Invoke(spawningPointData);
                // if (DataWriter.Instance != null)
                // {
                //     DataWriter.Instance.WriteTrainedLimbData();
                //     DataWriter.Instance.WriteReferenceLimbData();
                // }
                // else
                // {
                //     ShowMessage("DataWriter.Instance is null! Make sure DataWriter is initialized and present in the scene.");
                // }
                // Data Serialization End

                Destroy(other.transform.parent.gameObject);
                PlayAudio();
                if (!isFinished())
                {
                    currentLandmarkIndex = 0;
                    currentRouteIndex++;
                    if (currentRouteIndex < totalRoutes)
                    {
                        InstantiateSphereAtLandmark(sortedRoutes[currentRouteIndex].Value[currentLandmarkIndex]);
                        landMarkName = sortedRoutes[currentRouteIndex].Value[currentLandmarkIndex].name;
                    }
                }
            }
        }
    }

    // Check if the spell route is finished
    private bool isFinished()
    {
        if(spellRouteCount < totalRoutes)
        {
            // ShowMessage("Spell route " + spellRouteCount + " of "+ totalRoutes +" completed.");
            spellRouteCount++;
            return false;
        }
        else
        {
            // ShowMessage("All "+ totalRoutes +" spell routes are completed.");
            if(!initScene.ReferenceInstructionPanelAlreadyShown() && !initScene.ConclusionPanelAlreadyShown())
            {
                initScene.SetReferenceInstructionPanelShownState(true);
            }
            else if (initScene.ReferenceInstructionPanelAlreadyShown() && !initScene.ConclusionPanelAlreadyShown())
            {
                initScene.SetConclusionPanelShownState(true);
                // DataWriter.Instance.WriteUserData();
            }
            return true;
        }
    }

    // Not needed right now because the "Play on Awake" option is enabled in the audio source component of the Sphere prefab
    // to customize the audio source, use this method
    public void PlayAudio()
    {
        if (SphereCollisionSound != null) // && !SphereCollisionSound.isPlaying
        {
            SphereCollisionSound.Play();
        }
    }

    public SpawningPointData GetSpawningPointData()
    {
        return spawningPointData;
    }

    void ShowMessage(string message)
    {
        Debug.Log(message);
        try
        {
            DebugText.Instance.AppendLine(message);
        }
        catch
        {
            Debug.Log("DebugText not found or disabled");
        }
    }
}
