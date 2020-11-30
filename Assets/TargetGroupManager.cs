using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TargetGroupManager : MonoBehaviour
{
    public static TargetGroupManager instance;
    private CinemachineTargetGroup cinemachineTargetGroup;
    public List<GameObject> targetobjects;
    private void Awake()
    {
        instance = this;
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    public void AddTarget(GameObject go, float priority,float radius)
    {
        cinemachineTargetGroup.AddMember(go.transform,priority,radius);
    }

    public void RemoveTarget(GameObject go)
    {
        cinemachineTargetGroup.RemoveMember(go.transform);
    }
}
