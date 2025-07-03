using UnityEngine;

public class SyncControllerPosition : MonoBehaviour
{
    public Transform controller;
    public Vector3 positionOffset = new Vector3(0, -0.03f, 0);
    private Quaternion rotationOffset = Quaternion.Euler(0, 180, 0);

    // Update is called once per frame
    void Update()
    {
        transform.position = controller.position + positionOffset;
        transform.rotation = controller.rotation * rotationOffset;
    }
}
