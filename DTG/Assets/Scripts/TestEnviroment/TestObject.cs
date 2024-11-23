using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TestObject : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private XRGrabInteractable interactable;
    [SerializeField] private GameObject ball, torus, cube;
    
    [Header("Options")]
    [SerializeField] private Color color;
    [SerializeField] private SColor sColor;
    [SerializeField] private Shape shape = Shape.BALL;

    public void Start()
    {
        renderer.material.color = color;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ColorChanger colorChanger))
        {
            SetColor(colorChanger.GetColor(), colorChanger.GetSColor());
        }
        if (other.TryGetComponent(out ShapeChanger shapeChanger))
        {
            transform.position = shapeChanger.GetPos();
            rigidbody.velocity = Vector3.zero;
            SetShape(shapeChanger.GetShape());
        }
        if (other.TryGetComponent(out SolutionChecker solutionChecker))
        {
            solutionChecker.GiveSolution(new Solution(this));
            Destroy(gameObject);
        }
    }

    private void SetColor(Color c, SColor sc)
    {
        if (color == c) return;
        
        color = c;
        sColor = sc;
        renderer.material.color = color;
    }
    
    private void SetShape(Shape s)
    {
        if (shape == s) return;
        shape = s;

        interactable.enabled = false;

        HideAll();
        GameObject newShape = shape switch
        {
            Shape.BALL => ball,
            Shape.TORUS => torus,
            Shape.CUBE => cube,
            _ => renderer.gameObject
        };

        newShape.SetActive(true);
        renderer = newShape.gameObject.GetComponent<Renderer>();
        renderer.material.color = color;
        
        interactable.colliders.Clear();
        interactable.colliders.Add(newShape.GetComponent<Collider>());
        interactable.enabled = true; 
    }

    private void HideAll()
    {
        ball.SetActive(false);
        torus.SetActive(false);
        cube.SetActive(false);
    }

    public Solution GetSolution()
    {
        return new Solution(this);
    }

    [System.Serializable]
    public struct Solution
    {
        public SColor color;
        public Shape shape;

        public Solution(TestObject obj)
        {
            color = obj.sColor;
            shape = obj.shape;
        }
        
        public Solution(SColor c, Shape s)
        {
            color = c;
            shape = s;
        }

        public String GetShapeText()
        {
            return shape switch
            {
                Shape.BALL => "BALL",
                Shape.TORUS => "RING",
                Shape.CUBE => "WÜRFEL",
                _ => "FEHLER"
            };
        }
        
        public String GetColorText()
        {
            return color switch
            {
                SColor.RED => "roten",
                SColor.GREEN => "grünen",
                SColor.BLUE => "blauen",
                _ => "großen"
            };
        }
    }

    public enum Shape
    {
        BALL,
        TORUS,
        CUBE
    }
    public enum SColor
    {
        RED,
        GREEN,
        BLUE
    }
}
