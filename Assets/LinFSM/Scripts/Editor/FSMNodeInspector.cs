using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// ScriptableObject 也可以自定义 Inspecor
/// </summary>
[CustomEditor(typeof(FSMNode))]
public class FSMNodeInspector : Editor {

  
    protected FSMNode node;

    public virtual void OnEnable()
    {
        node = target as FSMNode;
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
      
    }

    public virtual void OnDisable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }

    protected override void OnHeaderGUI()
    {
        GUILayout.BeginVertical("IN BigTitle");
        EditorGUIUtility.labelWidth = 50;

        GUILayout.BeginHorizontal();

        node.Name = EditorGUILayout.TextField("Name", node.Name);
    
        GUILayout.EndHorizontal();
        GUILayout.Label("Description:");
        node.comment = EditorGUILayout.TextArea(node.comment, GUILayout.MinHeight(45));
        GUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(node);
        }
    }

    public static void Dirty()
    {
      //todo
    }

    protected virtual void MarkDirty()
    {
       //todo
    }
}
