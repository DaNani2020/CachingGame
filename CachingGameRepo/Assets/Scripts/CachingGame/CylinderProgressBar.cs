using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// CylinderProgressBar is the visual representation of the scores to go based on the next threshold (maxPoints).
/// </summary>
public class CylinderProgressBar : MonoBehaviour
{
    public GameObject outerBar;
    private int currentPoints = 0;
    private int maxPoints = 5;
    private int startMaxPoints = 5;
    private int offsetOfPoints = 0;
    private float heightOfBar;
    private float currentFillHeight = 0;
    private float currentYScaleOfBar = 0;
    private float yScaleOfBarAbsolut = 1f;
    private float offsetOfInnerBar = 0f;

    [Header("Glow Settings")]
    public Renderer innerBarRenderer;
    public Color baseEmissionColor = new Color(1f, 0.878f, 0.337f);
    public float glowSpeed = 3f;
    public float glowMin = 1f;
    public float glowMax = 5f;


    private bool maxPointsReached = false;
    private float maxPointsReachedTime = 0f;
    public float delayDuration = 0.3f;  // n second delay

    public UnityEvent<int> newLevelReached;
    private int currentLevel = 1;


    /// <summary>
    /// Start method adds listener to onPlay and onSettingGameMode event of overall game manager to reset values.
    /// It makes preparations by sizing positioning the innerBar based on the outerBar, providing a 3D progress bar.
    /// </summary>
    void Start()
    {
        ScoreManager.instance.onPlay.AddListener(ResetValues);
        ScoreManager.instance.onSettingGameMode.AddListener(ResetValues);

        yScaleOfBarAbsolut = outerBar.gameObject.transform.localScale.y * 0.99f;
        heightOfBar = outerBar.GetComponent<Renderer>().bounds.size.y * 0.99f;
        offsetOfInnerBar = outerBar.GetComponent<Renderer>().bounds.size.y * 0.005f;
        Debug.Log($"ANIMATION POINTS in update loop with maxPoints {maxPoints}, height of bar {heightOfBar} and y scale maximum {yScaleOfBarAbsolut}.");

        innerBarRenderer = GetComponent<Renderer>();
        if (innerBarRenderer == null)
        {
            Debug.LogError("No Renderer component found on this GameObject.");
        }

        UpdateVisuals(currentFillHeight);
    }

    /// <summary>
    /// Updates during the game the point visualisation based on the current maxPoints in the progress bar.
    /// If threshold of maxPoints is reached a short glow animation plays.
    /// If maxPoints is reached, the filling of the progress bar is reset to 0, by setting a new OffsetOfPoints value.
    /// </summary>
    void Update()
    {
        if (!ScoreManager.instance.isPlaying) return;
        if (maxPointsReached)
        {
            UpdateMagicalGlow();
            if (Time.time - maxPointsReachedTime >= delayDuration)
            {
                maxPointsReached = false;
                UpdateVisuals(currentFillHeight);
                Debug.Log($"ANIMATION POINTS Glow ends {maxPointsReachedTime} at time {Time.time} with delay Duration {delayDuration} <= time-maxpointsreached ");
            }
        }

        if (currentPoints != ScoreManager.instance.score && !maxPointsReached)
        {
            int currentScore = ScoreManager.instance.score;
            currentPoints = currentScore;
            Debug.Log($"ANIMATION POINTS in update loop with current points {currentPoints} and ScoreManager {ScoreManager.instance.score}");

            if (maxPoints == 0)
            {
                currentFillHeight = 0;
                Debug.LogError($"ANIMATION POINTS in update loop adjust maxPoints {maxPoints} and current fill height {currentFillHeight}.");
            }
            else if (currentPoints >= maxPoints)
            {
                int oldMaxPoints = maxPoints;
                Debug.Log($"ANIMATION POINTS hit max with current points {currentPoints} and current fill height {currentFillHeight} with max points {maxPoints}.");
                maxPoints = UpdateMaxPointsByFibonacciSequence(maxPoints);
                offsetOfPoints = oldMaxPoints;
                currentFillHeight = heightOfBar;
                maxPointsReached = true;
                currentLevel++;
                newLevelReached.Invoke(currentLevel);
                maxPointsReachedTime = Time.time;

                Debug.Log($"ANIMATION POINTS Glow starts {maxPointsReachedTime}");
            }
            else
            {
                float multiplier = 1;
                if (offsetOfPoints > 0) multiplier = 2;
                currentFillHeight = ((float)(currentPoints - offsetOfPoints) / maxPoints) * heightOfBar * multiplier;
                Debug.Log($"ANIMATION POINTS in update loop with maxPoints {maxPoints}, current points {currentPoints}, height of bar {heightOfBar} and current fill height {currentFillHeight}.");
            }
            Debug.Log($"ANIMATION POINTS glow with current points {currentPoints} and current max Points {maxPoints}.");
            UpdateVisuals(currentFillHeight);
        }
    }

    /// <summary>
    /// UpdateVisuals updates the filling of the progress bar.
    /// </summary>
    /// <param name="fillHeight">The fillHeight represents the scores collected.</param>
    private void UpdateVisuals(float fillHeight)
    {
        currentYScaleOfBar = fillHeight / yScaleOfBarAbsolut;
        transform.localScale = new Vector3(transform.localScale.x, currentYScaleOfBar, transform.localScale.z);

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            fillHeight / 2f - heightOfBar / 2f + offsetOfInnerBar,
            transform.localPosition.z
        );
    }

    /// <summary>
    /// UpdateMagicalGlow is a visual brightening and dimming of the innerBar.
    /// </summary>
    private void UpdateMagicalGlow()
    {
        // Calculate a smooth pulsing effect using PingPong
        float intensity = Mathf.Lerp(glowMin, glowMax, Mathf.PingPong(Time.time * glowSpeed, 1f));
        Color emissionColor = baseEmissionColor * intensity;

        if (innerBarRenderer != null && innerBarRenderer.material.HasProperty("_EmissionColor"))
        {
            innerBarRenderer.material.SetColor("_EmissionColor", emissionColor);
        }
        Debug.Log($"ANIMATION POINTS glow activated");
    }

    /// <summary>
    /// UpdateMaxPointsByFibonacciSequence sets a new maximum of points (threshold to reach).
    /// Based on the Fibonacci Sequence the threshold rises each time, based on the currentMaximumPoints.
    /// </summary>
    /// <param name="currentMaximumPoints"></param>
    /// <returns></returns>
    private int UpdateMaxPointsByFibonacciSequence(int currentMaximumPoints)
    {
        if (offsetOfPoints == 0) return currentMaximumPoints * 2;
        int a = offsetOfPoints;  // First number in the sequence
        int b = currentMaximumPoints;  // Second number in the sequence

        while (b <= currentMaximumPoints)
        {
            int next = a + b;
            a = b;
            b = next;
        }

        return b;
    }


    /// <summary>
    /// ResetValues to the starting values and update the visual representation.
    /// </summary>
    private void ResetValues()
    {
        currentPoints = 0;
        currentLevel = 1;
        maxPoints = startMaxPoints;
        offsetOfPoints = 0;
        currentFillHeight = 0;
        currentYScaleOfBar = 0;
        maxPointsReached = false;
        maxPointsReachedTime = 0f;
        UpdateVisuals(currentFillHeight);
    }
}
