using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using Utils;

public class TowerDamage : MonoBehaviour
{
    private Health _health;
    [SerializeField] private LayerMask _damageLayer;

    private void Awake()
    {
        _health = GetComponentInParent<Health>();
    }

    void OnTriggerEnter(Collider col)
    {
        //Avoid triggers
        if (col.isTrigger) return;

        if (!Util.IsInLayer(col.gameObject, _damageLayer)) return;

        Enemy enemy = col.GetComponentInParent<Enemy>();
        if (enemy)
        {
            _health.OnTakeDamage.Invoke(enemy.Damage);
            enemy.Health.OnDie.Invoke(enemy.gameObject);
        }
    }
}