using UnityEngine;
using System.Collections;
using UnityEditor;
[System.Serializable]
public class LinStateMachine : FSMState
{
    public LinStateMachine parent;
    public FSMNode[] Nodes=new FSMNode[0];

    public void DeleteNode(FSMNode node)
    {
        ArrayUtility.Remove(ref Nodes,node);
    }

    public void AddState(FSMState state)
    {
        ArrayUtility.Add(ref Nodes,state);
    }
}
