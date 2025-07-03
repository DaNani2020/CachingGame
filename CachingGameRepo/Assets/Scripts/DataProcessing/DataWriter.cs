using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using UnityEngine;
using Wave.Essence;
using Wave.Native;

// This class is responsible for measuring the date at the CORRECT time and store it in the corresponding serial classesto write it into the database
// The instances of the serialization classes will always be overwritten with the latest data
public class DataWriter : MonoBehaviour
{
    // Database instance
    private IMongoDatabase database;

    // Singleton instance
    public static DataWriter Instance { get; private set; }

    public Transform trackerChest; // The reference point, e.g., the spine or chest
    public Transform trackerElbowLeft; // The right elbow tracker
    public Transform controllerLeft; // The right controller
    public Transform trackerElbowRight; // The right elbow tracker
    public Transform controllerRight; // The right controller

    // References of Angle Measurement scripts
    public UpperLimbAngleCalculator upperLimbAngleCalculator;
    public InitScene initScene;
    private SpellRouting spellRoutingDominant;
    private SpellRouting spellRoutingNonDominant;

    // Further required variables for the user data
    private string trainedLimb;
    private string referenceLimb;
    private DateTime simulationStartTime;
    private DateTime simulationEndTime;

    // Serialization classes
    private TrackerData trackerChestData;
    private TrackerData trackerElbowRightData;
    private TrackerData controllerRightData;
    private TrackerData trackerElbowLeftData;
    private TrackerData controllerLeftData;

    private UserData userData;
    private InitialLimbData initialLimbData;
    private TrainedLimbData trainedLimbData;
    private List<TrainedLimbData> trainedLimbDataCollection;
    private ReferenceLimbData referenceLimbData;
    private List<ReferenceLimbData> referenceLimbDataCollection;
    private SpawningPointData spawningPointData;
    // private SpawningPointData spawningPointDataReference;

    private ChestData chestMotionData;
    private AngleData rightUpperLimbVerticalMotionData;
    private AngleData rightUpperLimbHorizontalMotionData;
    private AngleData rightShoulderRotationMotionData;
    private AngleData rightElbowExtensionMotionData;
    private AngleData rightLowerLimbSupinationMotionData;

    private AngleData leftUpperLimbVerticalMotionData;
    private AngleData leftUpperLimbHorizontalMotionData;
    private AngleData leftShoulderRotationMotionData;
    private AngleData leftElbowExtensionMotionData;
    private AngleData leftLowerLimbSupinationMotionData;

    private LimbData upperLimbHorizontalMotionData;
    private LimbData upperLimbVerticalMotionData;
    private LimbData shoulderRotationMotionData;
    private LimbData elbowExtensionMotionData;
    private LimbData lowerLimbSupinationMotionData;

    // Further Variables
    private bool isDominantSpellRoutingSubscribed = false;
    private bool isNonDominantSpellRoutingSubscribed = false;
    private int trainedReferenceOrderCnt = 0; // 1 is for the trainedArm and 2 is for the referenceArm to determine which arm is training and which is the reference
    private bool isUserDataWritten = false;

    private String mongoConnectionString; // Connection string for MongoDB

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Assign the current object to the static Instance
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates if another instance already exists
        }
    }


    void Start()
    {    
        try
        {
            TextAsset textAsset = Resources.Load<TextAsset>("mongo_connection_string"); // Omit the file extension
            if (textAsset != null)
            {
                // Fill in connection string details
                mongoConnectionString = textAsset.text ; // Standard connection string without DNS resolution (srv) - connecting directly to a replica set via nodes
            }
            else
            {
                ShowMessage("Failed to load the file from Resources or file <mongo_connection_string.txt> does not exist.");
            }

            ShowMessage("Connection:string: " + mongoConnectionString);
            var settings = MongoClientSettings.FromConnectionString(mongoConnectionString);

            // Connect to MongoDB
            var client = new MongoClient(settings); // Localhost: "mongodb://localhost:27017" (For local MongoDB server via DirectPreview)

            database = client.GetDatabase("vr_movement_db");
            ShowMessage("Database initialized: " + (database != null));

        }
        catch (Exception e){
            ShowMessage("An error occured while initializing the database: " + e.Message);
        }

        trainedLimb = PlayerPrefs.GetString("SelectedArm");
        referenceLimb = (trainedLimb == "Right") ? "Left" : "Right";

        // Chest
        if (trackerChest == null)
        {
            trackerChest = GameObject.FindGameObjectWithTag("ChestTracker").transform.childCount > 0 ? GameObject.FindGameObjectWithTag("ChestTracker").transform.GetChild(0).transform : GameObject.FindGameObjectWithTag("ChestTracker").transform;
        }

        // Right Upper Limb
        if (trackerElbowRight == null)
        {
            trackerElbowRight = GameObject.FindGameObjectWithTag("ElbowRightTracker").transform.childCount > 0 ? GameObject.FindGameObjectWithTag("ElbowRightTracker").transform.GetChild(0).transform : GameObject.FindGameObjectWithTag("ElbowRightTracker").transform;
        }
        if (controllerRight == null)
        {
            controllerRight = GameObject.FindGameObjectWithTag("DominantTracker").transform;
        }

        // Left Upper Limb
        if (trackerElbowLeft == null)
        {
            trackerElbowLeft = GameObject.FindGameObjectWithTag("ElbowLeftTracker").transform.childCount > 0 ? GameObject.FindGameObjectWithTag("ElbowLeftTracker").transform.GetChild(0).transform : GameObject.FindGameObjectWithTag("ElbowLeftTracker").transform;
        }
        if (controllerLeft == null)
        {
            controllerLeft = GameObject.FindGameObjectWithTag("NonDominantTracker").transform;
        }

        spellRoutingDominant = GameObject.FindGameObjectWithTag("DominantSpellRouting").GetComponent<SpellRouting>();
        spellRoutingNonDominant = GameObject.FindGameObjectWithTag("NonDominantSpellRouting").GetComponent<SpellRouting>();
 

        // Initialize the serialization classes
        trackerChestData = new TrackerData();
        trackerElbowRightData = new TrackerData();
        controllerRightData = new TrackerData();
        trackerElbowLeftData = new TrackerData();
        controllerLeftData = new TrackerData();	

        trainedLimbDataCollection = new List<TrainedLimbData>();
        referenceLimbDataCollection = new List<ReferenceLimbData>();
    }

    void Update()
    {
        trackerChestData.SetTrackerData(new SerializableVector3(trackerChest.position), new SerializableQuaternion(trackerChest.rotation));
        trackerElbowRightData.SetTrackerData(new SerializableVector3(trackerElbowRight.position), new SerializableQuaternion(trackerElbowRight.rotation));
        controllerRightData.SetTrackerData(new SerializableVector3(controllerRight.position), new SerializableQuaternion(controllerRight.rotation));
        trackerElbowLeftData.SetTrackerData(new SerializableVector3(trackerElbowLeft.position), new SerializableQuaternion(trackerElbowLeft.rotation));
        controllerLeftData.SetTrackerData(new SerializableVector3(controllerLeft.position), new SerializableQuaternion(controllerLeft.rotation));

        if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Left, WVR_InputId.WVR_InputId_Alias1_Y))
        {

        }

        if (WXRDevice.ButtonPress(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_B))
        {

        }

        if (initScene.ReferenceInstructionPanelAlreadyShown() && initScene.ConclusionPanelAlreadyShown() && !isUserDataWritten)
        {
            WriteUserData();
            isUserDataWritten = true;
        }

        // Correct SpellRoute reference to the correct hand
        if(spellRoutingDominant != null && spellRoutingDominant.isActiveAndEnabled && !isDominantSpellRoutingSubscribed)
        {
            isDominantSpellRoutingSubscribed = true;
            isNonDominantSpellRoutingSubscribed = false;
            trainedReferenceOrderCnt++;
            // Unsubscribe from the NonDominant Controller event
            spellRoutingNonDominant.OnSpawningPointDataCollected -= SpawningPointDataObserver;
            // Subscribe to the event
            spellRoutingDominant.OnSpawningPointDataCollected += SpawningPointDataObserver;
            
        } 
        
        if(spellRoutingNonDominant != null && spellRoutingNonDominant.isActiveAndEnabled && !isNonDominantSpellRoutingSubscribed)
        {
            isNonDominantSpellRoutingSubscribed = true;
            isDominantSpellRoutingSubscribed = false;
            trainedReferenceOrderCnt++;
            // Unsubscribe from the Dominant Controller event
            spellRoutingDominant.OnSpawningPointDataCollected -= SpawningPointDataObserver;
            // Subscribe to the event
            spellRoutingNonDominant.OnSpawningPointDataCollected += SpawningPointDataObserver;
        }
    }

#region Write chest data
    public void WriteChestData()
    {   
        chestMotionData = new ChestData();
        Vector3 chestRotation = upperLimbAngleCalculator.ChestRotationMeasurement(trackerChest);
        
        chestMotionData.SetChestData(trackerChestData, new SerializableVector3(chestRotation));
    }
#endregion

#region Write left limb data
    public void WriteLeftShoulderHorizontalData()
    {
        leftUpperLimbHorizontalMotionData = new AngleData();
        (float angle, Vector3 horizontalChestDirection, Vector3 horizontalElbowDirection) = upperLimbAngleCalculator.angleShoulderHorizontalLeft(trackerElbowLeft, trackerChest);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowLeft.parent.name, trackerElbowLeftData, new SerializableVector3(horizontalElbowDirection));
        Tuple<string, TrackerData, SerializableVector3> trackerChestComputingData = Tuple.Create(trackerChest.parent.name, trackerChestData, new SerializableVector3(horizontalChestDirection));
        leftUpperLimbHorizontalMotionData.SetAngleData(trackerElbowComputingData, trackerChestComputingData, angle, trackerChestData);
    }
    public void WriteLeftShoulderVerticalData()
    {
        leftUpperLimbVerticalMotionData = new AngleData();
        (float angle, Vector3 verticalChestDirection, Vector3 verticalElbowDirection) = upperLimbAngleCalculator.angleShoulderVerticalLeft(trackerElbowLeft, trackerChest);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowLeft.parent.name, trackerElbowLeftData, new SerializableVector3(verticalElbowDirection));
        Tuple<string, TrackerData, SerializableVector3> trackerChestComputingData = Tuple.Create(trackerChest.parent.name, trackerChestData, new SerializableVector3(verticalChestDirection));
        leftUpperLimbVerticalMotionData.SetAngleData(trackerElbowComputingData, trackerChestComputingData, angle, trackerChestData);
    }

    public void WriteLeftShoulderRotationData()
    {
        leftShoulderRotationMotionData = new AngleData();
        (float angle, Vector3 shoulderDirection, Vector3 elbowDirection) = upperLimbAngleCalculator.shoulderRotationLeft(trackerElbowLeft, trackerChest);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowLeft.parent.name, trackerElbowLeftData, new SerializableVector3(elbowDirection));
        Tuple<string, TrackerData, SerializableVector3> trackerChestComputingData = Tuple.Create(trackerChest.parent.name, trackerChestData, new SerializableVector3(shoulderDirection));
        leftShoulderRotationMotionData.SetAngleData(trackerElbowComputingData, trackerChestComputingData, angle, trackerChestData);
    }

    public void WriteLeftElbowExtensionData()
    {
        leftElbowExtensionMotionData = new AngleData();
        (float angle,  Vector3 upperLimbDirection, Vector3 lowerLimbDirection) = upperLimbAngleCalculator.angleElbowExtensionLeft(trackerElbowLeft, controllerLeft);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowLeft.parent.name, trackerElbowLeftData, new SerializableVector3(upperLimbDirection));
        Tuple<string, TrackerData, SerializableVector3> controllerComputingData = Tuple.Create(controllerLeft.name, controllerLeftData, new SerializableVector3(lowerLimbDirection));
        leftElbowExtensionMotionData.SetAngleData(controllerComputingData, trackerElbowComputingData, angle, trackerChestData);
    }

    public void WriteLeftLowerLimbSupinationData()
    {
        leftLowerLimbSupinationMotionData = new AngleData();
        (float angle, Vector3 upperLimbDirection, Vector3 lowerLimbDirection) = upperLimbAngleCalculator.elbowSupinationLeft(trackerElbowLeft, controllerLeft);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowLeft.parent.name, trackerElbowLeftData, new SerializableVector3(upperLimbDirection));
        Tuple<string, TrackerData, SerializableVector3> controllerComputingData = Tuple.Create(controllerLeft.name, controllerLeftData, new SerializableVector3(lowerLimbDirection));
        leftLowerLimbSupinationMotionData.SetAngleData(controllerComputingData, trackerElbowComputingData, angle, trackerChestData);
    }
#endregion

#region Write right limb data
    public void WriteRightShoulderHorizontalData()
    {
        rightUpperLimbHorizontalMotionData = new AngleData();
        (float angle, Vector3 horizontalChestDirection, Vector3 horizontalElbowDirection) = upperLimbAngleCalculator.angleShoulderHorizontalRight(trackerElbowRight, trackerChest);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowRight.parent.name, trackerElbowRightData, new SerializableVector3(horizontalElbowDirection));
        Tuple<string, TrackerData, SerializableVector3> trackerChestComputingData = Tuple.Create(trackerChest.parent.name, trackerChestData, new SerializableVector3(horizontalChestDirection));
        rightUpperLimbHorizontalMotionData.SetAngleData(trackerElbowComputingData, trackerChestComputingData, angle, trackerChestData);
    }

    public void WriteRightShoulderVerticalData()
    {
        rightUpperLimbVerticalMotionData = new AngleData();
        (float angle, Vector3 verticalChestDirection, Vector3 verticalElbowDirection) = upperLimbAngleCalculator.angleShoulderVerticalRight(trackerElbowRight, trackerChest);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowRight.parent.name, trackerElbowRightData, new SerializableVector3(verticalElbowDirection));
        Tuple<string, TrackerData, SerializableVector3> trackerChestComputingData = Tuple.Create(trackerChest.parent.name, trackerChestData, new SerializableVector3(verticalChestDirection));
        rightUpperLimbVerticalMotionData.SetAngleData(trackerElbowComputingData, trackerChestComputingData, angle, trackerChestData);
    }

    public void WriteRightShoulderRotationData()
    {
        rightShoulderRotationMotionData = new AngleData();
        (float angle, Vector3 shoulderDirection, Vector3 elbowDirection) = upperLimbAngleCalculator.shoulderRotationRight(trackerElbowRight, trackerChest);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowRight.parent.name, trackerElbowRightData, new SerializableVector3(elbowDirection));
        Tuple<string, TrackerData, SerializableVector3> trackerChestComputingData = Tuple.Create(trackerChest.parent.name, trackerChestData, new SerializableVector3(shoulderDirection));
        rightShoulderRotationMotionData.SetAngleData(trackerElbowComputingData, trackerChestComputingData, angle, trackerChestData);
    }

    public void WriteRightElbowExtensionData()
    {
        rightElbowExtensionMotionData = new AngleData();
        (float angle,  Vector3 upperLimbDirection, Vector3 lowerLimbDirection) = upperLimbAngleCalculator.angleElbowExtensionRight(trackerElbowRight, controllerRight);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowRight.parent.name, trackerElbowRightData, new SerializableVector3(upperLimbDirection));
        Tuple<string, TrackerData, SerializableVector3> controllerComputingData = Tuple.Create(controllerRight.name, controllerRightData, new SerializableVector3(lowerLimbDirection));
        rightElbowExtensionMotionData.SetAngleData(controllerComputingData, trackerElbowComputingData, angle, trackerChestData);
    }

    public void WriteRightLowerLimbSupinationData()
    {
        rightLowerLimbSupinationMotionData = new AngleData();
        (float angle, Vector3 upperLimbDirection, Vector3 lowerLimbDirection) = upperLimbAngleCalculator.elbowSupinationRight(trackerElbowRight, controllerRight);
        Tuple<string, TrackerData, SerializableVector3> trackerElbowComputingData = Tuple.Create(trackerElbowRight.parent.name, trackerElbowRightData, new SerializableVector3(upperLimbDirection));
        Tuple<string, TrackerData, SerializableVector3> controllerComputingData = Tuple.Create(controllerRight.name, controllerRightData, new SerializableVector3(lowerLimbDirection));
        rightLowerLimbSupinationMotionData.SetAngleData(controllerComputingData, trackerElbowComputingData, angle, trackerChestData);
    }
#endregion

#region Write Left and Right Limb data together - Used for the initial data
    public void WriteUpperLimbHorizontalData()
    {
        upperLimbHorizontalMotionData = new LimbData();
        WriteRightShoulderHorizontalData();
        WriteLeftShoulderHorizontalData();
        upperLimbHorizontalMotionData.SetLimbData(rightUpperLimbHorizontalMotionData, leftUpperLimbHorizontalMotionData);
    }

    public void WriteUpperLimbVerticalData()
    {
        upperLimbVerticalMotionData = new LimbData();
        WriteRightShoulderVerticalData();
        WriteLeftShoulderVerticalData();
        upperLimbVerticalMotionData.SetLimbData(rightUpperLimbVerticalMotionData, leftUpperLimbVerticalMotionData);
    }

    public void WriteShoulderRotationData()
    {
        shoulderRotationMotionData = new LimbData();
        WriteRightShoulderRotationData();
        WriteLeftShoulderRotationData();
        shoulderRotationMotionData.SetLimbData(rightShoulderRotationMotionData, leftShoulderRotationMotionData);
    }

    public void WriteElbowExtensionData()
    {
        elbowExtensionMotionData = new LimbData();
        WriteRightElbowExtensionData();
        WriteLeftElbowExtensionData();
        elbowExtensionMotionData.SetLimbData(rightElbowExtensionMotionData, leftElbowExtensionMotionData);
    }

    public void WriteLowerLimbSupinationData()
    {
        lowerLimbSupinationMotionData = new LimbData();
        WriteRightLowerLimbSupinationData();
        WriteLeftLowerLimbSupinationData();
        lowerLimbSupinationMotionData.SetLimbData(rightLowerLimbSupinationMotionData, leftLowerLimbSupinationMotionData);
    }
#endregion

#region Write Initial Data
    public void WriteInitialLimbData()
    {
        initialLimbData = new InitialLimbData();
        upperLimbAngleCalculator.SetInitialChestRotation(trackerChest.rotation);
        simulationStartTime = DateTime.Now;
        // Initial Writing of the motion data
        WriteChestData();
        WriteUpperLimbHorizontalData();
        WriteUpperLimbVerticalData();
        WriteShoulderRotationData();
        WriteElbowExtensionData();
        WriteLowerLimbSupinationData();
        initialLimbData.SetInitialLimbData(chestMotionData, upperLimbHorizontalMotionData, upperLimbVerticalMotionData, shoulderRotationMotionData, elbowExtensionMotionData, lowerLimbSupinationMotionData);
    }

#endregion

#region Write Spawning Point Data
    public void SpawningPointDataObserver(SpawningPointData spawningPointDataSubscriptionObject)
    {
        spawningPointData = new SpawningPointData();
        spawningPointData = spawningPointDataSubscriptionObject;
        WriteTrainedLimbData();
        WriteReferenceLimbData();
    }


#endregion

#region Write Trained Limb Data
    public void WriteTrainedLimbData()
    {
        trainedLimbData = new TrainedLimbData();
        if (trainedLimb == "Right" && trainedReferenceOrderCnt == 1)
        {
            WriteChestData();
            WriteRightShoulderHorizontalData();
            WriteRightShoulderVerticalData();
            WriteRightShoulderRotationData();
            WriteRightElbowExtensionData();
            WriteRightLowerLimbSupinationData();
            trainedLimbData.SetTrainedLimbData(chestMotionData, rightUpperLimbHorizontalMotionData, rightUpperLimbVerticalMotionData, rightShoulderRotationMotionData, rightElbowExtensionMotionData, rightLowerLimbSupinationMotionData, spawningPointData);
            trainedLimbDataCollection.Add(trainedLimbData);
        }
        if (trainedLimb == "Left" && trainedReferenceOrderCnt == 1)
        {
            WriteChestData();
            WriteLeftShoulderHorizontalData();
            WriteLeftShoulderVerticalData();
            WriteLeftShoulderRotationData();
            WriteLeftElbowExtensionData();
            WriteLeftLowerLimbSupinationData();
            trainedLimbData.SetTrainedLimbData(chestMotionData, leftUpperLimbHorizontalMotionData, leftUpperLimbVerticalMotionData, leftShoulderRotationMotionData, leftElbowExtensionMotionData, leftLowerLimbSupinationMotionData, spawningPointData);
            trainedLimbDataCollection.Add(trainedLimbData);
        }
    }

#endregion

#region Write Reference Limb Data
    public void WriteReferenceLimbData()
    {
        referenceLimbData = new ReferenceLimbData();
        if (referenceLimb == "Right" && trainedReferenceOrderCnt == 2)
        {
            WriteChestData();
            WriteRightShoulderHorizontalData();
            WriteRightShoulderVerticalData();
            WriteRightShoulderRotationData();
            WriteRightElbowExtensionData();
            WriteRightLowerLimbSupinationData();
            referenceLimbData.SetReferenceLimbData(chestMotionData, rightUpperLimbHorizontalMotionData, rightUpperLimbVerticalMotionData, rightShoulderRotationMotionData, rightElbowExtensionMotionData, rightLowerLimbSupinationMotionData, spawningPointData);
            referenceLimbDataCollection.Add(referenceLimbData);
        }
        if (referenceLimb == "Left" && trainedReferenceOrderCnt == 2)
        {
            WriteChestData();
            WriteLeftShoulderHorizontalData();
            WriteLeftShoulderVerticalData();
            WriteLeftShoulderRotationData();
            WriteLeftElbowExtensionData();
            WriteLeftLowerLimbSupinationData();
            referenceLimbData.SetReferenceLimbData(chestMotionData, leftUpperLimbHorizontalMotionData, leftUpperLimbVerticalMotionData, leftShoulderRotationMotionData, leftElbowExtensionMotionData, leftLowerLimbSupinationMotionData, spawningPointData);
            referenceLimbDataCollection.Add(referenceLimbData);
        }
    }

#endregion

#region Write User Data
    public void WriteUserData()
    {
        userData = new UserData();
        simulationEndTime = DateTime.Now;
        userData.SetUserData(simulationStartTime, simulationEndTime, trainedLimb, referenceLimb, initialLimbData, trainedLimbDataCollection, referenceLimbDataCollection);
        
        // Insert the UserData into the database
        try
        {
            // Insert the UserData into MongoDB
            ObjectId insertedId = InsertUserData(userData);
            ShowMessage("UserData inserted with ID: " + insertedId);

        }
        catch (Exception e)
        {
            ShowMessage("Error inserting UserData into MongoDB: " + e);
        }
        
        // Showing data as json
        // try
        // {
        //     string json = JsonConvert.SerializeObject(userData, Formatting.Indented);
        //     // Get the path to the persistent data folder
        //     string filePath = Path.Combine(Application.persistentDataPath, "UserData.json");

        //     // Write the JSON to a file
        //     File.WriteAllText(filePath, json);
        //     ShowMessage("JSON file created successfully at: " + filePath);
        // }
        // catch (Exception e)
        // {
        //     ShowMessage("Error in serializing the UserData: "+e);
        //     Debug.Log("Error in serializing the UserData: "+e);
        // }

    }

#endregion

#region Insert User Data into the Database

    private ObjectId InsertUserData(UserData userData) // UserData userData
    {
        var userDataCollection = database.GetCollection<BsonDocument>("UserData");
        ShowMessage("Collection fetched: " + (userDataCollection != null));

        // Serialize the UserData object to BsonDocument using the MongoDB Bson serializer
        var userDataDocument = userData.ToBsonDocument();
        ShowMessage("UserData: " + (userData != null ? JsonConvert.SerializeObject(userData) : "null"));


        // Insert the serialized document into the collection
        userDataCollection.InsertOne(userDataDocument);

        // Return the ObjectId of the inserted document
        return userDataDocument["_id"].AsObjectId;
    }

#endregion

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