using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Cohesion")]
public class CohesionBehaviour : FlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // No neighbors: no cohesion contribution
        if (context.Count == 0)
        {
            return Vector3.zero;
        }

        // Average position of the neighbors (center of mass)
        Vector3 cohesionMove = Vector3.zero;
        foreach (Transform item in context)
        {
            cohesionMove += item.position;
        }
        cohesionMove /= context.Count;

        // Turn the absolute target into a direction relative to the agent
        cohesionMove -= agent.transform.position;
        return cohesionMove;
    }
}
