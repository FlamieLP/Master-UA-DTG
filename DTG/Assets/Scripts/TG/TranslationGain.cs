using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TranslationGain : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("1 means no Translation Gain and 2 a 100% increase.")]
    private float xGain = 1, zGain = 1;

    [SerializeField] private Transform source;
    [SerializeField] private Transform orignSource;
    [SerializeField] private Transform origin;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(source);
        Assert.IsNotNull(orignSource);
        Assert.IsNotNull(origin);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyTranslationGain();
    }

    private void ApplyTranslationGain()
    {
        Vector3 pos = transform.position;
        Vector3 sourcePos = source.position - orignSource.position;
        Vector3 newPos = new Vector3(
            sourcePos.x * xGain,
            pos.y,
            sourcePos.z * zGain
        );

        transform.position = origin.position + newPos;
    }
}
