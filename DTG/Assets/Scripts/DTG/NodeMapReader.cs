using System;
using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class NodeMapReader : MonoBehaviour
{
    [SerializeField] private NodeMap nodeMap;
    [SerializeField] private Transform source;

    [SerializeField] private bool update = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(nodeMap);
    }

    // Update is called once per frame
    void Update()
    {
        if (!update) return;
        Vector3 offset = transform.position - source.position;
        offset.y = 0;
        
        transform.position = nodeMap.GetMappedPosition(source.position) + offset;
        //transform.position = nodeMap.GetMappedPositionIDW(source.position) + offset; //Not the way!
        //transform.position = nodeMap.GetMappedPositionBicubic(source.position) + offset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        List<Vector3> b = new List<Vector3>();
        var s = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRInputSubsystem>();
        if (s != null && s.TryGetBoundaryPoints(b))
        {
            foreach (var point in b)
            {
                Gizmos.DrawSphere(point, 0.3f);
            } 
        }
    }
}
