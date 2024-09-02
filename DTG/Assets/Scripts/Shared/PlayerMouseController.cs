using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseController : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float distance = 0.2f;
    
    [SerializeField] private List<Vector3> targets = new List<Vector3>();

    private Camera _mainCamera;
    
    void Awake()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        CalcTargetPosition();
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (targets.Count == 0)
        {
            return;
        }
        
        Vector3 targetPos = targets[0];
        Vector3 pos = transform.position;

        if (Vector3.SqrMagnitude(targetPos - pos) > distance * distance)
        {
            transform.position = Vector3.MoveTowards(pos, targetPos, speed * Time.deltaTime);
        }
        else
        {
            targets.RemoveAt(0);
        }
    }

    private void CalcTargetPosition()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        Ray ray = _mainCamera.ScreenPointToRay(mouse.position.value);
        RaycastHit hit;
        if (keyboard.ctrlKey.isPressed && mouse.leftButton.wasPressedThisFrame && Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            targets.Add(hit.point);
        }
        else if (mouse.leftButton.wasPressedThisFrame && Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            targets.Clear();
            targets.Add(hit.point);
        }
    }

    private void OnDrawGizmos()
    {
        int i = 0;
        Vector3 start = transform.position;
        
        while (i < targets.Count)
        {
            float step = i/(float)(targets.Count);
            Gizmos.color = new Color(Mathf.Lerp(1, 0, step),0,step, Mathf.Lerp(1f, .5f, step));
            Gizmos.DrawLine(start, targets[i]);
            start = targets[i];
            i++;
        }
    }
}
