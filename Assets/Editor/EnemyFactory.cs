using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Mono.Cecil;

// Using Factory Method pattern to create enemies based on their type (Melee or Ranged)
public class EnemyFactory 
{
    public static GameObject GenerateEnemy(GameObject basePrefab,GameObject modelPrefab, EnemySettings settings, bool isMelee, Vector3 position, int skinIndex, bool[] attachmentToggles)
    {

        // STEP 1: Instantiate the base prefab
        GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
        enemy.transform.position = position;
        enemy.name = isMelee ? "EnemyMelee_Generated" : "EnemyRanged_Generated";

        // STEP 2: Set the model prefab
        GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
        model.transform.SetParent(enemy.transform, false); // false = keep local position
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;


        //STEP 3: Set the skin index
        PreviewModelSelector skinSelector = model.GetComponent<PreviewModelSelector>();
        if(skinSelector != null)
        {
            skinSelector.SelectSkin(skinIndex);
        }
        else
        {
            Debug.LogWarning("PreviewModelSelector component not found on model prefab.");
        }

        // STEP 4: Set the attachment toggles
        AttachmentSelector attachmentSelector = model.GetComponent<AttachmentSelector>();
        if (attachmentSelector != null && attachmentToggles != null)
        {
            for (int i = 0; i < attachmentToggles.Length; i++)
            {
                attachmentSelector.SetAttachmentActive(i, attachmentToggles[i]);
            }
        }
        else
        {
            Debug.LogWarning("AttachmentSelector null: " + (attachmentSelector == null) +
                             " | Toggles null: " + (attachmentToggles == null));
        }

        // SELECT WEAPON
        EnemyWeaponSetup weaponSetup = model.GetComponentInChildren<EnemyWeaponSetup>();
        if (weaponSetup != null)
        {
            if (isMelee) weaponSetup.ActivateMelee();
            else weaponSetup.ActivateRanged();
        }

        // Step 5: Add correct enemy script 
        Enemy enemyScript;
        if (isMelee)
        {
            EnemyMelee melee = enemy.AddComponent<EnemyMelee>();
            melee.ApplyMeleeSettings(settings as MeleeEnemySettings);
            enemyScript = melee;
        }
        else
        {
            EnemyRanged ranged = enemy.AddComponent<EnemyRanged>();
            ranged.ApplyRangedSettings(settings as RangedEnemySettings);
            enemyScript = ranged;
        }

        // step 5.1: Add EnemyHealth script and apply health
        EnemyHealth healthScript = enemy.AddComponent<EnemyHealth>();
        healthScript.ApplyHealth(settings.health);

        // step 5.2: Add animator
        Animator animator = enemy.GetComponent<Animator>();

        RuntimeAnimatorController controller; 

        if (isMelee)
        {
            controller = Resources.Load("MeleeController") as RuntimeAnimatorController;
        }
        else 
        {
            controller = Resources.Load("RangedController") as RuntimeAnimatorController;
        }

        animator.runtimeAnimatorController = null;
        animator.runtimeAnimatorController = controller;
        
        if(animator.runtimeAnimatorController != null)
        {
            Debug.Log(animator.runtimeAnimatorController.name);
        }

        //Step 6: Apply the common settings to the enemy script
        enemyScript.ApplyEnemySettingForFactory(settings);

        //Step 7: Remove all inactive attachments from the model to clean up the hierarchy
        CleanupInactiveObjects(model);
        Undo.RegisterCreatedObjectUndo(enemy, isMelee ? "Generate Melee Enemy" : "Generate Ranged Enemy");

        return enemy;
    }

    private static void CleanupInactiveObjects(GameObject obj)
    {
        List<GameObject> inactiveObjects = new List<GameObject>();

        foreach(Transform child in obj.GetComponentsInChildren<Transform>(true))
        {
            if (!child.gameObject.activeSelf) // if the child is inactive, add it to the list for removal
            {
                inactiveObjects.Add(child.gameObject);
                //Debug.Log(child.gameObject);
            }
        }

        foreach (GameObject inactive in inactiveObjects)
        {
            if (inactive != null) Object.DestroyImmediate(inactive); // Destroy the inactive object immediately
        }
    }
}
