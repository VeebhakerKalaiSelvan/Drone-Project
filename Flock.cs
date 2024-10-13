using System.Collections;
using System.Collections.Generic;
using System.Diagnostics; // For the Stopwatch
using UnityEngine;
using UnityEngine.UI; // For the UI Text component

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehavior behavior;

    [Range(10, 500)]
    public int startingCount = 250;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    // Victim locations and status
    List<Vector3> victimLocations = new List<Vector3>();
    List<bool> victimFoundStatus = new List<bool>(); // Keeps track of whether a victim location is occupied

    private Stopwatch stopwatch; // For measuring the execution time of the function
    private long totalExecutionTime = 0; // Sum of all execution times
    private int frameCount = 0; // Number of frames over which we're measuring

    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        stopwatch = new Stopwatch(); // Initialize the stopwatch
        GenerateVictimLocations();

        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                Random.insideUnitCircle * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
            );
            newAgent.name = "Agent " + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }
    }

    void GenerateVictimLocations()
    {
        int numberOfVictims = Mathf.CeilToInt(startingCount * 0.1f); // Set number of victims to 10% of the number of drones
        victimLocations.Clear();
        victimFoundStatus.Clear();

        for (int i = 0; i < numberOfVictims; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-50f, 50f), // X position within scene bounds
                Random.Range(-50f, 50f), // Y position within scene bounds
                0f
            );
            victimLocations.Add(randomPosition);
            victimFoundStatus.Add(false); // Initially, no victim location is occupied
        }
    }
    
    void Update()
    {
        stopwatch.Reset();
        stopwatch.Start();
        
        PartitionDronesBasedOnVictims();
        stopwatch.Stop();
        // Accumulate the total execution time and increase frame count
        totalExecutionTime += stopwatch.ElapsedMilliseconds;
        frameCount++;

        // Calculate and display the average execution time in the console every 60 frames
        if (frameCount % 60 == 0) // Display every 60 frames (1 second if running at 60 FPS)
        {
            float averageTime = (float)totalExecutionTime / frameCount;
            UnityEngine.Debug.Log($"Average Partition Function Time: {averageTime:F2} ms over {frameCount} frames");
        }


        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);
            Vector2 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }

        
    }

    private void PartitionDronesBasedOnVictims()
    {
    foreach (FlockAgent agent in agents)
    {
        // Only consider drones that have not already found a victim
        if (!agent.HasFoundVictim)
        {
            bool foundVictim = false;
            
            // Iterate through each victim location to find an available victim
            for (int i = 0; i < victimLocations.Count; i++)
            {
                // Check if the victim has not been found by another drone
                if (!victimFoundStatus[i] && Vector3.Distance(agent.transform.position, victimLocations[i]) < neighborRadius)
                {
                    // Mark the victim as found and stop the drone at the victim's location
                    agent.VictimsFound += 1;
                    agent.HasFoundVictim = true;
                    agent.GetComponentInChildren<SpriteRenderer>().color = Color.red; // Change drone color to indicate it found a victim
                    agent.transform.position = victimLocations[i]; // Ensure the drone stops at the victim's location

                    victimFoundStatus[i] = true; // Mark this victim location as occupied
                    foundVictim = true;
                    break; // Exit the loop once a victim is found
                }
            }

            // If no victim is found, keep the drone moving normally
            if (!foundVictim)
            {
                agent.GetComponentInChildren<SpriteRenderer>().color = Color.white; // Normal drone color
            }
        }
    }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }
}