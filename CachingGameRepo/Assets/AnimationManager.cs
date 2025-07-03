using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AnimationManager : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Stop(); // Cannot set duration whilst Particle System is playing

        var main = ps.main;
        main.duration = 10.0f;

        ps.Play();
    }
}
