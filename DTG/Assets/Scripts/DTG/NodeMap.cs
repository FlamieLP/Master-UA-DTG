using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


//TODO: NOT working yet. Use other Method to interpolate/map points
public class NodeMap : MonoBehaviour
{
    [SerializeField] private Node nodePrefab;
    
    [SerializeField] private Vector2 sourceDimensions = Vector2.one;
    [SerializeField] private int nodesPerAxis = 4;
    
    
    [SerializeField] private bool displayGrit = true;
    
    [SerializeField] private List<Node> _sourceNodes = new List<Node>(), _targetNodes = new List<Node>();
    [SerializeField] private GameObject _sourceRoot, _targetRoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Vector3 GetMappedPositionBicubic(Vector3 pos)
    {
        var section = GetNodeSection(pos);
        float fracx = Mathf.Clamp01(section.ratioH);
        float fracy = Mathf.Clamp01(section.ratioV);
        int x = section.x;
        int y = section.y;

        
        float x1 = CubicPolate( getNodeX(x-1,y-1), getNodeX(x,y-1), getNodeX(x+1,y-1), getNodeX(x+2,y-1), fracx );
        float x2 = CubicPolate( getNodeX(x-1,y), getNodeX(x,y), getNodeX(x+1,y), getNodeX(x+2,y), fracx );
        float x3 = CubicPolate( getNodeX(x-1,y+1), getNodeX(x,y+1), getNodeX(x+1,y+1), getNodeX(x+2,y+1), fracx );
        float x4 = CubicPolate( getNodeX(x-1,y+2), getNodeX(x,y+2), getNodeX(x+1,y+2), getNodeX(x+2,y+2), fracx );
        
        float xFinal = CubicPolate( x1, x2, x3, x4, fracy );
        
        float y1 = CubicPolate( getNodeY(x-1,y-1), getNodeY(x,y-1), getNodeY(x+1,y-1), getNodeY(x+2,y-1), fracx );
        float y2 = CubicPolate( getNodeY(x-1,y), getNodeY(x,y), getNodeY(x+1,y), getNodeY(x+2,y), fracx );
        float y3 = CubicPolate( getNodeY(x-1,y+1), getNodeY(x,y+1), getNodeY(x+1,y+1), getNodeY(x+2,y+1), fracx );
        float y4 = CubicPolate( getNodeY(x-1,y+2), getNodeY(x,y+2), getNodeY(x+1,y+2), getNodeY(x+2,y+2), fracx );
        
        float yFinal = CubicPolate( y1, y2, y3, y4, fracy );

        return new Vector3(xFinal, 0, yFinal);

    }
    
    private float CubicPolate( float v0, float v1, float v2, float v3, float frac ) { //0 1 2 3 -> 0.5 => 1.5
        float A = (v3-v2)-(v0-v1); // 3-2-(0-1) = 1 --1 = 2
        float B = (v0-v1)-A; // 0-1-2 = -3
        float C = v2-v0;  // 2-0 = 2
        float D = v1; //1

        return A*Mathf.Pow(frac,3)+B*Mathf.Pow(frac,2)+C*frac+D;
    }

    public Vector3 GetMappedPositionIDW(Vector3 pos)
    {
        float distSum = 0;
        Vector3 weightedSum = Vector3.zero;
        foreach (Node node in _sourceNodes)
        {
            float distWeight = calcDistWeight(pos, node.GetPos());
            
            if (distWeight == 0)
            {
                return node.GetPartnerPos();
            }
            
            distSum += distWeight;
            weightedSum += distWeight * node.GetPartnerPos();
        }

        return weightedSum / distSum;
    }

    private float calcDistWeight(Vector3 pos, Vector3 refPos)
    {
        pos.y = 0;
        refPos.y = 0;
        float dist = Mathf.Pow(Vector3.Distance(pos, refPos), 2f);
        return dist <= float.Epsilon ? 0 : 1 / dist;
    }

    public Vector3 GetMappedPosition(Vector3 pos)
    {
        var section = GetNodeSection(pos);
        int bl = section.y * nodesPerAxis + section.x;
        int br = section.y * nodesPerAxis + Mathf.Clamp(section.x+1, 0, nodesPerAxis-1);
        int tl = Mathf.Clamp(section.y+1, 0, nodesPerAxis-1) * nodesPerAxis + section.x;
        int tr = Mathf.Clamp(section.y+1, 0, nodesPerAxis-1) * nodesPerAxis + Mathf.Clamp(section.x+1, 0, nodesPerAxis-1);

        Vector3 tHorizontal = _targetNodes[tr].GetPos() - _targetNodes[tl].GetPos();
        Vector3 bHorizontal = _targetNodes[br].GetPos() - _targetNodes[bl].GetPos();

        tHorizontal *= section.ratioH;
        bHorizontal *= section.ratioH;

        tHorizontal += _targetNodes[tl].GetPos();
        bHorizontal += _targetNodes[bl].GetPos();

        var vertical = tHorizontal - bHorizontal;

        vertical *= section.ratioV;

        vertical += bHorizontal;

        return vertical;
    }

    private NodeSection GetNodeSection(Vector3 pos)
    {
        var relPos = pos - _sourceNodes[0].GetPos();
        relPos = new(relPos.x / sourceDimensions.x, 0, relPos.z / sourceDimensions.y);
        relPos *= (nodesPerAxis-1);

        float clampedX = Mathf.Clamp(relPos.x, 0, nodesPerAxis - 1);
        float clampedZ = Mathf.Clamp(relPos.z, 0, nodesPerAxis - 1);

        int x = Mathf.FloorToInt(clampedX);
        int y = Mathf.FloorToInt(clampedZ);

        return new NodeSection(x, y, clampedX - x, clampedZ - y);
    }

    [ContextMenu("Create new Node Grit")]
    private void CreateNodes()
    {
        Setup();
        _sourceNodes = CreateNodeGrit(_sourceRoot.transform);
        _targetNodes = CreateNodeGrit(_targetRoot.transform);
        PairNodes(_sourceNodes, _targetNodes);
    }

    private List<Node> CreateNodeGrit(Transform parent)
    {
        List<Node> nodeList = new List<Node>();
        Vector2 adjustForDimension = sourceDimensions / (nodesPerAxis - 1);
        
        for (var y = 0; y < nodesPerAxis; y++)
        {
            for (var x = 0; x < nodesPerAxis; x++)
            {
                var index = (y * nodesPerAxis) + x;
                Node node = Instantiate(nodePrefab, parent);
                node.name = $"Node: {x} | {y}";
                node.transform.localPosition = new Vector3(
                    x * adjustForDimension.x,
                    0,
                    y * adjustForDimension.y
                );
                nodeList.Add(node);
            }
        }

        return nodeList;
    }

    private void PairNodes(List<Node> list1, List<Node> list2) 
    {
        Assert.AreEqual(list1.Count, list2.Count);
        
        for (int i = 0; i < list1.Count; i++)
        {
            list1[i].PairTo(list2[i]);
        }
    }

    private void Setup()
    {
        if (!_sourceRoot)
        {
            _sourceRoot = new GameObject("Source Root")
            {
                transform =
                {
                    parent = transform
                }
            };
        }
        if (!_targetRoot)
        {
            _targetRoot = new GameObject("Target Root")
            {
                transform =
                {
                    parent = transform
                }
            };
        }
    }

    private float getNodeX(int x, int y)
    {
        int cX = Mathf.Clamp(x, 0, nodesPerAxis - 1);
        int cY = Mathf.Clamp(y, 0, nodesPerAxis - 1);
        return _sourceNodes[cY * nodesPerAxis + cX].GetPartnerPos().x + (cX-x);
    }
    
    private float getNodeY(int x, int y)
    {
        int cX = Mathf.Clamp(x, 0, nodesPerAxis - 1);
        int cY = Mathf.Clamp(y, 0, nodesPerAxis - 1);
        return _sourceNodes[cY * nodesPerAxis + cX].GetPartnerPos().z + + (cY-y);
    }

    private void OnDrawGizmosSelected()
    {
        if (!displayGrit) return;
        
        Gizmos.color = Color.cyan;
        foreach (var node in _sourceNodes)
        {
            Gizmos.DrawSphere(node.transform.position, 0.1f);
        }
        Gizmos.color = Color.green;
        foreach (var node in _targetNodes)
        {
            Gizmos.DrawSphere(node.transform.position, 0.1f);
        }
    }
    
    public struct NodeSection
    {
        public int x, y;
        public float ratioH, ratioV;

        public NodeSection(int x, int y, float ratioH, float ratioV)
        {
            this.x = x;
            this.y = y;
            this.ratioH = ratioH;
            this.ratioV = ratioV;
        }

        public override string ToString()
        {
            return $"NodeSection: {x} | {y} - h = {ratioH} | v = {ratioV}";
        }
    }
}
