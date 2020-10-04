using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineEnemyPathController : CinemachinePathController
{
    [SerializeField] private List<LayerMask> excludedMasks;
    public override bool CheckIfPathISClear(Transform target, float distance, Quaternion orientation)
    {
        var colliders = Physics.OverlapBox(target.position + boxCollider.center, boxCollider.size / 2f, orientation);
        if (colliders.Length != 0) 
        {
            foreach (var c in colliders)
            {
                if (excludedMasks.Contains(c.gameObject.layer))
                {
                    Debug.Log(c.gameObject.name);
                    Debug.LogError("not free");
                    return false;
                }
            }
          
        }
        return true;
    }
}
