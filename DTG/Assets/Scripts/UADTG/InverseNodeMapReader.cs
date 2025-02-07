using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class InverseNodeMapReader : MonoBehaviour
{
    [SerializeField] private Inverse2DQI nodeMap;
    [SerializeField] private Transform source;

    [SerializeField] private bool update = false;
    [SerializeField] private InputActionReference generate;
    
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(nodeMap);
    }

    // Update is called once per frame
    void Update()
    {
        if (generate.action.WasPerformedThisFrame())
        {
            nodeMap.GenerateGrid();
            update = true;
        }
        if (!update) return;
        Vector3 offset = transform.position - source.position;
        offset.y = 0;
        
        transform.position = nodeMap.GetMappedPosition(-offset) + offset;
    }
}
