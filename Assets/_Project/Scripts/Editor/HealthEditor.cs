using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    static float damage = 5;
    static float maxHealth = 50;
    static float heal = 5;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Health health = (Health)target;

        EditorGUILayout.LabelField("QA Testing");
        damage = EditorGUILayout.FloatField("Damage", damage);

        if (GUILayout.Button("Take " + damage + " damage"))
        {
            health.TakeDamage(damage);
        }
        GUILayout.Space(10);
        
        heal = EditorGUILayout.FloatField("Heal", heal);

        if (GUILayout.Button("Heal " + heal))
        {
            health.Heal(heal);
        }
        GUILayout.Space(10);
        
        maxHealth = EditorGUILayout.FloatField("New max health", maxHealth);

        if (GUILayout.Button("Change max health to " + maxHealth))
        {
            health.ChangeMaxHealth(maxHealth, health.CurrentHealth, true);
        }
    }
}