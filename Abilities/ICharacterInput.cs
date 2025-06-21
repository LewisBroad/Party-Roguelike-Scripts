using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterInput
{
    bool GetPrimary();
    bool GetPrimaryHeld();
    bool GetSecondary();
    bool GetSecondaryHeld();
    bool GetSkill(int index); //0-3 for 4 skills
    Vector3 GetMovement();
    Vector3 GetLookDirection();

    bool GetJump();
    bool GetRun();
}
