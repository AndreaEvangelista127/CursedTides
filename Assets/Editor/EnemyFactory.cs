using UnityEditor;
using UnityEngine;

// Using Factory Method pattern to create enemies based on their type (Melee or Ranged)
public class EnemyFactory 
{

    // Using static because we don't need to instantiate the factory, we just need to call the method to create enemies
    public static GameObject CreateMeleeEnemy(MeleeEnemySettings settings, Vector3 position)
    {
        if(settings.prefab == null)
        {
            Debug.LogError("Melee Enemy prefab is not assigned in the settings.");
            return null;
        }

        /* 
         * This is similar to Instantiate but creates a Prefab connection to the Prefab, advantage is that 
         * we can modify the Prefab and all instances will be updated
         * with just Instatiate the object will be a normal Game Object disconnected from the Prefab
        */
        GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(settings.prefab);
        enemy.transform.position = position;
        enemy.name = "EnemyMelee_Spawned";
        enemy.GetComponent<Enemy>().ApplyEnemySettingForFactory(settings);
        /*
        Register the creation of the enemy for undo functionality in the Unity Editor because creating an onbject 
        in the editor is not undoable by default, so we need to register it manually
        */
        Undo.RegisterCreatedObjectUndo(enemy, "Spawn Melee Enemy");

        return enemy;
    }

    public static GameObject CreateRangedEnemy(RangedEnemySettings settings, Vector3 position)
    {
        if (settings.prefab == null)
        {
            Debug.LogError("EnemyFactory: Ranged prefab is not assigned!");
            return null;
        }

        GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(settings.prefab);
        enemy.transform.position = position;
        enemy.name = "EnemyRanged_Spawned";

        enemy.GetComponent<Enemy>().ApplyEnemySettingForFactory(settings);

        Undo.RegisterCreatedObjectUndo(enemy, "Spawn Ranged Enemy");

        return enemy;
    }

}
