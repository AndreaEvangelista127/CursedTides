using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent AgentPrefab;
    private List<FlockAgent> _agents = new List<FlockAgent>();
    public FlockBehaviour Behaviour;

    [Range(10, 500)]
    public int startingCount = 250; // Amount of agents in the scene
    //public const float AgentDensity = 0.08f;
    public const float AgentDensity = 0.20f;

    [Range(1f, 100f)]
    public float driveFactor = 10f; // Factor that will multiply all the different behaviour so that they´re not slow
    [Range(1f, 100f)]
    public float maxSpeed = 5f; // Speed of every agent
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f; // Radius used to look for neighbors
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    [Header("Boundary")]
    [Range(1f, 100f)]
    [SerializeField] private float _flockRadius = 20f;   // Agents are steered back inside this radius
    public float FlockRadius { get { return _flockRadius; } }

    [SerializeField] private bool _useGizmos = false;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    private void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                AgentPrefab, 
                Random.insideUnitSphere * startingCount * AgentDensity,
                Random.rotation, 
                transform);
            newAgent.name = "Agent " + i;
            _agents.Add(newAgent); 
        }
    }

    private void Update()
    {
        foreach (FlockAgent agent in _agents)
        {
            List<Transform> context = GetNearbyObjects(agent); // List of all neighbors of the specific agent at that frame

            Vector3 move = Behaviour.CalculateMove(agent, context, this); // Calculate how the agent should move based of his neighbors
            move *= driveFactor; // Increase or decrement the intensity of the movement based on the drive factor

            // Clamp to max speed using squared magnitude (avoids a sqrt)
            if (move.sqrMagnitude > squareMaxSpeed) // Check if the current movement speed isn´t higher of the max allowed
            {
                move = move.normalized * maxSpeed; // Keep the same direction but increase or decrease the move vector so that the speed is maxSpeed
            }

            agent.Move(move);
        }
    }


    private List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);

        foreach (Collider collider in contextColliders)
        {
            if (collider != agent.AgentCollider)
            {
                context.Add(collider.transform);
            }
        }

        return context;
    }

    private void OnDrawGizmos()
    {
        if (_useGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _flockRadius);   
        }
    }
}
