using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Node : MonoBehaviour
{
    public Node partner;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PairTo(Node partnerNode)
    {
        partner = partnerNode;
        partnerNode.partner = this;
    }

    public Vector3 GetPos()
    {
        return transform.position;
    }
    
    public Vector3 GetPartnerPos()
    {
        return partner.transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        if(!partner) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, partner.transform.position);
    }
}
