using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Alignment")]
public class AlignmentBehaviour : FlockBehaviour
{
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // No neighbors: keep the current heading
        if (context.Count == 0)
        {
            return agent.transform.forward;
        }

        // Average forward direction of the neighbors
        Vector3 alignmentMove = Vector3.zero;
        foreach (Transform item in context)
        {
            alignmentMove += item.forward;
        }
        alignmentMove /= context.Count;

        return alignmentMove;
    }
}
