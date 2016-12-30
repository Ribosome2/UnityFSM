using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
[InitializeOnLoad]
public class HierarchyAddOn : MonoBehaviour {

    static HierarchyAddOn()
		{
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
		}

    static void HierarchyWindowItemCallback(int pID, Rect pRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(pID) as GameObject;
        if (go != null && go.GetComponent<FSMBehavior>() != null)
        {
            Rect rect = new Rect(pRect.x + pRect.width-30, pRect.y - 3, 30, 25);
            GUI.Label(rect,"FSM");
        }
        #region 支持拖放一个FSM资源到Hierarchy上面后自动添加需要的对应组件
        Event ev = Event.current;
        if (ev.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            var selectedObjects = new List<GameObject>();
            foreach (var objectRef in DragAndDrop.objectReferences)
            {
                if (objectRef is LinStateMachine)
                {
                    if (pRect.Contains(ev.mousePosition))
                    {
                        var gameObject = (GameObject)EditorUtility.InstanceIDToObject(pID);
                        var componentX = gameObject.AddComponent<FSMBehavior>();
                        componentX.Machine = objectRef as LinStateMachine;
                        selectedObjects.Add(gameObject);
                    }
                }
            }

            if (selectedObjects.Count == 0)
            {
                return;
            }
            Selection.objects = selectedObjects.ToArray();
            ev.Use();
        }
        #endregion
    }
}
