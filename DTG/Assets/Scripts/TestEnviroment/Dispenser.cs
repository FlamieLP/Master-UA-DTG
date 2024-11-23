using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private TestObject testObjectPrefab;

    [Header("Options")] 
    [SerializeField] private Transform dispenserPoint;

    private TestObject testObject;

    public void Dispense()
    {
        if (testObject)
        {
            Destroy(testObject.gameObject);
        }

        testObject = Instantiate(testObjectPrefab, dispenserPoint.position, Quaternion.identity);
    }
}
