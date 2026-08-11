using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Stay In Radius")]
public class StayInRadiusBehaviour : FlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // Center and radius live on the Flock: single source of truth
        Vector3 center = flock.transform.position;
        float radius = flock.FlockRadius;

        Vector3 offsetToCenter = center - agent.transform.position;
        float distanceRatio = offsetToCenter.magnitude / radius;   // Distance between the center and the agent => 0 = center, 1 = on the edge

        // Comfortably inside (within 90%): no correction
        if (distanceRatio < 0.9f)
        {
            return Vector3.zero;
        }

        // Near or past the edge: pull back toward center, stronger the snappier is the turn back
        return offsetToCenter * distanceRatio * distanceRatio;
    }
}
