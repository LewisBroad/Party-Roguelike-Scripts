using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSensitivityController : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target)
        {
            transform.position = target.position;
        }
    }
}
