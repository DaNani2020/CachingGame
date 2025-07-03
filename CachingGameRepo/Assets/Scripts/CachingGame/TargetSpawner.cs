using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// TargetSpawner spawns objects (targets to collect) based on performance values and GameMode.
/// </summary>
public class TargetSpawner : MonoBehaviour
{
    public static TargetSpawner instance;

    [Header("Cacher objects")]
    public CacheScript cacheObject;
    public CacheScript otherCacheObject;

    [Header("Available object types and spawning points")]
    [Tooltip("Set prefabs of object types possible to spawn.")]
    public GameObject[] allBallObjects;
    [Tooltip("All object types currently spawn in spawn selection.")]
    public List<GameObject> ballObjectsList = new List<GameObject>();

    [SerializeField] private Transform ballParent;
    [Tooltip("Set spawning points (sources).")]
    public Transform[] allSpawningPoints;
    [Tooltip("All spawning points currently active (possible to select by algorithm).")]
    public List<Transform> spawningPoints;

    [Header("Spawn parameters")]
    [Tooltip("Time and therefore distance to next spawn.")]
    public float spawningDistance = 2;
    public float angleDeviation = 1;
    private float intervalAngleDeviation = 0.005f;

    private float timePlayed = 0;

    [Tooltip("Counter for time to next spawn.")]
    private float spawningTimer;
    private float flySpeed;
    private float flySpeedIncrement = 0.01f;
    private float opacityFlyingRay = 0.3f;

    private float initialSpawningDistance = 2;
    private float initialFlySpeed = 4;

    public Dictionary<string, string> quadrantRestrictions = new Dictionary<string, string>();

    
    [Tooltip("Parameters belonging to Markov Chain.")]
    private Dictionary<string, Dictionary<string, float>> transitionMatrix = new Dictionary<string, Dictionary<string, float>>();
    private GameObject lastSelectedBall = null;
    private int selectedSpawningPoint = 0;


    [Tooltip("Parameters for swapping sides in one hand GameMode (RED or BLUE).")]
    public UnityEvent onHandSwitch = new UnityEvent();
    public float delayTimerOnHandSwitch;
    public float delayPeriodOnHandSwitch = 2f;
    private bool stateChangeQueued;

    private float swapInterval = 10f;
    private float swappingTimer = 0f;
    private float swapBalancer = 0.5f;


    [Header("Performance parameters")]
    private Dictionary<string, float> overallSuccessRateOfTypes = new Dictionary<string, float>();
    private Dictionary<string, float> reachedRateOfTypes = new Dictionary<string, float>();
    public float thresholdOfSuccessRateTypesMin = 0.3f;
    public float thresholdOfSuccessRateTypesMax = 0.8f;

    private (float, float) positionOffsetsXY = (0, 0);
    private Dictionary<int, Dictionary<string, float>> reachedSuccessRateOfTypes = new Dictionary<int, Dictionary<string, float>>();
    private Dictionary<int, (float, float)> positionOffsetsXYPerSource = new Dictionary<int, (float, float)>();
    private Dictionary<int, float> angleDeviationPerSource = new Dictionary<int, float>();

    private bool dictionariesFilledByPerformanceManager = false;


    [Header("Link to progress bar")]
    public CylinderProgressBar cylinderProgressBar;
    private int currentGameLevel = 1;


    [Tooltip("Name of type for basic game object types.")]
    private string redLayerKey = "";
    private string blueLayerKey = "";
    private string bombObjectKey = "Bomb";


    /// <summary>
    /// Before start, Awake triggers initialization of essential elements and adds listeners to events of game manager (ScoreManager), cachers, and progress bar.
    /// </summary>
    private void Awake()
    {
        instance = this;
        ScoreManager.instance.onGameOver.AddListener(ClearBalls);
        ScoreManager.instance.onPlay.AddListener(ResetStoredPerformanceValues);
        ScoreManager.instance.onSettingGameMode.AddListener(InitializeTransitionMatrix);


        if (cacheObject != null)
        {
            cacheObject.extraSpeedModifier.AddListener(UpdateSpeedModifier);
            Debug.Log("EXTRA Points Listener in Scoremanager activated");
        }
        if (otherCacheObject != null)
        {
            otherCacheObject.extraSpeedModifier.AddListener(UpdateSpeedModifier);
            Debug.Log("EXTRA Points Listener in Scoremanager activated");
        }

        if (cylinderProgressBar != null)
        {
            cylinderProgressBar.newLevelReached.AddListener(UpdateGameLevel);
            Debug.Log("EXTRA Points Listener in Scoremanager activated");
        }

        SetUpLayerKeys();
        InitializeTransitionMatrix();
        InitializeSuccessRatesPerType();

        flySpeed = initialFlySpeed;
        spawningDistance = initialSpawningDistance;
        ballObjectsList.AddRange(allBallObjects);
    }

    /// <summary>
    /// Start method initializes another batch of elements before the first game.
    /// </summary>
    void Start()
    {
        InitializeQuadrantRestrictions();
        InitializeSpawningPoints();
        delayTimerOnHandSwitch = delayPeriodOnHandSwitch;
    }

    /// <summary>
    /// Update only during game active, swapping sides in GameMode Red or Blue, updating and spawning objects.
    /// </summary>
    void Update()
    {

        if (!dictionariesFilledByPerformanceManager && ScoreManager.instance.isPlaying)
        {
            dictionariesFilledByPerformanceManager = AreDictionariesFilled();

        }
        else if (ScoreManager.instance.isPlaying && dictionariesFilledByPerformanceManager)
        {
            timePlayed += Time.deltaTime;

            if (stateChangeQueued)
            {
                delayTimerOnHandSwitch -= Time.deltaTime;
                if (delayTimerOnHandSwitch <= 0f)
                {
                    onHandSwitch.Invoke();
                    stateChangeQueued = false;

                    float distanceFactor = spawningDistance / initialSpawningDistance;
                    float newSpeedFactor = initialFlySpeed / flySpeed;
                    float speedBaseDelayAdjustment = Mathf.Clamp01((flySpeed - initialFlySpeed) / 3f);
                    float baseDelay = Mathf.Lerp(1f, 2f, speedBaseDelayAdjustment);
                    delayPeriodOnHandSwitch = baseDelay * newSpeedFactor * distanceFactor;

                    delayTimerOnHandSwitch = delayPeriodOnHandSwitch;
                }
            }

            if (spawningTimer > spawningDistance)
            {
                if (swappingTimer > swapInterval)
                {
                    Debug.Log("GAME MODE SWAP HANDS in effect");
                    SwapGameMode();
                    swappingTimer = 0;
                    swapInterval = swapInterval * swapBalancer;
                    swapBalancer = 1f / swapBalancer;
                    stateChangeQueued = true;
                    Debug.Log($"GAME MODE SWAP HANDS BALANCER {swapBalancer} in effect for interval {swapInterval}");
                }

                TriggerUpdate();

                bool acceptableObjectParameters;
                int loopCounter = 0;
                do
                {
                    acceptableObjectParameters = SpawnNextObject();
                    Debug.Log("Spawning Loop Counter " + loopCounter);
                } while (!acceptableObjectParameters && loopCounter < 50);
            }

            spawningTimer += Time.deltaTime;
            swappingTimer += Time.deltaTime;
        }
    }


    /// <summary>
    /// Extracting name of key game elements and adapting them to spawn names (by adding Clone).
    /// </summary>
    private void SetUpLayerKeys()
    {
        for (int i = 0; i < allBallObjects.Length; i++)
        {
            // Updating spawning settings
            int layerIndex = allBallObjects[i].layer;
            // Convert the layer index to the layer name
            string layerName = LayerMask.LayerToName(layerIndex);
            if (layerName.Contains("Red"))
            {
                redLayerKey = allBallObjects[i].name + "(Clone)";
            }
            if (layerName.Contains("Blue"))
            {
                blueLayerKey = allBallObjects[i].name + "(Clone)";
            }
            if (allBallObjects[i].name.Contains("Bomb"))
            {
                bombObjectKey = allBallObjects[i].name + "(Clone)";
            }
        }
    }


    /// <summary>
    /// SwapGameMode changes the current GameMode with the other GameMode (Red and Blue).
    /// </summary>
    private void SwapGameMode()
    {
        ScoreManager.GameMode currentGameMode = ScoreManager.instance.GetCurrentGameMode();

        if (currentGameMode == ScoreManager.GameMode.RED)
        {
            ScoreManager.instance.SetCurrentGameMode(ScoreManager.GameMode.BLUE);
        }
        else if (currentGameMode == ScoreManager.GameMode.BLUE)
        {
            ScoreManager.instance.SetCurrentGameMode(ScoreManager.GameMode.RED);
        }
        Debug.Log($"GAME MODE SWAP HANDS in effect with {currentGameMode} to new mode {ScoreManager.instance.GetCurrentGameMode()}");
    }

    /// <summary>
    /// Check for debugging, if all necessary variables are filled to utilize the spawn mechanism.
    /// </summary>
    /// <returns>True if all necessary variables and dictionaries are filled.</returns>
    private bool AreDictionariesFilled()
    {
        bool allFilled =
            overallSuccessRateOfTypes != null && overallSuccessRateOfTypes.Count > 0 &&
            reachedRateOfTypes != null && reachedRateOfTypes.Count > 0 &&
            reachedSuccessRateOfTypes != null && reachedSuccessRateOfTypes.Count > 0 &&
            positionOffsetsXYPerSource != null && positionOffsetsXYPerSource.Count > 0 &&
            angleDeviationPerSource != null && angleDeviationPerSource.Count > 0;
        if (!allFilled) Debug.LogWarning("Some dictionaries are empty or null.");

        return allFilled;
    }

    /// <summary>
    /// Initialize quadrant restriction of object types based on their names.
    /// </summary>
    private void InitializeQuadrantRestrictions()
    {
        quadrantRestrictions[bombObjectKey] = "None";
        quadrantRestrictions[redLayerKey] = "Fourth";
        quadrantRestrictions[blueLayerKey] = "Third";
    }

    /// <summary>
    /// Resets performance and game values.
    /// Reduces only certain parameters, to adjust to player's level of performance.
    /// </summary>
    private void ResetStoredPerformanceValues()
    {
        SpawnedObjectTracker.instance.ResetTracker();
        Debug.Log("START INFO " + ScoreManager.instance.isPlaying);
        timePlayed = 0;
        flySpeed = Mathf.Lerp(flySpeed, initialFlySpeed, 0.2f);
        spawningDistance = Mathf.Lerp(spawningDistance, initialSpawningDistance, 0.2f);

        positionOffsetsXY = (0, 0);
        overallSuccessRateOfTypes.Clear();
        spawningPoints.Clear();
        angleDeviationPerSource.Clear();

        reachedRateOfTypes.Clear();
        reachedSuccessRateOfTypes.Clear();
        positionOffsetsXYPerSource.Clear();

        InitializeSpawningPoints();
        InitializeTransitionMatrix();
    }

    /// <summary>
    /// ClearBalls destroys all currently existing game objects.
    /// </summary>
    private void ClearBalls()
    {
        foreach (Transform child in ballParent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Updates at certain level every level the number of spawning points.
    /// </summary>
    /// <param name="level">The number of times + 1, the threshold of progress bar has changed.</param>
    private void UpdateGameLevel(int level)
    {
        currentGameLevel = level;

        if (currentGameLevel > 3)
        {
            InitializeSpawningPoints();
        }
    }


    /// <summary>
    /// Adds with each call one additional spawning point to active spawning points (sources).
    /// </summary>
    private void InitializeSpawningPoints()
    {
        Debug.Log("Spawning points initialize " + spawningPoints + " are " + spawningPoints.Count);
        int numberOfActiveSpawningPoints = spawningPoints.Count;
        if (allSpawningPoints.Length > numberOfActiveSpawningPoints)
        {
            spawningPoints.Add(allSpawningPoints[numberOfActiveSpawningPoints]);
            angleDeviationPerSource.Add(numberOfActiveSpawningPoints, angleDeviation);
        }
        else
        {
            Debug.LogWarning("New spawning points have to be created");
        }
    }


    /// <summary>
    /// Spawns a new object which adheres to quadrant restrictions and, if Bomb, does not collide with current player position.
    /// Spawned object gets unique identifier (UUID) and is registered in SpawnedObjectTracker.
    /// </summary>
    /// <returns>True if a game object is spawned with a UUID attached to it.</returns>
    private bool SpawnNextObject()
    {
        // Choose which balls to select and where to position
        GameObject ballObjectToSpawn = GetBallObjectByMarkovChain();
        lastSelectedBall = ballObjectToSpawn;

        selectedSpawningPoint = UnityEngine.Random.Range(0, spawningPoints.Count);
        Debug.Log($"[SPAWNINGPOINT] selected spawningpoint: {selectedSpawningPoint}");
        GameObject ball = Instantiate(ballObjectToSpawn, spawningPoints[selectedSpawningPoint]);

        bool hitsPlayer;
        int attempts = 0;
        Quaternion previousRotation = ball.transform.rotation;

        do
        {
            attempts++;

            if (attempts >= 20)
            {
                Debug.LogWarning("Couldn't find a non-hitting rotation in BallSpawner.");
                break;
            }
            ball.transform.localPosition = Vector3.zero;
            // Reset to default rotation
            ball.transform.rotation = previousRotation;

            // Calculate and apply new rotation
            Vector3 rotation = CalculateRotation(ballObjectToSpawn);
            ball.transform.Rotate(rotation.x, rotation.y, rotation.z);

            // Check if this rotation causes it to hit the player
            hitsPlayer = CheckHittingPlayer(ball.transform.position, ball.transform.forward);

            Debug.Log("SPAWNING LOOP: Current Rotation of not yet spawned ball " + rotation + " on try number " + attempts + " works " + hitsPlayer);

        } while (hitsPlayer && ballObjectToSpawn.name.Contains(bombObjectKey));


        ball.GetComponent<FlyingTarget>().maxFlySpeed = flySpeed;
        ball.GetComponent<FlyingTarget>().rayOpacity = opacityFlyingRay;
        ball.transform.parent = ballParent;

        spawningTimer -= spawningDistance;

        string generatedUuid = System.Guid.NewGuid().ToString();
        var reference = ball.GetComponent<SpawnedObjectReference>();
        if (reference == null)
        {
            reference = ball.AddComponent<SpawnedObjectReference>();
        }
        reference.uuid = generatedUuid;

        Debug.Log($"[SPAWN] {ball.name} - UUID: {generatedUuid}");

        if (ball.TryGetComponent<SpawnedObjectReference>(out var refCheck))
        {
            Debug.Log("[SPAWN CONFIRM] Component assigned Reference with UUID: " + refCheck.uuid);
        }
        else
        {
            Debug.LogError("[SPAWN ERROR] Spawned object Reference does not have UUID component!");
            return false;
        }

        SpawnedObjectTracker.instance.Register(ball, generatedUuid, selectedSpawningPoint);
        return true;
    }


    /// <summary>
    /// CheckHittingPlayer checks if a Player or player-representing tagged gameObject is in the current fly path. 
    /// </summary>
    /// <returns>True, if player-like object is hit.</returns>
    public bool CheckHittingPlayer(Vector3 origin, Vector3 direction)
    {

        if (Physics.Raycast(origin, direction, out RaycastHit hit, Mathf.Infinity))
        {
            // Check if the ray hit the player object
            if (hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "PlayerSpace" || hit.collider.gameObject.tag == "NoTargetSpace")
            {
                Debug.Log("Raycast hit the player!");
                return true;
            }
            Debug.Log("BAM - SPAWNING LOOP: Current Rotation of not yet spawned ball hits " + hit.collider.tag);

            if (hit.collider.gameObject.tag == "Player") return true;
            if (hit.collider.gameObject.tag == "PlayerSpace") return true;
            return hit.collider.CompareTag("PlayerSpace");
        }
        return false;
    }


    /// <summary>
    /// Calculates an allowed angle (rotation modified and checked by BiasMap) which will be set as the flying path.
    /// </summary>
    /// <param name="ballObjectToSpawn">The gameObject to spawn.</param>
    /// <returns>Vector3 which sets the rotation direction (angle).</returns>
    private Vector3 CalculateRotation(GameObject ballObjectToSpawn)
    {
        // Random rotation within the allowed range
        Vector3 angleRotation;
        string quadrantRestriction;
        if (quadrantRestrictions.ContainsKey(ballObjectToSpawn.name + "(Clone)"))
        {
            // Get the restriction rule for the object
            quadrantRestriction = quadrantRestrictions[ballObjectToSpawn.name + "(Clone)"];
            Debug.Log("QUADRANT restriction based on ball met.  " + ballObjectToSpawn.name + " " + quadrantRestriction);
        }
        else
        {
            quadrantRestriction = "None";
        }
        var spawnPosition = BiasMap.GenerateSpawnPosition(quadrantRestriction, angleDeviation);
        angleRotation.x = spawnPosition.Item1;
        angleRotation.y = spawnPosition.Item2;
        angleRotation.z = 0;
        return angleRotation;
    }


    /// <summary>
    /// Function to select a object type using Markov Chain logic based on the last selected object type.
    /// </summary>
    /// <returns></returns>
    private GameObject GetBallObjectByMarkovChain()
    {
        Debug.Log("MARKOV selection entered");
        ScoreManager.GameMode currentGameMode = ScoreManager.instance.GetCurrentGameMode();
        if (lastSelectedBall == null)
        {
            GameObject substituteBall = SelectGameObjectByGameMode(currentGameMode);
            return substituteBall;
        }

        // Get the name of the last selected ball
        string lastBallName = lastSelectedBall.name;
        lastBallName = lastBallName.Replace("(Clone)", "").Trim();

        // Check if transition data exists for the last selected ball
        if (!transitionMatrix.ContainsKey(lastBallName))
        {
            Debug.LogWarning("MARKOV No transition data available for " + lastBallName);
            GameObject substituteBall = SelectGameObjectByGameMode(currentGameMode);
            return substituteBall;
        }

        // Get the transition probabilities for the last selected ball
        Dictionary<string, float> transitions = transitionMatrix[lastBallName];

        // Calculate the total weight of valid transitions (the sum of all probabilities)
        float totalWeight = 0f;
        foreach (float weight in transitions.Values)
        {
            totalWeight += weight;
        }

        // Generate a random value between 0 and totalWeight to select the next ball
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        // Iterate through the transition probabilities to find the next selected ball
        float cumulativeWeight = 0f;
        foreach (var transition in transitions)
        {
            Debug.Log("MARKOV " + transition.Value + " value of " + transition);
            if (transition.Value == 0.0f)
            {
                Debug.Log("MARKOV 0 value of " + transition);
                continue;
            }
            cumulativeWeight += transition.Value;

            if (randomValue <= cumulativeWeight)
            {
                // Find the corresponding GameObject for the selected ball
                GameObject selectedBall = ballObjectsList.Find(ball => ball.name == transition.Key);
                Debug.Log("MARKOV: Selected ball object to spawn: " + selectedBall.name + " from cumulative weight " + cumulativeWeight);
                lastSelectedBall = selectedBall;  // Update the last selected ball
                return selectedBall;
            }
        }

        // Fallback if no ball was selected (shouldn't normally happen)
        Debug.LogWarning("Problem with ball selection using Markov Chain.");
        return ballObjectsList[UnityEngine.Random.Range(0, ballObjectsList.Count)];
    }


    /// <summary>
    /// Selects type of game object by current GameMode.
    /// </summary>
    /// <param name="currentGameMode">The current GameMode.</param>
    /// <returns>GameObject selected.</returns>
    private GameObject SelectGameObjectByGameMode(ScoreManager.GameMode currentGameMode)
    {
        string lastBallSubstituteName = "";
        if (currentGameMode == ScoreManager.GameMode.DUAL_HAND)
        {
            lastBallSubstituteName = (UnityEngine.Random.value < 0.5f) ? redLayerKey : blueLayerKey;
        }
        else if (currentGameMode == ScoreManager.GameMode.RED)
        {
            lastBallSubstituteName = redLayerKey;
        }
        else if (currentGameMode == ScoreManager.GameMode.BLUE)
        {
            lastBallSubstituteName = blueLayerKey;
        }
        else
        {
            Debug.LogError("No valid (or implemented) GameMode chosen.");
        }

        if (ballObjectsList == null || ballObjectsList.Count == 0)
        {
            Debug.LogError("MARKOV substitute ballObjectsList is empty or not assigned!");
            return null;
        }

        lastBallSubstituteName = lastBallSubstituteName.Replace("(Clone)", "").Trim();
        GameObject substituteBall = ballObjectsList.Find(ball => ball.name.Contains(lastBallSubstituteName));
        Debug.Log("MARKOV substitute byGameMode " + currentGameMode + " with selected name: " + lastBallSubstituteName + " with object " + substituteBall);
        return substituteBall;
    }


    /// <summary>
    /// Changes and normalizes transition matrix values of basic game elements based on given performance.
    /// </summary>
    /// <param name="newSuccessRate">The performance values collected based on object type as key.</param>
    private void UpdateTransitionMatrixBasedOnScore(Dictionary<string, float> newSuccessRate)
    {
        if (!newSuccessRate.ContainsKey(redLayerKey) || !newSuccessRate.ContainsKey(blueLayerKey))
        {
            Debug.LogWarning("Markov Missing success rates for red or blue ball.");
            return;
        }

        if (Mathf.Abs(newSuccessRate[redLayerKey] - newSuccessRate[blueLayerKey]) <= 0.4f)
        {
            Debug.Log("Markov No significant difference in red/blue success rates. Skipping update.");
            return;
        }

        string redObjectProbabilities = redLayerKey.Replace("(Clone)", "").Trim();
        string blueObjectProbabilities = blueLayerKey.Replace("(Clone)", "").Trim();

        string moreSuccessful = newSuccessRate[redLayerKey] > newSuccessRate[blueLayerKey] ? redObjectProbabilities : blueObjectProbabilities;
        string lessSuccessful = newSuccessRate[redLayerKey] > newSuccessRate[blueLayerKey] ? blueObjectProbabilities : redObjectProbabilities;

        float adjustment = 0.1f; // change this value to control how strong the effect is
        Debug.Log($"MARKOV adjustment: Boosting {lessSuccessful} due to lower success rate.");

        // Loop over all source states
        foreach (var source in transitionMatrix.Keys.ToList())
        {
            var transitions = transitionMatrix[source];

            if (!transitions.ContainsKey(moreSuccessful) || !transitions.ContainsKey(lessSuccessful))
                continue;

            // Rebalance weights between more and less successful types
            float transferAmount = Mathf.Min(adjustment, transitions[moreSuccessful]);
            transitions[moreSuccessful] -= transferAmount;
            transitions[lessSuccessful] += transferAmount;

            NormalizeMatrixRow(source, transitions);
        }

        Debug.Log("MARKOV transition matrix updated based on success rates.");
    }


    /// <summary>
    /// Changes and normalizes transition matrix values of Bomb and extra game elements based on given performance and time played.
    /// </summary>
    /// <param name="timePlayed">The time played in the current game.</param>
    /// <param name="goodPerformance">The goodPerformance determining, if performance was good enough for extra elements to occur.</param>
    private void UpdateSpecialTransitionsOverTime(float timePlayed, bool goodPerformance)
    {
        Debug.Log("MARKOV transition UPDATE over TIME " + timePlayed + goodPerformance);
        // Gradually boost BombObject to 50% probability by n * 60 seconds
        float bombBoost = Mathf.Lerp(0f, 0.5f, timePlayed / (10 * 60f));
        float extraBoost = goodPerformance ? 0.2f : 0.0f;
        float boostCap = 0.2f;

        foreach (var source in transitionMatrix.Keys.ToList())
        {
            var transitions = transitionMatrix[source];

            // Apply capped boosts
            if (transitions.ContainsKey("BombObject") && !source.Contains("BombObject"))
            {
                float newValue = transitions["BombObject"] + bombBoost * 0.02f;
                transitions["BombObject"] = Mathf.Min(newValue, boostCap);
                Debug.Log("MARKOV transition UPDATE BOMB boost " + bombBoost + " new value " + newValue + " in transitions " + transitions["BombObject"]);
            }

            if (transitions.ContainsKey("SpeedExtraObject") && currentGameLevel > 3 && !source.Contains("ExtraObject"))
            {
                float newValue = transitions["SpeedExtraObject"] + extraBoost * 0.2f;
                transitions["SpeedExtraObject"] = Mathf.Min(newValue, boostCap);
                Debug.Log("MARKOV transition UPDATE SpeedExtraObject to " + Mathf.Min(newValue, boostCap));
            }

            if (transitions.ContainsKey("PointsExtraObject") && currentGameLevel > 5 && !source.Contains("ExtraObject"))
            {
                float newValue = transitions["PointsExtraObject"] + extraBoost * 0.1f;
                transitions["PointsExtraObject"] = Mathf.Min(newValue, boostCap);
                Debug.Log("MARKOV transition UPDATE PointsExtraObject to " + Mathf.Min(newValue, boostCap));
            }

            NormalizeMatrixRow(source, transitions);
        }

        Debug.Log("MARKOV: Transition matrix updated in-place with bomb/extra boost logic.");
    }


    /// <summary>
    /// Normalizes transition matrix rows.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="transitions"></param>
    private void NormalizeMatrixRow(string source, Dictionary<string, float> transitions)
    {
        // Normalize in place (modify original dictionary)
        float total = transitions.Values.Sum();
        var keys = transitions.Keys.ToList(); // To avoid modifying during enumeration

        foreach (var key in keys)
        {
            transitions[key] = transitions[key] / total;
            Debug.Log("MARKOV: Normalize matrix in " + source + " for " + key + " for value " + transitions[key]);
        }

        // Save the normalized transitions
        transitionMatrix[source] = transitions;
    }


    /// <summary>
    /// Updates spawning parameters including flySpeed, probability and distance of spawned objects as well as visibility of fly path projection based on given preformance.
    /// </summary>
    /// <param name="newSuccessRate">The newSuccessRate includes the performance values grouped by object type.</param>
    private void UpdateSpawnParamsBasedOnSuccessRate(Dictionary<string, float> newSuccessRate)
    {

        bool extraObjectsProbability = false;

        // Update Object Speed
        if (newSuccessRate[redLayerKey] < thresholdOfSuccessRateTypesMin || newSuccessRate[blueLayerKey] < thresholdOfSuccessRateTypesMin)
        {
            opacityFlyingRay = 0.4f;
            if (flySpeed > flySpeedIncrement)
            {
                flySpeed -= flySpeedIncrement;
            }
            else
            {
                spawningDistance += flySpeedIncrement;
            }
            Debug.Log("Speed down " + flySpeed + " Success rate red " + newSuccessRate[redLayerKey] + " blue " + newSuccessRate[blueLayerKey]);
        }
        if (newSuccessRate[redLayerKey] > 0.5f || newSuccessRate[blueLayerKey] > 0.5f)
        {
            opacityFlyingRay = 0f;
        }
        if (newSuccessRate[redLayerKey] > thresholdOfSuccessRateTypesMax || newSuccessRate[blueLayerKey] > thresholdOfSuccessRateTypesMax)
        {
            flySpeed += flySpeedIncrement;
            spawningDistance -= flySpeedIncrement;
            Debug.Log("Speed up " + flySpeed + " distance down " + spawningDistance);
            extraObjectsProbability = true;
        }

        if (ScoreManager.instance.GetCurrentGameMode() == ScoreManager.GameMode.DUAL_HAND)
        {
            Debug.Log("TEST DualMode active " + ScoreManager.instance.GetCurrentGameMode());
            UpdateTransitionMatrixBasedOnScore(newSuccessRate);
        }
        UpdateSpecialTransitionsOverTime(timePlayed, extraObjectsProbability);

    }

    /// <summary>
    /// Updates spawning points positions and maximum rotation of objects based on the performance values.
    /// The performance values represent the rate of reached objects and the angles indicated objects are hard to reach. 
    /// </summary>
    private void UpdateSourcePositionBasedOnAngleInfoPerSource()
    {
        for (int i = 0; i < spawningPoints.Count; i++)
        {
            Transform spawnPoint = spawningPoints[i].transform;
            if (!reachedSuccessRateOfTypes.ContainsKey(i)) continue;
            // Overall low success rate means a general change has to be made for easier gameplay
            if (reachedSuccessRateOfTypes[i][redLayerKey] < thresholdOfSuccessRateTypesMax || reachedSuccessRateOfTypes[i][blueLayerKey] < thresholdOfSuccessRateTypesMax)
            {
                if (Math.Abs(positionOffsetsXYPerSource[i].Item1) < 0.7 * angleDeviationPerSource[i] && Math.Abs(positionOffsetsXYPerSource[i].Item2) < 0.7 * angleDeviationPerSource[i])
                {
                    angleDeviationPerSource[i] -= intervalAngleDeviation;
                }
            }

            if (reachedSuccessRateOfTypes[i][redLayerKey] > thresholdOfSuccessRateTypesMax && reachedSuccessRateOfTypes[i][blueLayerKey] > thresholdOfSuccessRateTypesMax)
            {
                angleDeviationPerSource[i] += intervalAngleDeviation;
            }

            if (reachedSuccessRateOfTypes[i][redLayerKey] < thresholdOfSuccessRateTypesMin && reachedSuccessRateOfTypes[i][blueLayerKey] < thresholdOfSuccessRateTypesMin)
            {
                angleDeviationPerSource[i] += intervalAngleDeviation;
            }

            if (Math.Abs(positionOffsetsXYPerSource[i].Item1) > 0.7 * angleDeviationPerSource[i])
            {
                Debug.Log("SPAWNINGPOINT OFFSET angle deviation for " + i + " with " + angleDeviationPerSource[i]);
                if (Math.Abs(positionOffsetsXYPerSource[i].Item2) > 0.7 * angleDeviationPerSource[i])
                {
                    // Both x and y deviations are too great
                    angleDeviationPerSource[i] -= intervalAngleDeviation;
                    Debug.Log("OFFSET Make angle deviation smaller " + i + " with " + angleDeviationPerSource[i]);
                }
                else
                {
                    spawnPoint.position = new Vector3(spawnPoint.position.x + (-1) * Math.Sign(positionOffsetsXYPerSource[i].Item1) * intervalAngleDeviation, spawnPoint.position.y, spawnPoint.position.z);
                    Debug.Log($"OFFSET Spawn point {i} moved.");
                }

            }
            else if (Math.Abs(positionOffsetsXYPerSource[i].Item2) > 0.7 * angleDeviationPerSource[i])
            {
                //Debug.Log("Y Position of Spawning Point " + spawnPoint.position + " has to be moved");
                spawnPoint.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y + (-1) * Math.Sign(positionOffsetsXYPerSource[i].Item2) * intervalAngleDeviation / 10, spawnPoint.position.z);
            }
        }

    }

    /// <summary>
    /// Updates fly speed modifier to speed up the spawning objects.
    /// </summary>
    /// <param name="extraSpeedModifierValue">The value of the extraSpeedModifier.</param>
    private void UpdateSpeedModifier(float extraSpeedModifierValue)
    {
        flySpeed = flySpeed + flySpeed * extraSpeedModifierValue;
        Debug.Log("EFFECT SPEED: in BallSpawner set to " + flySpeed + " with modifier " + extraSpeedModifierValue);
    }

    /// <summary>
    /// Initializes success rates per object type, setting all to 0.
    /// </summary>
    private void InitializeSuccessRatesPerType()
    {
        Dictionary<string, float> initialRates = new Dictionary<string, float>();
        foreach (var ballObject in ballObjectsList)
        {
            initialRates[ballObject.name + "(Clone)"] = 0f;
            Debug.Log("SPEED Set Success rate INITIALLY in Ballspawner: Check NEW success rate of types " + initialRates.Keys + " " + initialRates.Values);
        }
    }

    /// <summary>
    /// Setting success rates per type into class specific format, filling missing values with 0.
    /// </summary>
    /// <param name="newRates">The newRates of success rate per object type.</param>
    public void SetSuccessRatesPerType(Dictionary<string, float> newRates)
    {
        // Fill not yet calculated rates 
        foreach (var ballObject in ballObjectsList)
        {
            if (!newRates.ContainsKey(ballObject.name + "(Clone)"))
            {
                newRates[ballObject.name + "(Clone)"] = 0f;
            }
        }

        overallSuccessRateOfTypes = newRates;

        foreach (var item in overallSuccessRateOfTypes)
        {
            Debug.Log("SPEED Set Success rate in Ballspawner: Check set NEW success rate of types " + item);
        }
    }

    /// <summary>
    /// Triggers the update of the spawning parameters based on success rate and the angle (rotation) information of each spawning point (source).
    /// </summary>
    public void TriggerUpdate()
    {
        UpdateSpawnParamsBasedOnSuccessRate(overallSuccessRateOfTypes);
        UpdateSourcePositionBasedOnAngleInfoPerSource();
    }


    /// <summary>
    /// Setting reached rates per type into class specific format, filling missing values with 0.
    /// </summary>
    /// <param name="reachedRates">The newRates of reached rate per object type.</param>
    public void SetReachedRatesPerType(Dictionary<string, float> reachedRates)
    {
        // Fill not yet calculated rates 
        foreach (var ballObject in ballObjectsList)
        {
            if (!reachedRates.ContainsKey(ballObject.name + "(Clone)"))
            {
                reachedRates[ballObject.name + "(Clone)"] = 0f;
            }
        }

        reachedRateOfTypes = reachedRates;

        foreach (var item in reachedRateOfTypes)
        {
            Debug.Log("SPEED Set reached rate in Ballspawner: Check set NEW reached rate " + item.Value + " of types " + item);
        }
    }


    /// <summary>
    /// Setting reached rates per type into class specific format, filling missing values with 0.
    /// </summary>
    /// <param name="reachedRates">The reachedRates per object type grouped by spawning point (source).</param>
    public void SetReachedRatesPerSourceAndType(Dictionary<int, Dictionary<string, float>> reachedRates)
    {
        if (reachedRates == null)
        {
            Debug.LogWarning("reachedRates is null in SetReachedRatesPerSourceAndType!");
            reachedRates = new Dictionary<int, Dictionary<string, float>>();
        }

        for (int i = 0; i < spawningPoints.Count; i++)
        {
            if (!reachedRates.ContainsKey(i))
            {
                reachedRates[i] = new Dictionary<string, float>();
            }
            // Fill not yet calculated rates 
            foreach (var ballObject in ballObjectsList)
            {
                if (!reachedRates[i].ContainsKey(ballObject.name + "(Clone)"))
                {
                    reachedRates[i][ballObject.name + "(Clone)"] = 0f;
                }
            }

            reachedSuccessRateOfTypes = reachedRates;

            /*
            foreach (var item in reachedSuccessRateOfTypes)
            {
                Debug.Log("Spawn Point Check set NEW reached rate item collection " + item.Value + " of key " + item);

            }
            Debug.Log("Spawn Point Check keys " + reachedSuccessRateOfTypes.Keys + " and values " + reachedSuccessRateOfTypes.Values);
            Debug.Log("Spawn Point Check example " + reachedSuccessRateOfTypes[0] + " and values " + reachedSuccessRateOfTypes[0][redLayerKey]);
            */
        }
    }


    /// <summary>
    /// Setting sum of x,y position offset into class specific format.
    /// </summary>
    /// <param name="angleSumOfXandY">The total angle sum of x and y (of uncached objects).</param>
    public void SetXandYPositionOffset((float, float) angleSumOfXandY)
    {
        positionOffsetsXY = angleSumOfXandY;
        Debug.Log("OFFSET: Sums of X angle " + positionOffsetsXY.Item1 + " Y angle " + positionOffsetsXY.Item2);
    }

    /// <summary>
    /// Setting sum of x,y position offset per source into class specific format, filling missing values with 0.
    /// </summary>
    /// <param name="positionOffsetsXYForSource">The angle sum of x and y (of uncached objects) per source.</param>
    public void SetXYPositionOffsetPerSource(Dictionary<int, (float, float)> positionOffsetsXYForSource)
    {
        for (int i = 0; i < spawningPoints.Count; i++)
        {
            if (!positionOffsetsXYForSource.ContainsKey(i))
            {
                positionOffsetsXYForSource[i] = (0, 0);
            }
        }

        positionOffsetsXYPerSource = positionOffsetsXYForSource;
    }


    /// <summary>
    /// Initializes the transition matrix with values representing the probability of spawning object type based on current GameMode.
    /// </summary>
    private void InitializeTransitionMatrix()
    {
        transitionMatrix.Clear();
        Debug.Log("MARKOV initialization entered with Game Mode " + ScoreManager.instance.GetCurrentGameMode());
        ScoreManager.GameMode currentGameMode = ScoreManager.instance.GetCurrentGameMode();
        if (currentGameMode == ScoreManager.GameMode.DUAL_HAND)
        {
            Debug.Log("MARKOV initialization GAME MODE for DUAL HANDS");
            transitionMatrix["BallObjectRed"] = new Dictionary<string, float> { { "BallObjectRed", 0.3f }, { "BallObjectBlue", 0.6f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["BallObjectBlue"] = new Dictionary<string, float> { { "BallObjectRed", 0.6f }, { "BallObjectBlue", 0.3f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["BombObject"] = new Dictionary<string, float> { { "BallObjectRed", 0.5f }, { "BallObjectBlue", 0.5f }, { "BombObject", 0.0f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["SpeedExtraObject"] = new Dictionary<string, float> { { "BallObjectRed", 0.4f }, { "BallObjectBlue", 0.4f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["PointsExtraObject"] = new Dictionary<string, float> { { "BallObjectRed", 0.4f }, { "BallObjectBlue", 0.4f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
        }
        else if (currentGameMode == ScoreManager.GameMode.RED)
        {
            Debug.Log("MARKOV initialization GAME MODE for RED");
            transitionMatrix["BallObjectRed"] = new Dictionary<string, float> { { "BallObjectRed", 0.9f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["BombObject"] = new Dictionary<string, float> { { "BallObjectRed", 0.999f }, { "BombObject", 0.001f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["SpeedExtraObject"] = new Dictionary<string, float> { { "BallObjectRed", 0.9f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["PointsExtraObject"] = new Dictionary<string, float> { { "BallObjectRed", 0.9f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
        }
        else if (currentGameMode == ScoreManager.GameMode.BLUE)
        {
            transitionMatrix["BallObjectBlue"] = new Dictionary<string, float> { { "BallObjectBlue", 0.9f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["BombObject"] = new Dictionary<string, float> { { "BallObjectBlue", 0.999f }, { "BombObject", 0.001f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["SpeedExtraObject"] = new Dictionary<string, float> { { "BallObjectBlue", 0.9f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
            transitionMatrix["PointsExtraObject"] = new Dictionary<string, float> { { "BallObjectBlue", 0.9f }, { "BombObject", 0.1f }, { "SpeedExtraObject", 0.0f }, { "PointsExtraObject", 0.0f } };
        }
        else
        {
            Debug.LogError("No correct Transition Matrix for MARKOV initialization according to Game Mode found.");
        }
    }
}