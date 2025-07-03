using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeatSword : MonoBehaviour
{
    public LayerMask layer;
    private Vector3 previousPos;
    // Start is called before the first frame update
    void Start()
    {
        previousPos = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        //Raycast, starting from hands, is going forward with a length of 1 == Sword length
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1, layer))
        {
            if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130) 
            {
                ScoreManager.instance.AddPoints(30);
                Destroy(hit.transform.gameObject);
            }
        }
        previousPos = transform.position;
        
    }
}
