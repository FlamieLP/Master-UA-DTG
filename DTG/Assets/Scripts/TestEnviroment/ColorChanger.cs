using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private new Renderer renderer;
    [Header("Options")]
    [SerializeField] private Color color;
    [SerializeField] private TestObject.SColor sColor;
    [SerializeField] private float alpha;

    public void Start()
    {
        var alphaColor = color;
        alphaColor.a = alpha;
        renderer.material.color = alphaColor;
    }

    public Color GetColor()
    {
        return color;
    }
    
    public TestObject.SColor GetSColor()
    {
        return sColor;
    }
}
