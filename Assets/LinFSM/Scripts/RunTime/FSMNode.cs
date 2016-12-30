using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class FSMNode : ScriptableObject
{
    public FSMTransition[] Transitions = new FSMTransition[0];
    public FSMAction[] ActionSequnce = new FSMAction[0];
    public const int NODE_WIDTH = 200;
    public const int  NODE_HEIGHT=50;

    public FSMNode()
    {
        name = "FSMNode";
    }
   
    public string Description;
    
    public Vector2 position;
    public  bool IsStartNode=false;
    public bool IsSequence;
    public Rect PosRect
    {
        get {return new Rect(position.x-NODE_WIDTH*0.5f, position.y-NODE_HEIGHT*0.5f, NODE_WIDTH, NODE_HEIGHT); }
    }



    public int color;
    public string comment;

    [SerializeField]
    private new string name;

    public string Name
    {
        get { return this.name; }
        set
        {
            this.name = value;
            base.name = value;
        }
    }

    public void DelectTransition(FSMTransition transition)
    {
        ArrayUtility.Remove(ref Transitions, transition);
    }
}
