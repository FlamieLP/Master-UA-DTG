using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverse2DQI : MonoBehaviour
{
    [SerializeField] private GridGenerator generator;
    [SerializeField] private Vector3[,] sourcGrid = new Vector3[3,3], targetGrid = new Vector3[3,3];

    [SerializeField] private float l, m;
    
    // Start is called before the first frame update
    void Start()
    {
        sourcGrid = new Vector3[3,3]
        {
            {new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(2,0,0)},
            {new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(2,0,1)},
            {new Vector3(0,0,2), new Vector3(1,0,2), new Vector3(2,0,2)},
        };
        targetGrid = new Vector3[3,3]
        {
            {new Vector3(5,0,0), new Vector3(6,0,0), new Vector3(7,0,0)},
            {new Vector3(5,0,1), new Vector3(6,0,1), new Vector3(7,0,1)},
            {new Vector3(5,0,2), new Vector3(6,0,2), new Vector3(7,0,2)},
        };
    }

    [ContextMenu("Update Grid")]
    public void UpdateGrid()
    {
        sourcGrid = generator.GetSourceGrid();
        targetGrid = generator.GetTargetGrid();
    }
    
    Vector4 GetCoefficient(Vector4 points)
    {
        Matrix4x4 m = new Matrix4x4(
            new(1f,-1f,-1f,1f),
            new(0f,1f,0f,-1f),
            new(0f,0f,0f,1f),
            new(0f,0f,1f,-1f)
        );
        return m * points;
    }

    Vector2 Interpolate(float x, float y, Vector4 xs, Vector4 ys)
    {
        var aa = xs.w * ys.z - xs.z * ys.w;
        var bb = xs.w * ys.x - xs.x * ys.w + xs.y * ys.z - xs.z * ys.y + x * ys.w - y * xs.w;
        var cc = xs.y * ys.x - xs.x * ys.y + x * ys.y - y*xs.y;

        var det = Mathf.Sqrt(bb * bb - 4 * aa * cc);
        m = (Mathf.Abs(aa) < float.Epsilon) ? -cc/bb : (-bb + det) / (2 * aa);

        l = (x - xs.x - (xs.z * m)) / (xs.y + (xs.w * m));
        return new(l, m);
    }

    Vector2 Interpolate(Vector3 pos, Quad quad)
    {
        var xs = GetCoefficient(quad.GetXs());
        var ys = GetCoefficient(quad.GetYs());

        return Interpolate(pos.x, pos.z, xs, ys);
    }
    
    NodeMap.NodeSection Interpolate(Vector3 pos, Vector3[,] nodes)
    {
        if (nodes.GetLength(0) < 2 || nodes.GetLength(1) < 2)
        {
            throw new ArgumentException("Dimensions too small");
        }

        for (int y = 0; y < nodes.GetLength(0) - 1; y++)
        {
            for (int x = 0; x < nodes.GetLength(1) - 1; x++)
            {
                Quad q = new Quad(
                    nodes[y, x],
                    nodes[y, x+1],
                    nodes[y+1, x+1],
                    nodes[y+1, x]
                );
                var ratio = Interpolate(pos, q);
                if (ratio.x >= 0 && ratio.x <= 1 && ratio.y >= 0 && ratio.y <= 1)
                {
                    return new NodeMap.NodeSection(x, y, ratio.x, ratio.y);
                }
            }
        }
        return new NodeMap.NodeSection(0, 0, 0, 0);
    }
    
    public Vector3 GetMappedPosition(Vector3 pos)
    {
        var section = Interpolate(pos, sourcGrid);
        var ratios = new Vector2(section.ratioH, section.ratioV);
        var x = section.x;
        var y = section.y;
        Vector3 bl = targetGrid[y, x];
        Vector3 br = targetGrid[y, x+1];
        Vector3 tr = targetGrid[y+1, x+1];
        Vector3 tl = targetGrid[y+1, x];
        
        var alpha1=(1-ratios.y)*(1-ratios.x);
        var alpha2=ratios.x*(1-ratios.y);
        var alpha3=ratios.y*ratios.x;
        var alpha4=(1-ratios.x)*ratios.y;
        
        return alpha1*bl+alpha2*br+alpha3*tr+alpha4*tl;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var pos in sourcGrid)
        {
            Gizmos.DrawSphere(pos, 0.1f);
        }
    
        Gizmos.color = Color.red;
        foreach (var pos in targetGrid)
        {
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }

    [System.Serializable]
    public struct Quad
    {
        public Vector3 v1, v2, v3, v4;

        public Quad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
        }

        public Vector4 GetXs()
        {
            return new Vector4(
                v1.x,
                v2.x,
                v3.x,
                v4.x
            );
        }
        
        public Vector4 GetYs()
        {
            return new Vector4(
                v1.z,
                v2.z,
                v3.z,
                v4.z
            );
        }
    }
}
