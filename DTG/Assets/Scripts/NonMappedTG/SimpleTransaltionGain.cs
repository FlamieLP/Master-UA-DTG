using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SimpleTransaltionGain : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Transform source;
    [SerializeField] private float gain = 0f;

    [SerializeField] private InputActionReference slower, faster;
    [SerializeField] private InputActionReference speedAxis;
    
    [SerializeField] private float speedPerSecond = 1f;
    [SerializeField] private float minimalAdjust = 0.001f;
    [SerializeField] private float maxSpeed = 10f;
    
    private Vector3 lastPos = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        lastPos = source.position;
        text.text = $"{GetSpeed():00.000}";
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGain();
        if (slower.action.WasPressedThisFrame())
        {
            gain = Mathf.Clamp(gain - minimalAdjust, 0, maxSpeed);
            text.text = $"{GetSpeed():00.000}";
        }
        if (faster.action.WasPressedThisFrame())
        {
            gain = Mathf.Clamp(gain + minimalAdjust, 0, maxSpeed);
            text.text = $"{GetSpeed():00.000}";
        }
        if (speedAxis.action.WasPerformedThisFrame())
        {
            float axis = speedAxis.action.ReadValue<Vector2>().y;
            float axisSignedSquare = Mathf.Sign(axis) * Mathf.Abs(Mathf.Pow(axis, 2));
            float speedAdjustment = axisSignedSquare * speedPerSecond * Time.deltaTime;
            gain = Mathf.Clamp(gain + speedAdjustment, 0, maxSpeed);
            text.text = $"{GetSpeed():00.000}";
        }
    }

    public void SetSpeed(float speed)
    {
        gain = Mathf.Clamp(speed - 1,0, 10);
    }
    
    public float GetSpeed()
    {
        return gain+1;
    }

    private void ApplyGain()
    {
        var pos = source.position;
        var moveDir = pos - lastPos;
        moveDir.y = 0;

        transform.position += moveDir * gain;
        lastPos = source.position;
    }
}
