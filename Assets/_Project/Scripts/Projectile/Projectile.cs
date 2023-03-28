using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPool;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10;
    public float _areaDamageRange = 5;
    public float _freezeTime = 2;
    [SerializeField] private bool _freeze;
    [SerializeField] private bool _areaDamage;
    private ObjectPool<Projectile> _ownerPool;
    private Vector3 _targetDirection;
    private float _damage;


    private void FixedUpdate()
    {
        transform.position += (_targetDirection * _speed * Time.deltaTime);
        transform.forward = _targetDirection;
    }

    public void Setup(Vector3 targetPosition, float damage, ObjectPool<Projectile> ownerPool)
    {
        _targetDirection = (targetPosition - transform.position).normalized;
        _damage = damage;
        _ownerPool = ownerPool;
    }

    private void KillProjectile()
    {
        if (_ownerPool != null)
        {
            _ownerPool.DeactivateObject(this);
        }
        else
        {
            Debug.LogWarning("Pool is not referenced, destroying projectile");
            Destroy(gameObject);
        }
    }

    private void OnHit()
    {
        //TODO Play sound fx, instantiate impact vfx
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Avoid triggers
        if (collision.collider.isTrigger) return;

        if (_areaDamage)
        {
            Collider[] hitColliders = Physics.OverlapSphere(collision.GetContact(0).point, _areaDamageRange);
            foreach (var col in hitColliders)
            {
                DealDamage(col.gameObject);
            }
        }
        else if (_freeze)
        {
            NavMeshMovement movement = collision.collider.GetComponent<NavMeshMovement>();
            if (movement)
                movement.StartEnemyRecoil(_freezeTime);
            DealDamage(collision.gameObject);
        }
        else
        {
            DealDamage(collision.gameObject);
        }

        OnHit();
        KillProjectile();
    }

    void DealDamage(GameObject gameObj)
    {
        Health health = gameObj.GetComponentInParent<Health>();
        if (health)
        {
            health.TakeDamage(_damage);
        }
    }
}