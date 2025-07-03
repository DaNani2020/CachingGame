using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


/// <summary>
/// This class manages the game, including the GameMode, scores and state of the game.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public CacheScript cacheObject;
    public CacheScript otherCacheObject;

    public Text scoreText;
    public Text highScoreText;

    public int score = 0;
    private int highScore = 0;
    public int highscoreOfPlayer;

    private float effectDuration = 10;
    private float extraEffectCountDownPoints = 0;
    private int pointMultiplier = 1;

    /// <summary>
    /// Provides all possible GameModes.
    /// </summary>
    public enum GameMode
    {
        DUAL_HAND,
        RED,
        BLUE,
        DEFAULT_NONE
    }
    private GameMode currentGameMode = GameMode.DUAL_HAND;


    // For Game Loop
    public bool isPlaying = false;
    public UnityEvent onPlay = new UnityEvent();
    public UnityEvent onGameOver = new UnityEvent();
    public UnityEvent onSettingGameMode = new UnityEvent();


    private void Awake()
    {
        if (instance == null) instance = this;
    }

    /// <summary>
    /// Start method adds listener to extraPointModifier event of cachers and sets score display to respective scores.
    /// </summary>
    void Start()
    {
        if (cacheObject != null)
        {
            cacheObject.extraPointsModifier.AddListener(UpdatePointsModifier);
        }
        if (otherCacheObject != null)
        {
            otherCacheObject.extraPointsModifier.AddListener(UpdatePointsModifier);
        }

        highScore = PlayerPrefs.GetInt("highscore", 0);
        highscoreOfPlayer = highScore;

        scoreText.text = score.ToString() + " POINTS";
        highScoreText.text = "HIGHSCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Update method checks during game, if point multiplier is still in effect.
    /// </summary>
    void Update()
    {
        if (extraEffectCountDownPoints > 0f && isPlaying)
        {
            extraEffectCountDownPoints -= Time.deltaTime;

            if (extraEffectCountDownPoints < 0f) pointMultiplier = 1;
        }
    }

    /// <summary>
    /// Manages the score's point multiplier based on effect duration.
    /// </summary>
    /// <param name="extraPointsModifierValue">The new value for the point multiplier.</param>
    private void UpdatePointsModifier(int extraPointsModifierValue)
    {
        pointMultiplier = extraPointsModifierValue;
        if (extraEffectCountDownPoints <= 0)
        {
            extraEffectCountDownPoints = effectDuration;
        }
        else
        {
            extraEffectCountDownPoints += effectDuration;
        }
    }


    /// <summary>
    /// Adds points to the score counter, checks if highscore is reached, and updates text fields for scores.
    /// </summary>
    /// <param name="objectPoints">The number of points attributed to an object.</param>
    public void AddPoints(int objectPoints)
    {
        score = score + pointMultiplier * objectPoints;
        scoreText.text = score.ToString() + " POINTS";

        if (highScore < score)
        {
            highscoreOfPlayer = score;
            highScore = score;
            highScoreText.text = "HIGHSCORE: " + highScore.ToString();
        }
    }

    /// <summary>
    /// Method to change the current GameMode by setting a new GameMode, firing event signaling change of current GameMode.
    /// </summary>
    /// <param name="selectedGameMode">The new GameMode.</param>
    public void SetCurrentGameMode(GameMode selectedGameMode)
    {
        if (selectedGameMode != GameMode.DEFAULT_NONE)
        {
            Debug.Log("Setting current game mode " + currentGameMode + " to selected mode " + selectedGameMode);
            currentGameMode = selectedGameMode;
            onSettingGameMode.Invoke();
        }
    }

    public GameMode GetCurrentGameMode()
    {
        return currentGameMode;
    }

    /// <summary>
    /// Starting the game by setting score to 0, firing event signaling start of game and setting game state to true.
    /// </summary>
    public void StartGame()
    {
        score = 0;
        scoreText.text = score.ToString() + " POINTS";

        onPlay.Invoke();
        isPlaying = true;
    }

    /// <summary>
    /// Ending the game, firing event signaling end of game and setting game state to false.
    /// </summary>
    public void GameOver()
    {
        onGameOver.Invoke();
        isPlaying = false;
    }
}
