using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeatMapManager : MonoBehaviour
{
    [SerializeField] private int width = 16, height = 16;
    
    [SerializeField] private Texture2D heatmap;
    [SerializeField] private GameObject floor;

    [SerializeField] private Transform lowerLeftCorner, upperRightCorner;
    private Vector3 _bounds = Vector3.zero;
    
    
    [SerializeField] private LayerMask targetLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        Ray ray = Camera.main.ScreenPointToRay(mouse.position.value);
        if (mouse.rightButton.wasPressedThisFrame && Physics.Raycast(ray, out var hit, Mathf.Infinity, targetLayer))
        {
            var pos = GetMapPosition(hit.point);
            if (keyboard.sKey.isPressed)
            {
                print($"Sample Gain: {GetGainFactor(pos)}");
            } else if (keyboard.numpadPlusKey.isPressed)
            {
                int pixelPosX = (int)Mathf.Round(pos.x * width);
                int pixelPosY = (int)Mathf.Round(pos.y * height);
                var color = heatmap.GetPixel(pixelPosX, pixelPosY);
                color.r = Mathf.Clamp(color.r + .1f, 0, 1);
                heatmap.SetPixel(pixelPosX, pixelPosY, color);
                heatmap.Apply();
                print("Brighter");
            } else if (keyboard.numpadMinusKey.isPressed)
            {
                int pixelPosX = (int)Mathf.Round(pos.x * width);
                int pixelPosY = (int)Mathf.Round(pos.y * height);
                var color = heatmap.GetPixel(pixelPosX, pixelPosY);
                color.r = Mathf.Clamp(color.r - .1f, 0, 1);
                heatmap.SetPixel(pixelPosX, pixelPosY, color);
                heatmap.Apply();
                print("Darker");
            }
        }
    }
    
    public Vector2 GetMapPosition(Vector3 worldPos)
    {
        if (!heatmap || !floor || !lowerLeftCorner || !upperRightCorner)
        {
            throw new MissingFieldException("Missing Information");
        }

        var relativePos = worldPos - lowerLeftCorner.position;

        if (relativePos.x < 0 || relativePos.z < 0 || relativePos.x > _bounds.x || relativePos.z > _bounds.z)
        {
            Debug.LogError($"Outside Heatmap - return Default; relativePos: {relativePos}, bounds: {_bounds}");
            return new Vector2(0, 0);
        }

        float x = relativePos.x / _bounds.x;
        float z = relativePos.z / _bounds.z;

        return new Vector2(x, z);
    }

    public GainFactor GetGainFactor(Vector2 relativePos)
    {
        return new GainFactor(heatmap.GetPixelBilinear(relativePos.x, relativePos.y));
    }

    [ContextMenu("Create new Heatmap")]
    private void CreateNewHeatmap()
    {
        Debug.Log("Creating Heatmap...");

        heatmap = new Texture2D(width, height, TextureFormat.RG16, false);
        var mapData = heatmap.GetPixels();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var index = (y * height) + x;
                mapData[index].r = x / (float)(width - 1);
                mapData[index].g = y / (float)(height - 1);
            }
        }
        heatmap.SetPixels(mapData);
        heatmap.Apply();
        Debug.Log("Finished Creating Heatmap!");
    }
    
    [ContextMenu("Apply Heatmap to Renderer")]
    private void ApplyMapToRenderer()
    {
        if (!heatmap)
        {
            Debug.LogError("No heatmap provided");
            return;
        }
        if (!floor)
        {
            Debug.LogError("No floor provided");
            return;
        }
        
        floor.GetComponent<Renderer>().sharedMaterial.mainTexture = heatmap;
    }
    
    [ContextMenu("Create Anchors")]
    private void CreateAnchors()
    {
        if (!floor)
        {
            Debug.LogError("No floor provided");
            return;
        }
        if (lowerLeftCorner && upperRightCorner)
        {
            Debug.LogError("Please Clear Current Anchors first");
            return;
        }

        var llAnchor = new GameObject("Lower Left Corner");
        var urAnchor = new GameObject("Upper Right Corner");

        lowerLeftCorner = llAnchor.transform;
        upperRightCorner = urAnchor.transform; 
        
        lowerLeftCorner.parent = transform;
        upperRightCorner.parent = transform;

        lowerLeftCorner.position = floor.transform.position;
        upperRightCorner.position = floor.transform.position;
    }
    
    [ContextMenu("Calculate Bounds")]
    private void CalcBounds()
    {
        if (!lowerLeftCorner || !upperRightCorner)
        {
            Debug.LogError("Please Set Corners first");
            return;
        }

        _bounds = upperRightCorner.position - lowerLeftCorner.position;
    }

    public struct GainFactor
    {
        public float xGain;
        public float yGain;

        public GainFactor(Color heatmapValue, float max = 1)
        {
            xGain = heatmapValue.r / max;
            yGain = heatmapValue.g / max;
        }
        
        public GainFactor(float x, float y)
        {
            xGain = x;
            yGain = y;
        }
        
        public override string ToString() =>
            $"Gain Factor: x: {xGain}; y: {yGain}";
    }
}
