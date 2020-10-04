using Cinemachine;
using Cinemachine.PostFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(CinemachineDollyCart))]
public class CameraDollyController : MonoBehaviour
{
    [SerializeField] private bool followTarget;
    [SerializeField] private bool followTargetWithDOF;
    private CinemachineDollyCart cart;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachinePostProcessing cinemachinePostProcessing;

    private void Awake()
    {
        cart = GetComponent<CinemachineDollyCart>();
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachinePostProcessing = GetComponentInChildren<CinemachinePostProcessing>();
    }

    public void InitDolly(CinemachineSmoothPath dollyTrack, Transform target, float speed = 0f)
    {
        if(speed != 0f)
            cart.m_Speed = speed;
        cart.m_Path = dollyTrack;
        if(followTarget)
            virtualCamera.m_LookAt = target;
        if(cinemachinePostProcessing != null)
        {
            DepthOfField depth;
            cinemachinePostProcessing.m_Profile.TryGetSettings<DepthOfField>(out depth);
            if(depth != null)
            {
                if (followTargetWithDOF)
                {
                    depth.active = true;
                    cinemachinePostProcessing.m_FocusTarget = target;
                }
                else
                {
                    depth.active = false;
                }
            }
            else
            {
                depth.active = false;
            }
        }           
    }
}
