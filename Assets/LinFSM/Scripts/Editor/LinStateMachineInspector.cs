using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
[CustomEditor(typeof(LinStateMachine))]
public class LinStateMachineInspector :FSMNodeInspector{

    public override void OnEnable()
    {
        base.OnEnable();
        EditorApplication.projectWindowItemOnGUI += OnDoubleClickProjectItem;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        EditorApplication.projectWindowItemOnGUI -= OnDoubleClickProjectItem;

    }

    /// <summary>
    /// 支持双击资源打开窗口
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="rect"></param>
    public virtual void OnDoubleClickProjectItem(string guid, Rect rect)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && rect.Contains(Event.current.mousePosition))
        {
            LinFsmEditor.Instance.SelectFSM((LinStateMachine)target);
        }
    }
}
