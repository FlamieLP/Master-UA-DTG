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
    
    private Vector3 lastPos = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        lastPos = source.position;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGain();
        if (slower.action.WasPressedThisFrame())
        {
            gain = Mathf.Clamp(gain - 0.2f, 0, 10);
            text.text = ""+GetSpeed();
        }
        if (faster.action.WasPressedThisFrame())
        {
            gain = Mathf.Clamp(gain + 0.2f, 0, 10);
            text.text = ""+GetSpeed();
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
