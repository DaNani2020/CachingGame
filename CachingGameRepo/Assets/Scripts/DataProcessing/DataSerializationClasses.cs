using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class DataSerializationClasses : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // // Serialize to JSON
        // string json = JsonConvert.SerializeObject(UserData, Formatting.Indented);

        // // Define the path to save the JSON file in the persistent data path
        // string path = Path.Combine(Application.persistentDataPath, "userData.json");

        // // Write the JSON to a file
        // File.WriteAllText(path, json);

        // Debug.Log("JSON file created successfully at: " + path);
    }
}

public class UserData
{
    // public string UserID { get; set; }
    public DateTime SimulationStartTime { get; set; }
    public DateTime SimulationEndTime { get; set; }
    public string TrainedLimb { get; set; }
    public string ReferenceLimb { get; set; }
    public InitialLimbData InitialLimbData { get; set; }
    public List<TrainedLimbData> TrainedLimbData { get; set; }
    public List<ReferenceLimbData> ReferenceLimbData { get; set; }

    public void SetUserData(DateTime simulationStartTime, DateTime simulationEndTime, string trainedLimb, string referenceLimb, InitialLimbData initialLimbData, List<TrainedLimbData> trainedLimbData, List<ReferenceLimbData> referenceLimbData){

        // UserID = "user123",
        SimulationStartTime = simulationStartTime;
        SimulationEndTime = simulationEndTime;
        TrainedLimb = trainedLimb;
        ReferenceLimb = referenceLimb;
        InitialLimbData = initialLimbData;
        TrainedLimbData = trainedLimbData;
        ReferenceLimbData = referenceLimbData;
        // ShowMessage("SpawningpointData Name: "+ trainedLimbDataCollection[trainedLimbDataCollection.Count-1].SpawningPointData.Name);
        // for (int i = 0; i < trainedLimbData.Count; i++){
        //     ShowMessage("SpawningpointData Name: "+ trainedLimbData[i].SpawningPointData.Name);
        // }
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

public class InitialLimbData
{
    public ChestData ChestData { get; set; }
    public LimbData UpperLimbHorizontal { get; set; }
    public LimbData UpperLimbVertical { get; set; }
    public LimbData ShoulderRotation { get; set; }
    public LimbData ElbowExtension { get; set; }
    public LimbData LowerLimbSupination { get; set; }

    public void SetInitialLimbData(ChestData chestData, LimbData upperLimbVertical, LimbData upperLimbHorizontal, LimbData shoulderRotation, LimbData elbowExtension, LimbData lowerLimbSupination){

        ChestData = chestData;
        UpperLimbHorizontal = upperLimbHorizontal;
        UpperLimbVertical = upperLimbVertical;
        ShoulderRotation = shoulderRotation;
        ElbowExtension = elbowExtension;
        LowerLimbSupination = lowerLimbSupination;
    }
}

public class LimbData
{
    public AngleData RightLimbData { get; set; }
    public AngleData LeftLimbData { get; set; }

    public void SetLimbData(AngleData rightLimbData, AngleData leftLimbData){

        RightLimbData = rightLimbData;
        LeftLimbData = leftLimbData;
    }
}

public class TrainedLimbData
{
    public ChestData ChestData { get; set; }
    public AngleData UpperLimbHorizontal { get; set; }
    public AngleData UpperLimbVertical { get; set; }
    public AngleData ShoulderRotation { get; set; }
    public AngleData ElbowRotation { get; set; }
    public AngleData LowerLimbRotation { get; set; }
    public SpawningPointData SpawningPointData { get; set; }

    public void SetTrainedLimbData(ChestData chestData, AngleData upperLimbVertical, AngleData upperLimbHorizontal, AngleData shoulderRotation, AngleData elbowRotation, AngleData lowerLimbRotation, SpawningPointData spawningPointData){

        ChestData = chestData;
        UpperLimbHorizontal = upperLimbHorizontal;
        UpperLimbVertical = upperLimbVertical;
        ShoulderRotation = shoulderRotation;
        ElbowRotation = elbowRotation;
        LowerLimbRotation = lowerLimbRotation;
        SpawningPointData = spawningPointData;
    }
}

public class ReferenceLimbData
{
    public ChestData ChestData { get; set; }
    public AngleData UpperLimbHorizontal { get; set; }
    public AngleData UpperLimbVertical { get; set; }
    public AngleData ShoulderRotation { get; set; }
    public AngleData ElbowExtension { get; set; }
    public AngleData LowerLimbSupination { get; set; }
    public SpawningPointData SpawningPointData { get; set; }

    public void SetReferenceLimbData(ChestData chestData, AngleData upperLimbVertical, AngleData upperLimbHorizontal, AngleData shoulderRotation, AngleData elbowExtension, AngleData lowerLimbSupination, SpawningPointData spawningPointData){

        ChestData = chestData;
        UpperLimbHorizontal = upperLimbHorizontal;
        UpperLimbVertical = upperLimbVertical;
        ShoulderRotation = shoulderRotation;
        ElbowExtension = elbowExtension;
        LowerLimbSupination = lowerLimbSupination;
        SpawningPointData = spawningPointData;
    }
}

public class ChestData
{
    public TrackerData TrackerChestData { get; set; }
    public SerializableVector3 ChestRotationEulerAngles { get; set; }

    public void SetChestData(TrackerData chestTrackerData, SerializableVector3 chestRotationEulerAngles){

        TrackerChestData = chestTrackerData;
        ChestRotationEulerAngles = chestRotationEulerAngles;
    }
}

public class AngleData
{
    public string TrackerName0 { get; set; }
    public SerializableVector3 TrackerPosition0 { get; set; }
    public SerializableQuaternion TrackerRotation0 { get; set; }
    public SerializableVector3 ComputingVectorDirection0 { get; set; }

    public string TrackerName1 { get; set; }
    public SerializableVector3 TrackerPosition1 { get; set; }
    public SerializableQuaternion TrackerRotation1 { get; set; }
    public SerializableVector3 ComputingVectorDirection1 { get; set; }

    public float JointAngle { get; set; }
    public SerializableQuaternion ChestRotation { get; set; }


    public void SetAngleData(Tuple<string, TrackerData, SerializableVector3> trackerAngleComputingData0, Tuple<string, TrackerData, SerializableVector3> trackerAngleComputingData1, float jointAngle, TrackerData chestRotation )
    {
        TrackerName0 = trackerAngleComputingData0.Item1;
        TrackerPosition0 = trackerAngleComputingData0.Item2.TrackerPosition;
        TrackerRotation0 = trackerAngleComputingData0.Item2.TrackerRotation;
        ComputingVectorDirection0 = trackerAngleComputingData0.Item3;

        TrackerName1 = trackerAngleComputingData1.Item1;
        TrackerPosition1 = trackerAngleComputingData1.Item2.TrackerPosition;
        TrackerRotation1 = trackerAngleComputingData1.Item2.TrackerRotation;
        ComputingVectorDirection1 = trackerAngleComputingData1.Item3;
        JointAngle = jointAngle;
        ChestRotation = chestRotation.TrackerRotation;
    }
}

public class TrackerData
{
    public SerializableVector3 TrackerPosition { get; set; }
    public SerializableQuaternion TrackerRotation { get; set; }

    public void SetTrackerData( SerializableVector3 trackerPosition, SerializableQuaternion trackerRotation){
        
        TrackerPosition = trackerPosition;
        TrackerRotation = trackerRotation;
    }
}

public class SpawningPointData
{
    public string Name { get; set; }
    public SerializableVector3 Position { get; set; }
    public DateTime TimestampAtCollection { get; set; }
    public SerializableVector3 SizeOfSpawningPointAura { get; set; }

    public void SetSpawningPointData(string name, SerializableVector3 position, DateTime timestampAtCollection, SerializableVector3 sizeOfSpawningPointAura){

        Name = name;
        Position = position;
        TimestampAtCollection = timestampAtCollection;
        SizeOfSpawningPointAura = sizeOfSpawningPointAura;
    }
}
