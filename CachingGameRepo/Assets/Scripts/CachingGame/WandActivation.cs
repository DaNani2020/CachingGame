using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activation and deactivation of wand for interaction with game menu based on ScoreManager event.
/// </summary>
public class WandActivation : MonoBehaviour
{
    /// <summary>
    /// Start method adds listener to onPlay and onGameOver event of overall game manager.
    /// </summary>
    void Start()
    {
        // As soon as onPlay is called, wand will be activated
        ScoreManager.instance.onPlay.AddListener(DeactivateWand);
        // As soon as onGameOver is called, wand will be deactivated
        ScoreManager.instance.onGameOver.AddListener(ActivateWand);
    }

    /// <summary>
    /// Activates wand object.
    /// </summary>
    private void ActivateWand()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates wand object.
    /// </summary>
    private void DeactivateWand()
    {
        gameObject.SetActive(false);
    }
}
