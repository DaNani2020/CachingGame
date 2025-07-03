using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWandRenderOffset : MonoBehaviour
{
    public Transform renderOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = renderOffset.position;
        transform.rotation = renderOffset.rotation;

    }
}
