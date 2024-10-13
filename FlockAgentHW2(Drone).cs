using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlockAgent : MonoBehaviour
{
    Flock agentFlock;
    public Flock AgentFlock { get { return agentFlock; } }

    Collider2D agentCollider;
    public Collider2D AgentCollider { get { return agentCollider; } }

    // New attributes for search and rescue
    public float DistanceToVictim { get; set; }
    public float BatteryLife { get; set; } = 100f; // default battery percentage
    public int VictimsFound { get; set; } = 0;
    public bool HasFoundVictim { get; set; } = false; // Flag to check if the drone found a victim

    void Start()
    {
        agentCollider = GetComponent<Collider2D>();
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }

    public void Move(Vector2 velocity)
    {
        // Only move if the drone hasn't found a victim
        if (!HasFoundVictim)
        {
            transform.up = velocity;
            transform.position += (Vector3)velocity * Time.deltaTime;
        }
    }
}