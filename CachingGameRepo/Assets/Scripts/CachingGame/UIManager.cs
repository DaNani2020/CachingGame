using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class handeling UI elements for transition out or into game.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenuUI;
    [SerializeField] private GameObject gameOverMenuUI;

    ScoreManager gameManager;


    /// <summary>
    /// Start method adds listener to onGameOver event of overall game manager.
    /// </summary>
    void Start()
    {
        gameManager = ScoreManager.instance;
        gameManager.onGameOver.AddListener(ActivateGameOverMenuUI);
    }


    /// <summary>
    /// Starts game loop.
    /// </summary>
    public void PlayButtonHandler()
    {
        gameManager.StartGame();
    }


    /// <summary>
    /// Activates menu for the end of game loop.
    /// </summary>
    public void ActivateGameOverMenuUI()
    {
        gameOverMenuUI.SetActive(true);
    }
}
