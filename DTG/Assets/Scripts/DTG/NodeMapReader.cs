using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEngine;
using UnityEngine.Assertions;

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
        
        //transform.position = nodeMap.GetMappedPosition(source.position) + offset;
        //transform.position = nodeMap.GetMappedPositionIDW(source.position) + offset; //Not the way!
        transform.position = nodeMap.GetMappedPositionBicubic(source.position) + offset;
    }
}
