using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShapeChanger : MonoBehaviour
{
    [SerializeField] private TestObject.Shape shape = TestObject.Shape.CUBE;
    [SerializeField] private Transform point;
    [SerializeField] private Collider coll;
    
    [SerializeField] private TMP_Text[] uiShape;

    public void Start()
    {
        ChangeShape(shape);
    }

    public TestObject.Shape GetShape()
    {
        return shape;
    }
    
    public Vector3 GetPos()
    {
        return point.position;
    }

    public void ToBall()
    {
        ChangeShape(TestObject.Shape.BALL);
    }
    
    public void ToTorus()
    {
        ChangeShape(TestObject.Shape.TORUS);
    }
    
    public void ToCube()
    {
        ChangeShape(TestObject.Shape.CUBE);
    }

    public void ChangeShape(TestObject.Shape s)
    {
        shape = s;
        coll.enabled = false;
        switch (shape)
        {
            case TestObject.Shape.BALL:
                DisplayText("BALL");
                break;
            case TestObject.Shape.TORUS:
                DisplayText("RING");
                break;
            case TestObject.Shape.CUBE:
                DisplayText("WÜRFEL");
                break;
        }

        coll.enabled = true;
    }

    private void DisplayText(String text)
    {
        foreach (TMP_Text tmpText in uiShape)
        {
            tmpText.text = text;
        }
    }
}
