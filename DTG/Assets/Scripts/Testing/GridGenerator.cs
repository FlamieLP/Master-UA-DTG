using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Vector2 sourceDimensions = Vector2.one;
    [SerializeField] private int nodesPerAxis = 4;
    
    [SerializeField] private Transform[,] _sourceNodes, _targetNodes;
    [SerializeField] private GameObject sourceRoot, targetRoot;

    public Vector3[,] GetSourceGrid()
    {
        Vector3[,] grid = new Vector3[nodesPerAxis, nodesPerAxis];
        for (var y = 0; y < nodesPerAxis; y++)
        {
            for (var x = 0; x < nodesPerAxis; x++)
            {
                grid[y, x] = _sourceNodes[y, x].position;
            }
        }

        return grid;
    }
    
    public Vector3[,] GetTargetGrid()
    {
        Vector3[,] grid = new Vector3[nodesPerAxis, nodesPerAxis];
        for (var y = 0; y < nodesPerAxis; y++)
        {
            for (var x = 0; x < nodesPerAxis; x++)
            {
                grid[y, x] = _targetNodes[y, x].position;
            }
        }

        return grid;
    }

    [ContextMenu("Create new Node Grit")]
    private void CreateNodes()
    {
        Setup();
        _sourceNodes = CreateNodeGrit(sourceRoot.transform);
        _targetNodes = CreateNodeGrit(targetRoot.transform);
    }

    private Transform[,] CreateNodeGrit(Transform parent)
    {
        Transform[,] nodeList = new Transform[nodesPerAxis, nodesPerAxis];
        Vector2 adjustForDimension = sourceDimensions / (nodesPerAxis - 1);
        
        for (var y = 0; y < nodesPerAxis; y++)
        {
            for (var x = 0; x < nodesPerAxis; x++)
            {
                var index = (y * nodesPerAxis) + x;
                var node = Instantiate(nodePrefab, parent);
                node.name = $"Node: {x} | {y}";
                node.transform.localPosition = new Vector3(
                    x * adjustForDimension.x,
                    0,
                    y * adjustForDimension.y
                );
                nodeList[y,x] = node.transform;
            }
        }

        return nodeList;
    }
    
    private void Setup()
    {
        if (!sourceRoot)
        {
            sourceRoot = new GameObject("Source Root")
            {
                transform =
                {
                    parent = transform
                }
            };
        }
        if (!targetRoot)
        {
            targetRoot = new GameObject("Target Root")
            {
                transform =
                {
                    parent = transform
                }
            };
        }
    }
}
