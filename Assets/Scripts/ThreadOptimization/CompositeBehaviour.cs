using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
public class CompositeBehaviour : FlockBehaviour
{
    [SerializeField] private FlockBehaviour[] _behaviours;
    [SerializeField] private float[] _weights;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        // Each behaviour must have exactly one matching weight
        if (_weights.Length != _behaviours.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        Vector3 move = Vector3.zero;

        for (int i = 0; i < _behaviours.Length; i++)
        {
            Vector3 partialMove = _behaviours[i].CalculateMove(agent, context, flock) * _weights[i];

            if (partialMove != Vector3.zero)
            {
                // Clamp each behaviour so no single one can dominate the result
                if (partialMove.sqrMagnitude > _weights[i] * _weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= _weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }
}
