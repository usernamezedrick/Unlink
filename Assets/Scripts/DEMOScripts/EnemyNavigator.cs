using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    public List<Transform> waypoints; // List of waypoints for the enemy to navigate between
    private NavMeshAgent agent;       // NavMeshAgent component reference
    private int currentWaypointIndex = 0; // Track the current waypoint index
    public float pathUpdateInterval = 0.1f; // Time interval for path recalculation
    public float obstacleAvoidanceDistance = 2.0f; // Distance for obstacle avoidance
    public float slowDownDistance = 5.0f; // Distance at which to slow down before reaching a waypoint
    public float edgeAvoidanceDistance = 3.0f; // Distance to avoid edges of unbaked areas

    private float lastPathUpdateTime = 0f; // Track the last path update time

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        // Ensure there are waypoints
        if (waypoints.Count == 0)
        {
            Debug.LogError("No waypoints set for the enemy.");
            return;
        }

        // Set the initial destination to the first waypoint
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void Update()
    {
        // Avoid obstacles and edges of unbaked areas
        AvoidObstacles();

        // Recalculate the path periodically
        if (Time.time - lastPathUpdateTime > pathUpdateInterval)
        {
            UpdatePath();
            lastPathUpdateTime = Time.time;
        }

        // Adjust speed based on proximity to the current waypoint
        AdjustSpeed();

        // Check if the agent has reached its current waypoint
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void UpdatePath()
    {
        // Dynamically update the path to the next waypoint
        if (agent.isOnNavMesh)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;

            // Avoid edges of unbaked areas
            if (IsNearUnbakedArea(targetPosition))
            {
                // Find a new position away from the edge
                Vector3 newTarget = GetSafeTargetPosition(targetPosition);
                agent.SetDestination(newTarget);
            }
            else
            {
                agent.SetDestination(targetPosition);
            }
        }
        else
        {
            Debug.LogWarning("Agent is not on the NavMesh!");
        }
    }

    bool IsNearUnbakedArea(Vector3 position)
    {
        // Check if the position is near an unbaked area using a raycast
        RaycastHit hit;
        return Physics.Raycast(position, Vector3.down, out hit, edgeAvoidanceDistance) &&
               !NavMesh.SamplePosition(hit.point, out _, 0.1f, NavMesh.AllAreas);
    }

    Vector3 GetSafeTargetPosition(Vector3 originalTarget)
    {
        // Attempt to find a new target position away from the unbaked area
        Vector3 safeTarget = originalTarget;

        // Check in a few directions to find a valid position
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f; // Check 8 directions
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward * edgeAvoidanceDistance;

            if (NavMesh.SamplePosition(originalTarget + direction, out NavMeshHit navHit, 0.1f, NavMesh.AllAreas))
            {
                safeTarget = navHit.position; // Update to a valid position
                break;
            }
        }

        return safeTarget;
    }

    void AvoidObstacles()
    {
        // Check for obstacles in front of the agent
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceDistance))
        {
            // Calculate a new direction away from the obstacle
            Vector3 avoidanceDirection = (transform.position - hit.point).normalized;
            avoidanceDirection.y = 0; // Ignore vertical direction

            // Calculate a new destination by steering away from the obstacle
            Vector3 newDestination = transform.position + avoidanceDirection * 2.0f; // Adjust strength if needed

            NavMeshHit navHit;
            // Sample the NavMesh to find a valid point for the new destination
            if (NavMesh.SamplePosition(newDestination, out navHit, 0.1f, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
            }
        }
    }

    void AdjustSpeed()
    {
        // Calculate the distance to the current waypoint
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);

        // If close to the waypoint, slow down
        if (distanceToWaypoint < slowDownDistance)
        {
            agent.speed = Mathf.Lerp(agent.speed, 2.0f, Time.deltaTime); // Slow speed (adjust as needed)
        }
        else
        {
            agent.speed = 4.0f; // Normal speed (adjust as needed)
        }
    }

    void OnDrawGizmos()
    {
        // Visualize waypoints and paths in the editor
        if (waypoints != null && waypoints.Count > 1)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);

                if (i < waypoints.Count - 1)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
                else
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[0].position); // Close the loop
                }
            }
        }
    }
}
