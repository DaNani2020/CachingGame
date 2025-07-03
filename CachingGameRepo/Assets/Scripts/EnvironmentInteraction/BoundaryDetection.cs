using UnityEngine;

public class VRBoundaryDetection : MonoBehaviour
{
    public Transform cameraRig = null; // Reference to the VR camera transform

    public SpellRouting spellRoutingLeft; // Reference to the SpellRouting script for the Left hand

    public SpellRouting spellRoutingRight; // Reference to the SpellRouting script for the Right hand

    public GameObject sphere = null; // Reference to the sphere object in the scene

    public GameObject sphereAura = null; // Reference to the sphere aura object in the scene

    private string playerPrefs; // Reference to the player preferences

    void Start() 
    {
        playerPrefs = PlayerPrefs.GetString("SelectedArm");
        cameraRig = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == cameraRig)
        {
            sphere = GameObject.FindGameObjectsWithTag("Sphere")[0];
            sphereAura = GameObject.FindGameObjectsWithTag("Sphere")[1];
            if (playerPrefs == "Left")
            {
                spellRoutingLeft.auraIsGrowing =false;
                sphere.SetActive(false);
                sphereAura.SetActive(false);
            }
            else if (playerPrefs == "Right")
            {
                spellRoutingRight.auraIsGrowing = false;
                sphere.SetActive(false);
                sphereAura.SetActive(false);
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == cameraRig)
        {
            if (playerPrefs == "Left" && sphere != null && sphereAura != null)
            {
                sphere.SetActive(true);
                sphereAura.SetActive(true);
                spellRoutingLeft.auraIsGrowing = true;
            }
            else if (playerPrefs == "Right" && sphere != null && sphereAura != null)
            {
                sphere.SetActive(true);
                sphereAura.SetActive(true);
                spellRoutingRight.auraIsGrowing = true;
            }
        }
    }

    void ShowMessage(string message)
    {
        Debug.Log(message);
        DebugText.Instance.AppendLine(message);
    }
}
