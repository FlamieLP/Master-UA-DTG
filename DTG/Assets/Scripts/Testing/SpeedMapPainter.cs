using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpeedMapPainter : MonoBehaviour
{
    [SerializeField] private LayerMask speedMapLayer;
    [SerializeField] private Color color = new Color(1, 0, 1);
    [SerializeField] private float colorAdjustmentPerSecond = 1f;
    [SerializeField] private Renderer indicator;

    [SerializeField] private InputActionReference paint;
    [SerializeField] private InputActionReference pick;
    [SerializeField] private InputActionReference brushAxis;

    private void Update()
    {
        if (paint.action.IsPressed())
        {
            Paint();
        }
        if (pick.action.IsPressed())
        {
            Pick();
        }
        if (brushAxis.action.WasPerformedThisFrame())
        {
            float xAxis = brushAxis.action.ReadValue<Vector2>().x;
            float yAxis = brushAxis.action.ReadValue<Vector2>().y;
            float xAxisSignedSquare = Mathf.Sign(xAxis) * Mathf.Abs(Mathf.Pow(xAxis, 2));
            float yAxisSignedSquare = Mathf.Sign(yAxis) * Mathf.Abs(Mathf.Pow(yAxis, 2));
            Vector2 colorAdjustment = new Vector2(xAxisSignedSquare, yAxisSignedSquare) * (colorAdjustmentPerSecond * Time.deltaTime);
            var c = new Color(Mathf.Clamp01(color.r + colorAdjustment.x),0,Mathf.Clamp01(color.b + colorAdjustment.y), 1);
            SetColor(c);
        }
    }

    private void Paint()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out var hit, 10f, speedMapLayer))
        {
            if (hit.collider.transform.parent.TryGetComponent(out SpeedMap map))
            {
                map.SetPixel(hit.point, color);
            }
        }
    }
    
    private void Pick()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out var hit, 10f, speedMapLayer))
        {
            if (hit.collider.transform.parent.TryGetComponent(out SpeedMap map))
            {
                SetColor(map.GetColor(hit.point));
            }
        }
    }

    private void SetColor(Color c)
    {
        color = c;
        indicator.material.color = c;
    }
}
