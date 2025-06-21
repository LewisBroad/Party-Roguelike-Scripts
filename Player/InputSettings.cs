using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputSettings", menuName = "Game/InputSettings")]
public class InputSettings : ScriptableObject
{
    [Header("Movement")]
    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode jump = KeyCode.Space;
    public KeyCode run = KeyCode.LeftShift;

    [Header("Sensitivity")]
    public float mouseSensitivityX = 5f;
    public float mouseSensitivityY = 5f;
    public float controllerSensitivityX = 5f;
    public float controllerSensitivityY = 5f;



    [Header("Abilities")]
    public KeyCode primaryFire = KeyCode.Mouse0;
    public KeyCode secondaryFire = KeyCode.Mouse1;

    public KeyCode ability1 = KeyCode.Alpha1;
    public KeyCode ability2 = KeyCode.Alpha2;
    public KeyCode ability3 = KeyCode.R;
    public KeyCode ability4 = KeyCode.Q;

    [Header("Misc")]
    public KeyCode pause = KeyCode.Escape;
    public KeyCode interact = KeyCode.E;

}
