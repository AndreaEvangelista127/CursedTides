using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
public class AvoidanceBehaviour : FlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> neighbors, Flock flock)
    {
        // No neighbors keep going that way
        if (neighbors.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 avoidanceMove = Vector3.zero;
        int neighborsToAvoid = 0;

        foreach (Transform item in neighbors)
        {
            // Only react to neighbors inside the (squared) avoidance radius
            if (Vector3.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                neighborsToAvoid++;
                // Push away: direction pointing from the neighbor to the agent, so a Vector that points to the opposite direction between agent and neighbor
                avoidanceMove += agent.transform.position - item.position;
            }
        }

        // Average of the push away directions
        if (neighborsToAvoid > 0)
        {
            avoidanceMove /= neighborsToAvoid;
        }

        return avoidanceMove;
    }
}
