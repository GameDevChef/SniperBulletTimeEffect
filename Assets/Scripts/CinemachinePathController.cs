using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CinemachineSmoothPath))]
public abstract class CinemachinePathController : MonoBehaviour
{
    public CinemachineSmoothPath path;
    public BoxCollider boxCollider;

    public abstract bool CheckIfPathISClear(Transform target, float distance, Quaternion orientation);

}
