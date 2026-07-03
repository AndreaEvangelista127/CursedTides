using UnityEditor;
using UnityEngine;

// Using Factory Method pattern to create enemies based on their type (Melee or Ranged)
public class EnemyFactory 
{
    public static GameObject SpawnEnemy(GameObject prefab, EnemySettings settings, Vector3 position, bool isMelee)
    {
        if (prefab == null)
        {
            Debug.LogError("EnemyFactory: Prefab is not assigned!");
            return null;
        }

        /* 
         * This is similar to Instantiate but creates a Prefab connection to the Prefab, advantage is that 
         * we can modify the Prefab and all instances will be updated
         * with just Instatiate the object will be a normal Game Object disconnected from the Prefab
        */
        GameObject enemy = GameObject.Instantiate(prefab);
        //GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        enemy.transform.position = position;
        enemy.name = isMelee ? "EnemyMelee_Spawned" : "EnemyRanged_Spawned";

        // WEAPON ASSIGNMENT BASED ON ENEMY TYPE
        EnemyWeaponSetup weaponSetup = enemy.GetComponent<EnemyWeaponSetup>();
        if (weaponSetup != null)
        {
            if (isMelee) weaponSetup.ActivateMelee();
            else weaponSetup.ActivateRanged();
        }
        else
        {
            Debug.LogWarning("EnemyFactory: EnemyWeaponSetup not found on prefab!");
        }

        // APPLY COMMONG SETTINGS TO BOTH ENEMYS
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
            enemyComponent.ApplyEnemySettingForFactory(settings);

        // PASSING VIA PARAMETER THE SPECIFIC TYPE OF SETTINGS SO THAT THE "is" CASTING CAN CHECK IF THEY ARE MELEE/RANGED SETTINGS
        if (isMelee && settings is MeleeEnemySettings meleeSettings)
        {
            EnemyMelee meleeEnemy = enemy.GetComponent<EnemyMelee>();
            if (meleeEnemy != null)
                meleeEnemy.ApplyMeleeSettings(meleeSettings);
        }
        else if (!isMelee && settings is RangedEnemySettings rangedSettings)
        {
            EnemyRanged rangedEnemy = enemy.GetComponent<EnemyRanged>();
            if (rangedEnemy != null)
                rangedEnemy.ApplyRangedSettings(rangedSettings);
        }

        // Register undo so Ctrl+Z removes the spawned enemy
        Undo.RegisterCreatedObjectUndo(enemy, isMelee ? "Spawn Melee Enemy" : "Spawn Ranged Enemy");

        return enemy;
    }


    //// Using static because we don't need to instantiate the factory, we just need to call the method to create enemies
    //public static GameObject CreateMeleeEnemy(MeleeEnemySettings settings, Vector3 position)
    //{
    //    if(settings.prefab == null)
    //    {
    //        Debug.LogError("Melee Enemy prefab is not assigned in the settings.");
    //        return null;
    //    }

    //    /* 
    //     * This is similar to Instantiate but creates a Prefab connection to the Prefab, advantage is that 
    //     * we can modify the Prefab and all instances will be updated
    //     * with just Instatiate the object will be a normal Game Object disconnected from the Prefab
    //    */
    //    GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(settings.prefab);
    //    enemy.transform.position = position;
    //    enemy.name = "EnemyMelee_Spawned";
    //    enemy.GetComponent<Enemy>().ApplyEnemySettingForFactory(settings);
    //    /*
    //    Register the creation of the enemy for undo functionality in the Unity Editor because creating an onbject 
    //    in the editor is not undoable by default, so we need to register it manually
    //    */
    //    Undo.RegisterCreatedObjectUndo(enemy, "Spawn Melee Enemy");

    //    return enemy;
    //}

    //public static GameObject CreateRangedEnemy(RangedEnemySettings settings, Vector3 position)
    //{
    //    if (settings.prefab == null)
    //    {
    //        Debug.LogError("EnemyFactory: Ranged prefab is not assigned!");
    //        return null;
    //    }

    //    GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(settings.prefab);
    //    enemy.transform.position = position;
    //    enemy.name = "EnemyRanged_Spawned";

    //    enemy.GetComponent<Enemy>().ApplyEnemySettingForFactory(settings);

    //    Undo.RegisterCreatedObjectUndo(enemy, "Spawn Ranged Enemy");

    //    return enemy;
    //}

}
