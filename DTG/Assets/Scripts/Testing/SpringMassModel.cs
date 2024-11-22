using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringMassModel : MonoBehaviour
{
    [SerializeField] private int steps = 1;
    private List<Node> nodes = new List<Node>();
    private List<Spring> springs = new List<Spring>();

    public void UpdateNodes(Transform[,] output)
    {
        var (width, height) = (
            output.GetLength(1),
            output.GetLength(0)
        );

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                Vector2 pos = nodes[index].pos;
                output[y, x].position = new Vector3(pos.x, 0, pos.y);
            }
        }
    }

    [ContextMenu("Next")]
    public void NextSteps()
    {
        NextSteps(steps);
    }
    
    public void NextSteps(float stepCount)
    {
        for (int n = 0; n < stepCount; n++)
        {
            NextStep();
        }
    }
    
    private void NextStep()
    {
        float t = 0.01f;
        foreach (Spring spring in springs)
        {
            spring.ApplyForce();
        }
        foreach (Node node in nodes)
        {
            node.Update(t);
        }
    }

    public void Init(Transform[,] initialNodes, Texture2D map)
    {
        nodes.Clear();
        springs.Clear();
        
        var (width, height) = (
            initialNodes.GetLength(1),
            initialNodes.GetLength(0)
        );

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 nodePos = initialNodes[y, x].position;
                Vector2 position = new Vector2(nodePos.x, nodePos.z);
                bool stickyX = x == 0 || x == width - 1;
                bool stickyY = y == 0 || y == height - 1;
                nodes.Add(new Node((x,y),position,stickyX,stickyY));
            }
        }
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                //Top Spring
                if (y < height - 1)
                {
                    var mapIndex1 = Mathf.Clamp(x, 0, width - 1);
                    var mapIndex2 = Mathf.Clamp(x-1, 0, width - 1);
                    var gain1 = map.GetPixel(mapIndex1, y).r;
                    var gain2 = map.GetPixel(mapIndex2, y).r;
                    var factor = (gain1+ gain2) / 2f;
                    var restLength = Mathf.Lerp(1, .25f, factor);
                    
                    springs.Add(new Spring(nodes[index], nodes[index + width], restLength));
                }
                // Right Spring
                if (x < width - 1)
                {
                    var mapIndex1 = Mathf.Clamp(y, 0, height - 1);
                    var mapIndex2 = Mathf.Clamp(y-1, 0, height - 1);
                    var gain1 = map.GetPixel(x, mapIndex1).r;
                    var gain2 = map.GetPixel(x, mapIndex2).r;
                    var factor = (gain1+ gain2) / 2f;
                    var restLength = Mathf.Lerp(1, .25f, factor);
                    
                    springs.Add(new Spring(nodes[index], nodes[index + 1], restLength));
                }
                //Top Right Spring
                if (y < height - 1 && x < height-1)
                {
                    var mapIndex = Mathf.Clamp(y, 0, height - 1);
                    var gain = map.GetPixel(mapIndex, y).r;
                    var restLength = Mathf.Lerp(1, .25f, gain);
                    
                    springs.Add(new Spring(nodes[index], nodes[index + 1 + width], restLength));
                }
                //Top Left Spring
                if (y < height - 1 && x > 0)
                {
                    var mapIndex = Mathf.Clamp(x-1, 0, height - 1);
                    var gain = map.GetPixel(mapIndex, y).r;
                    var restLength = Mathf.Lerp(1, .25f, gain);
                    
                    springs.Add(new Spring(nodes[index], nodes[index - 1 + width], restLength));
                }
            }
        }

    }
    
    void OnDrawGizmos()
    {
        if (nodes == null || springs == null) return;

        // Draw nodes
        Gizmos.color = Color.red;
        foreach (Node node in nodes)
        {
            Gizmos.DrawSphere(node.pos, 0.1f);
        }

        // Draw springs
        Gizmos.color = Color.blue;
        foreach (Spring spring in springs)
        {
            Gizmos.DrawLine(spring.nodeA.pos, spring.nodeB.pos);
        }
    }

    private class Node
    {
        public int x,y;
        public Vector2 pos, oldPos;
        public Vector2 acceleration;
        public bool stickyX, stickyY;
        
        public Node((int,int) index, Vector2 position, bool stickyX, bool stickyY)
        {
            (x,y) = index;
            pos = position;
            oldPos = position;
            acceleration = Vector2.zero;
            this.stickyX = stickyX;
            this.stickyY = stickyY;
        }
        
        public void AddForce(Vector2 force)
        {
            acceleration += force;
        }
        
        public void Update(float deltaTime)
        {
            if (stickyX)
            {
                acceleration.x = 0;
            }
            if (stickyY)
            {
                acceleration.y = 0;
            }
            // Verlet Integration
            Vector2 temp = pos;
            var velocity = (pos - oldPos) * 0.8f;
            pos += velocity + acceleration * deltaTime * deltaTime;
            oldPos = temp;

            // Reset acceleration for the next frame
            acceleration = Vector2.zero;
        }
    }

    private class Spring
    {
        public Node nodeA;
        public Node nodeB;
        public float restLength;
        public float stiffness = 50;
        
        public Spring(Node nodeA, Node nodeB, float length)
        {
            this.nodeA = nodeA;
            this.nodeB = nodeB;
            restLength = Vector2.Distance(nodeA.pos, nodeB.pos) * length;//length;
        }
        
        public void ApplyForce()
        {
            Vector2 direction = nodeB.pos - nodeA.pos;
            float currentLength = Mathf.Min(direction.magnitude, 2*restLength);
            float displacement = currentLength - restLength;
            Vector2 force = direction.normalized * (displacement * stiffness);

            if (nodeB.x == 0 && nodeB.y == 1 && nodeA.x == 0 && nodeA.y == 0)
            {
                print($"Length = {currentLength} - RestLength = {restLength}");
                print($"Node Force = {force}");
            }

            Vector2 relVelocity = (nodeA.pos - nodeA.oldPos) - (nodeB.pos - nodeB.oldPos);
            Vector2 dampForce = Vector2.Dot(relVelocity, direction.normalized) * 14.14f * direction.normalized;

            nodeA.AddForce(force + dampForce);
            nodeB.AddForce(-force -dampForce);
        }
    }
}