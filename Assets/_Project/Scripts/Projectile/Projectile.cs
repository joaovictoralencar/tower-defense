using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPool;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10;

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
        Debug.Log(collision.gameObject.name);

        //Avoid triggers
        if (collision.collider.isTrigger) return;
        
        Health health = collision.gameObject.GetComponentInParent<Health>();
        if (health)
        {
            health.OnTakeDamage.Invoke(_damage);
        }

        OnHit();
        KillProjectile();
    }
}