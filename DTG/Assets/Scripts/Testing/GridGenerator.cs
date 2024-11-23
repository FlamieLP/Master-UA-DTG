using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D map;
    
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private float stepSizeSource = 1f, stepSizeTarget;
    [SerializeField] private float maxSpeed = 4f;
    
    [SerializeField] private Transform[,] _sourceNodes, _targetNodes;
    [SerializeField] private GameObject sourceRoot, targetRoot;

    [Header("Spring Model")] [SerializeField]
    private SpringMassModel model;

    private (int,int) GetDimensions()
    {
        return (
            map.width+1,
            map.height+1
        );
    }
    
    public Vector3[,] GetSourceGrid()
    {
        var (width, height) = GetDimensions();
        Vector3[,] grid = new Vector3[width, height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                grid[y, x] = _sourceNodes[y, x].localPosition;
            }
        }

        return grid;
    }
    
    public Vector3[,] GetTargetGrid()
    {
        var (width, height) = GetDimensions();
        Vector3[,] grid = new Vector3[width, height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                grid[y, x] = _targetNodes[y, x].position;
            }
        }

        return grid;
    }
    
    [ContextMenu("Converge Grid")]
    public void ConvergeGrid()
    {
        model.Init(_sourceNodes, map, maxSpeed);
        model.NextSteps(100);
        model.UpdateNodes(_sourceNodes);
        
    }
    
    [ContextMenu("Share Grid")]
    public void ShareGrid()
    {
        model.Init(_targetNodes, map, maxSpeed);
    }

    [ContextMenu("Create new Node Grid")]
    public void CreateNodes()
    {
        Setup();
        _sourceNodes = CreateNodeGridUniform(targetRoot.transform, stepSizeSource);//CreateNodeGrid(sourceRoot.transform);
        _targetNodes = CreateNodeGridUniform(targetRoot.transform, stepSizeTarget);
    }

    private Transform[,] CreateNodeGrid(Transform parent)
    {
        var (width, height) = GetDimensions();
        print("Width = " + width);
        print("Height = " + height);
        Transform[,] nodeList = new Transform[height, width];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var node = Instantiate(nodePrefab, parent);
                if (x == 0 && y == 0)
                {
                    node.name = $"Node: {x} | {y}";
                    node.transform.localPosition = new Vector3(
                        x,
                        0,
                        y
                    );
                    nodeList[y,x] = node.transform;
                }
                else
                {
                    float gainX = stepSizeSource;
                    float gainY = stepSizeSource;
                    if (x > 0 || y > 0)
                    {
                        var gainColor = map.GetPixel(Mathf.Max(0,x - 1), Mathf.Max(0,y - 1));
                        gainX /= Mathf.Lerp(1, maxSpeed, gainColor.r);
                        gainY /= Mathf.Lerp(1, maxSpeed, gainColor.b);
                    }
                    node.name = $"Node: {x} | {y}";
                    var xOffset = x == 0 ? 0 : nodeList[y, x - 1].localPosition.x + gainX;
                    var yOffset = y == 0 ? 0 : nodeList[y - 1, x].localPosition.z + gainY;
                    node.transform.localPosition = new Vector3(
                        xOffset,
                        0,
                        yOffset
                    );
                    nodeList[y,x] = node.transform;
                }
            }
        }

        return nodeList;
    }
    
    private Transform[,] CreateNodeGridUniform(Transform parent, float stepSize)
    {
        var (width, height) = GetDimensions();
        print("Width = " + width);
        print("Height = " + height);
        Transform[,] nodeList = new Transform[height, width];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var node = Instantiate(nodePrefab, parent);
                if (x == 0 && y == 0)
                {
                    node.name = $"Node: {x} | {y}";
                    node.transform.localPosition = new Vector3(
                        x,
                        0,
                        y
                    );
                    nodeList[y,x] = node.transform;
                }
                else
                {
                    float gainX = stepSize;
                    float gainY = stepSize;
                    node.name = $"Node: {x} | {y}";
                    var xOffset = x == 0 ? 0 : nodeList[y, x - 1].localPosition.x + gainX;
                    var yOffset = y == 0 ? 0 : nodeList[y - 1, x].localPosition.z + gainY;
                    node.transform.localPosition = new Vector3(
                        xOffset,
                        0,
                        yOffset
                    );
                    nodeList[y,x] = node.transform;
                }
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
