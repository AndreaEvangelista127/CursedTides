using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyMeleePrefab;
    [SerializeField] private GameObject _enemyRangedPrefab;
    [SerializeField] private int _enemyMeleeCount = 2;
    [SerializeField] private int _enemyRangedCount = 2;
    [SerializeField] private float _spawnRadius = 5f;
    [SerializeField] private LayerMask _terrainLayer;
    [SerializeField] private int _maxAttempts = 10;
    [SerializeField] private float _heightSpawnOffset = 20f;

    private List<GameObject> _spawnedEnemies = new List<GameObject>(); // Saving all the enemy that spawned in case we want to eliminate them for specific scenarios

    private void Start()
    {
        SpawnGuards();
    }

    private void SpawnGuards()
    {
        for (int i = 0; i < _enemyMeleeCount; i++)
        {
            if(_enemyMeleePrefab != null) SpawnEnemy(_enemyMeleePrefab);
        }

        for (int i = 0; i < _enemyRangedCount; i++)
        {
            if(_enemyRangedPrefab != null) SpawnEnemy(_enemyRangedPrefab);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if(enemyPrefab == null) return;

        for (int attempt = 0; attempt < _maxAttempts; attempt++)
        {
            Vector3 randomCircle = transform.position + UnityEngine.Random.insideUnitSphere * _spawnRadius; 
            Vector3 spawnOrigin = new Vector3(randomCircle.x, 50f, randomCircle.z); // spawnPosition very high to avoid spawning the enemy inside the terrain
            Ray ray = new Ray(spawnOrigin, Vector3.down);

            if(Physics.Raycast(ray,out RaycastHit hit, Mathf.Infinity, _terrainLayer))
            {
                Vector3 spawnPos = hit.point + Vector3.up * _heightSpawnOffset;
                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                _spawnedEnemies.Add(enemy);
                return; // Spawned, exit the attempt loop
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}
