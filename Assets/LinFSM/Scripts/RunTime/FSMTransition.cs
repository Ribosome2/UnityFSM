using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSMTransition : ScriptableObject
{
    public FSMTransition(FSMNode target)
    {
        TagetState = target;
    }
    public FSMNode TagetState;
    public List<FSMCondition> conditions;
}
