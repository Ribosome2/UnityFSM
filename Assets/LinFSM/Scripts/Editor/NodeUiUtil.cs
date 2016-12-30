using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class NodeUiUtil : MonoBehaviour
{
    public static Material material;
    public static void DrawConnection(Vector3 start, Vector3 end, Color color, int arrows, bool offset)
    {
        if (Event.current.type != EventType.repaint)
        {
            return;
        }

        Vector3 cross = Vector3.Cross((start - end).normalized, Vector3.forward);
        if (offset)
        {
            GetOffetTransitionPoints(start, end, out start, out end);
        }

        Texture2D tex = (Texture2D)UnityEditor.Graphs.Styles.connectionTexture.image;
        Handles.color = color;
        Handles.DrawAAPolyLine(tex, 5.0f, new Vector3[] { start, end });

        Vector3 vector3 = end - start;
        Vector3 vector31 = vector3.normalized;
        Vector3 vector32 = (vector3 * 0.5f) + start;
        vector32 = vector32 - (cross * 0.5f);
        Vector3 vector33 = vector32 + vector31;

        for (int i = 0; i < arrows; i++)
        {
            Vector3 center = vector33 + vector31 * 10.0f * i + vector31 * 5.0f - vector31 * arrows * 5.0f;
            DrawArrow(color, cross, vector31, center, 6.0f);
        }
    }

    /// <summary>
    /// 注意这里要跟上面函数的画连线的算法对上
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="outStart"></param>
    /// <param name="outEnd"></param>
    public static void GetOffetTransitionPoints(Vector3 start, Vector3 end, out Vector3 outStart, out Vector3 outEnd)
    {
        Vector3 cross = Vector3.Cross((start - end).normalized, Vector3.forward);
        outStart = start + cross * 6;
        outEnd = end + cross * 6;
        
    }
    public static  void DrawArrow(Color color, Vector3 cross, Vector3 direction, Vector3 center, float size)
    {
       
        Vector3[] vector3Array = new Vector3[]
            {
                center + (direction*size),
                (center - (direction*size)) + (cross*size),
                (center - (direction*size)) - (cross*size),
                center + (direction*size)
            };

        Color color1 = color;
        color1.r *= 0.8f;
        color1.g *= 0.8f;
        color1.b *= 0.8f;

        CreateMaterial();
        material.SetPass(0);

        GL.Begin(GL.TRIANGLES);
        GL.Color(color1);
        GL.Vertex(vector3Array[0]);
        GL.Vertex(vector3Array[1]);
        GL.Vertex(vector3Array[2]);
        GL.End();
    }

    public static void CreateMaterial()
    {
        if (material != null)
            return;

        material = new Material("Shader \"Lines/Colored Blended\" {" +
                                "SubShader { Pass { " +
                                "    Blend SrcAlpha OneMinusSrcAlpha " +
                                "    ZWrite Off Cull Off Fog { Mode Off } " +
                                "    BindChannels {" +
                                "      Bind \"vertex\", vertex Bind \"color\", color }" +
                                "} } }");
        material.hideFlags = HideFlags.HideAndDontSave;
        material.shader.hideFlags = HideFlags.HideAndDontSave;
    }

    /// <summary>
    /// 粗略的计算一个点是否在两点之间的线段上
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool IsPosOnLine(Vector2 startPos, Vector2 endPos, Vector2 pos)
    {
        //may need to find a better algo
        float lineDist = Vector2.Distance(startPos, endPos);
        if (Vector2.Distance(pos, startPos) + Vector2.Distance(pos, endPos) < lineDist + 1)
        {
            return true;
        }
        return false;
    }

    public static T AddNode<T>( LinStateMachine parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("Can't add node to parent state machine, because the parent state machine is null!");
            return default(T);
        }
        //注意ScriptableObject不能直接new 需要用ScriptableObject.CreateInstance（）
        FSMNode node = (FSMState)ScriptableObject.CreateInstance(typeof(T));
        node.hideFlags = HideFlags.HideInHierarchy;
     
        parent.Nodes = AddNode<FSMNode>(parent.Nodes, node);
       

        if (EditorUtility.IsPersistent(parent))
        {
            AssetDatabase.AddObjectToAsset(node, parent);
        }

        if (node.GetType() == typeof(LinStateMachine))
        {
            AnyState state = AddNode<AnyState>( (LinStateMachine)node);
            state.Name = "Any State";
        }

        AssetDatabase.SaveAssets();
        return (T)(object)node;
    }


    public static T[] AddNode<T>(T[] array, T item)
    {
        return (new List<T>(array)
			        {
				item
			}).ToArray();
    }

    public static void CreateTransition(FSMNode parent,FSMNode targetNode)
    {

         FSMTransition t = ArrayUtility.Find(parent.Transitions, delegate(FSMTransition transition)
        {
            return transition.TagetState == targetNode;
        });
        if (t == null)
        {
            var transition = ScriptableObject.CreateInstance<FSMTransition>();
            transition.hideFlags = HideFlags.HideInHierarchy;

            if (EditorUtility.IsPersistent(parent))
            {
                AssetDatabase.AddObjectToAsset(transition, parent);
            }

            transition.TagetState = targetNode;
            ArrayUtility.Add(ref parent.Transitions, transition);

            AssetDatabase.SaveAssets();
        }
    }
}
