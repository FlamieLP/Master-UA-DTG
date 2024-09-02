using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerKeyboardController : MonoBehaviour
{

    [SerializeField] private float speed = 1f;

    void Update()
    {
        //Vector3 inputVec = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); //Sadly not working with VR
        var input = Keyboard.current;
        Vector3 clunkyInput = new(
            input.dKey.wasPressedThisFrame ? 1 : input.aKey.wasPressedThisFrame ? -1 : 0,
            0,
            input.wKey.wasPressedThisFrame ? 1 : input.sKey.wasPressedThisFrame ? -1 : 0
        );
        transform.position += clunkyInput * speed;
    }
}
