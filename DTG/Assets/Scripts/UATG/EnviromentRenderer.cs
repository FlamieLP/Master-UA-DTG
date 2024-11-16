using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnviromentRenderer : MonoBehaviour
{
    [SerializeField] private GameObject environmentFreeSpace, environmentDenseSpace;
    [SerializeField] private InputActionReference spawnFreeSpace, spawnDenseSpace;

    [SerializeField] private Transform player;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnFreeSpace.action.WasPerformedThisFrame())
        {
            SpawnFreeSpace();
            print("Free Space");
        }
        if (spawnDenseSpace.action.WasPerformedThisFrame())
        {
            SpawnDenseSpace();
            print("Dense Space");
        }
    }

    private void SpawnFreeSpace()
    {
        environmentDenseSpace.SetActive(false);
        environmentFreeSpace.SetActive(true);

        var pos = player.transform.position;
        pos.y = 0;

        environmentFreeSpace.transform.position = pos;
    }
    
    private void SpawnDenseSpace()
    {
        environmentFreeSpace.SetActive(false);
        environmentDenseSpace.SetActive(true);

        var pos = player.transform.position;
        pos.y = 0;

        environmentDenseSpace.transform.position = pos;
    }
}
