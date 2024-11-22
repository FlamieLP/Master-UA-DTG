using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedMap : MonoBehaviour
{
    [SerializeField] private Vector2 dimensions = Vector2.one;
    [SerializeField] private Texture2D truth;
    [SerializeField] private Texture2D speedMap;

    [SerializeField] private GameObject map;

    private void Start()
    {
        Graphics.CopyTexture(truth, speedMap);
        map.GetComponent<Renderer>().sharedMaterial.mainTexture = speedMap;
    }

    public void SetPixel(Vector3 hitPoint, Color color)
    {
        var width = speedMap.width;
        var height = speedMap.height;
        Vector2 pos = ToMapPosition(hitPoint);
        int pixelPosX = (int)Mathf.Floor(pos.x * width);
        int pixelPosY = (int)Mathf.Floor(pos.y * height);
        
        speedMap.SetPixel(pixelPosX, pixelPosY, color);
        speedMap.Apply();
        print($"Set Color: ({pos.x}/{pos.y}) to pixel ({pixelPosX}/{pixelPosY})");
    }
    
    public Color GetColor(Vector3 hitPoint)
    {
        var width = speedMap.width;
        var height = speedMap.height;
        Vector2 pos = ToMapPosition(hitPoint);
        int pixelPosX = (int)Mathf.Floor(pos.x * width);
        int pixelPosY = (int)Mathf.Floor(pos.y * height);
        
        return speedMap.GetPixel(pixelPosX, pixelPosY);
    }

    private Vector2 ToMapPosition(Vector3 pos)
    {
        var relativePos = pos - transform.position;
        float x = relativePos.x / dimensions.x;
        float z = relativePos.z / dimensions.y;

        return new Vector2(x, z);
    }
}
