using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Managing the single components of the CacherSystem, by activating and deactivating them.
/// CacherSystem additionally allows for de- and activating cacher based on GameMode during game. 
/// </summary>
public class CacherSystem : MonoBehaviour
{
    public GameObject cacherObject;
    public GameObject cacherBackside;

    [Tooltip("Assign the layer to apply (and therefore the hand interactable) to this object")]
    public int layerToAssign;
    private string assignedLayerName;


    /// <summary>
    /// Start method adds listener to onPlay, onGameOver, and onHandSwitch event of overall game manager to activate and deactivate cacher components.
    /// </summary>
    void Start()
    {
        ScoreManager.instance.onPlay.AddListener(ActivateCache);
        ScoreManager.instance.onGameOver.AddListener(DeactivateCache);
        TargetSpawner.instance.onHandSwitch.AddListener(ActivateCache);

        gameObject.layer = layerToAssign;
        assignedLayerName = LayerMask.LayerToName(layerToAssign);

        DeactivateCache();
    }

    /// <summary>
    /// ActivateCache activates cacher based on the current GameMode.
    /// It utilies the LayerName to differentiate which side to activate or deactivate if not in DUAL_HAND GameMode.
    /// </summary>
    private void ActivateCache()
    {
        if (ScoreManager.instance.GetCurrentGameMode() != ScoreManager.GameMode.DUAL_HAND)
        {
            if (ScoreManager.instance.GetCurrentGameMode() == ScoreManager.GameMode.RED && assignedLayerName.Contains("Blue"))
            {
                gameObject.SetActive(false);
                cacherBackside.SetActive(false);
                cacherObject.SetActive(false);
            }
            else if (ScoreManager.instance.GetCurrentGameMode() == ScoreManager.GameMode.BLUE && assignedLayerName.Contains("Red"))
            {
                gameObject.SetActive(false);
                cacherBackside.SetActive(false);
                cacherObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                cacherBackside.SetActive(true);
                cacherObject.SetActive(true);
            }
        }
        else
        {
            gameObject.SetActive(true);
            cacherBackside.SetActive(true);
            cacherObject.SetActive(true);
        }
    }

    /// <summary>
    /// Deactivates all cacher components.
    /// </summary>
    private void DeactivateCache()
    {
        gameObject.SetActive(false);
        cacherBackside.SetActive(false);
        cacherObject.SetActive(false);
    }

}

