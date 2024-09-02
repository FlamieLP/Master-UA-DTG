using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MirrorMovement : MonoBehaviour
{
    [SerializeField] private Transform originTarget;
    [SerializeField] private Transform target;

    [SerializeField] private Transform origin;
    
    private Vector3 _mirrorPosition = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(originTarget);
        Assert.IsNotNull(target);
        Assert.IsNotNull(origin);
    }

    // Update is called once per frame
    void Update()
    {
        _mirrorPosition = GetMirrorPosition();
        transform.position = _mirrorPosition;
    }

    private Vector3 GetMirrorPosition()
    {
        return origin.position + (target.position - originTarget.position);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (target && originTarget && origin)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_mirrorPosition, 1);
        }
    }
}
