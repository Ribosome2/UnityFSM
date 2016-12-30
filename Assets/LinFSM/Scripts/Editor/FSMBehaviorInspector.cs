using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FSMBehavior))]
public class FSMBehaviorInspector : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        FSMBehavior behavior=target as FSMBehavior;
        if (GUILayout.Button("ShowInEditor"))
        {
           LinFsmEditor.Instance.SelectFSM(behavior.Machine);
        }
    }
}
