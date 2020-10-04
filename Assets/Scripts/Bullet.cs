using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Transform visualTransform;

    private Transform hitTransform;
    private bool isEnemyShot;
    private float shootingForce;
    private Vector3 direction;
    private Vector3 hitPoint;

    public void Launch(float shootingForce, Transform hitTransform, Vector3 hitPoint)
    {
        direction = (hitPoint - transform.position).normalized;
        isEnemyShot = false;
        this.hitTransform = hitTransform;
        this.shootingForce = shootingForce;
        this.hitPoint = hitPoint;
    }

    private void Update()
    {
        Move();
        Rotate();
        CheckDistanceToEnemy();
    }

    private void Move()
    {
        transform.Translate(direction * shootingForce * Time.deltaTime, Space.World);
    }

    private void CheckDistanceToEnemy()
    {
        float distance = Vector3.Distance(transform.position, hitPoint);
        if(distance <= 0.1 && !isEnemyShot)
        {
            EnemyController enemy = hitTransform.GetComponentInParent<EnemyController>();
            if (enemy)
            {
                ShootEnemy(hitTransform, enemy);
            }
        }
    }

    private void Rotate()
    {
        visualTransform.Rotate(Vector3.forward, 1200 * Time.deltaTime, Space.Self);
    }

    private void ShootEnemy(Transform hitTransform, EnemyController enemy)
    {
        isEnemyShot = true;
        Rigidbody shotRB = hitTransform.GetComponent<Rigidbody>();
        enemy.OnEnemyShot(transform.forward, shotRB);
    }

    public float GetBulletSpeed()
    {
        return shootingForce;
    }

	internal Transform GetHitEnemyTransform()
	{
        return hitTransform;
	}
}