using System;
using System.Diagnostics;

/// <summary>
/// Class to handle a specific spawning pattern regarding the spawn angle and circle quadrants.
/// </summary>
public class BiasMap
{
    /// <summary>
    /// Method to handle spawn restriction based on quadrant.
    /// </summary>
    /// <param name="angle">The randomly generated rotation angle.</param>
    /// <param name="maxAngleDeviation">The maximum angle deviation allowed.</param>
    /// <param name="quadrantRestriction">The name of the banned quadrant.</param>
    /// <returns>0 for banned quadrant.</returns>
    public static float BiasMapFunction(float angle, float maxAngleDeviation, string quadrantRestriction)
    {
        // Exclude a quadrant based on the object name
        if (quadrantRestriction == "Third")  // Exclude third quadrant (Q3) for RedObject
        {
            if (Math.PI <= angle && angle < 3 * Math.PI / 2)
            {
                return 0;  // Exclude Q3 
            }
        }
        else if (quadrantRestriction == "Fourth")  // Exclude fourth quadrant (Q4)
        {
            if (3 * Math.PI / 2 <= angle && angle < 2 * Math.PI)
            {
                return 0;  // Exclude Q4
            }
        }

        // Return the maxAngleDeviation as the radius, with stronger bias near the edges
        return maxAngleDeviation * maxAngleDeviation;  // Bias stronger near the edges
    }

    /// <summary>
    /// Method to generate a spawn values based on the biased angle and exclusion rules.
    /// </summary>
    /// <param name="quadrantRestriction">The name of the banned quadrant.</param>
    /// <param name="maxAngleDeviation">The maximum angle deviation allowed.</param>
    /// <returns>(float, float) x and y values.</returns>
    public static (float, float) GenerateSpawnPosition(string quadrantRestriction, float maxAngleDeviation)
    {
        // Generate a random angle between 0 and 2π (full circle)
        Random random = new Random();
        float angle = (float)(2 * Math.PI * random.NextDouble());

        // Apply the Bias Map Function to calculate the biased spawn value
        float bias = BiasMapFunction(angle, maxAngleDeviation, quadrantRestriction);

        if (bias == 0)
        {
            UnityEngine.Debug.Log("QUADRANT scored which is not allowed");
            // Retry until we get a valid spawn position (not in excluded quadrant)
            return GenerateSpawnPosition(quadrantRestriction, maxAngleDeviation);  // Recursion until valid spawn
        }

        // Calculate spawn rotation values with allowed quadrants value (based on angle)
        float spawnX = maxAngleDeviation * (float)Math.Cos(angle);
        float spawnY = maxAngleDeviation * (float)Math.Sin(angle);

        // Return the final spawn rotations for x and y
        return (spawnX, spawnY);
    }
}