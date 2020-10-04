using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAnimationController))]
[RequireComponent(typeof(RagdollController))]
public class EnemyController : MonoBehaviour
{
    private EnemyAnimationController animationController;
    private RagdollController ragdollController;

    private void Awake()
    {
        animationController = GetComponent<EnemyAnimationController>();
        ragdollController = GetComponent<RagdollController>();
    }

    public void OnEnemyShot(Vector3 shootDirection, Rigidbody shotRB)
    {
        StopAnimation();
        ragdollController.EnableRagdoll();
        if (shotRB)
        {
            shotRB.AddForce(shootDirection.normalized * 100f, ForceMode.Impulse);
        }            
    }

    public void StopAnimation()
    {
        animationController.DisableAnimator();
    }
}
