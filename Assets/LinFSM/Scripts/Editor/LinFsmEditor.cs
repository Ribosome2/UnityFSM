using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public enum SelectFlag
{
    None=0,
    SingleNode=1,
    TransitionLink=2,

}
public class LinFsmEditor : EditorWindow


{
    [MenuItem("LinFSM/CreateFSM")]
    public static void CreateStateMachine()
    {
      
        string path = EditorUtility.SaveFilePanelInProject("NewFSM", "NewLinFSM", "asset", "ChooseSavePath");
        if (string.IsNullOrEmpty(path) == false)
        {
            var fsmAsset = ScriptableObject.CreateInstance<LinStateMachine>();
            AssetDatabase.CreateAsset(fsmAsset, path);
            NodeUiUtil.AddNode<AnyState>(fsmAsset);
        }
    }

    Color gridMinorColor = EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.18f) : new Color(0f, 0f, 0f, 0.1f);
    Color gridMajorColor = EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.28f) : new Color(0f, 0f, 0f, 0.15f);
    public const float MaxCanvasSize = 50000f;
    private const float GridMinorSize = 16f;
    private const float GridMajorSize = 160f;
    [SerializeField]
    private Rect worldViewRect;
    private Vector2 offset;
    private Event currentEvent;
    [SerializeField]
    protected Vector2 scrollPosition;

    [SerializeField]
    private List<FSMNode> selection = new List<FSMNode>();
    public static LinFsmEditor Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GetWindow<LinFsmEditor>();
            }
            return mInstance;
        }
    }

    private LinStateMachine targetStateMachine;

    private Vector2 mousePosition;
    private FSMNode startNode;
    public SelectFlag SelectFlag;
    public FSMNode selectNode;
    private FSMNode mouseOverNode;
    private FSMTransition mouseOverTransition;
    private FSMTransition selectTransition;

    FSMNode[]  Nodes
    {
        get
        {
            if (targetStateMachine == null)
            {
                return null;
            }
            return targetStateMachine.Nodes;
        }
    }


    public void SelectFSM(LinStateMachine machine)
    {
        targetStateMachine = machine;
    }

    public static LinFsmEditor mInstance ;

    public void OnGUI()
    {
        currentEvent = Event.current;
        mousePosition = Event.current.mousePosition;
        DragCanvas();
        DrawGrid();
        if (targetStateMachine != null)
        {
            mouseOverNode = MouseOverNode();
            if (mouseOverNode)
            {
                mouseOverTransition = null;
            }
            else
            {
                mouseOverTransition = MouseOverTransition();
            }
            DrawNodes(Nodes);
        }
    }

    private void DrawGrid()
    {
        
       
        Rect canvasSize= new Rect(0,0,position.width,position.height);
        if (Event.current.type == EventType.Repaint)
        {
            LinFsmGUIStyle.canvasBackground.Draw(canvasSize, false, false, false, false);
            GL.PushMatrix();
            GL.Begin(1);
            this.DrawGridLines(canvasSize, GridMinorSize, offset,gridMinorColor);
            this.DrawGridLines(canvasSize, GridMajorSize, offset, gridMajorColor);
            GL.End();
            GL.PopMatrix();
        }
    }

    private void DragCanvas()
    {
        if (currentEvent.button != 2)
        {
            return;
        }

        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (currentEvent.rawType)
        {
            case EventType.mouseDown:
                GUIUtility.hotControl = controlID;
                currentEvent.Use();
                break;
            case EventType.mouseUp:
                if (GUIUtility.hotControl == controlID)
                {
                    GUIUtility.hotControl = 0;
                    currentEvent.Use();
                }
                break;
            case EventType.mouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    UpdateScrollPosition(scrollPosition - currentEvent.delta);
                    currentEvent.Use();
                }
                break;
        }
    }

    private void DrawGridLines(Rect rect, float gridSize, Vector2 offset, Color gridColor)
    {
        GL.Color(gridColor);
        for (float i = rect.x + (offset.x < 0f ? gridSize : 0f) + offset.x % gridSize;i < rect.x + rect.width;i = i + gridSize)
        {
            this.DrawLine(new Vector2(i, rect.y), new Vector2(i, rect.y + rect.height));
        }
        for (float j = rect.y + (offset.y < 0f ? gridSize : 0f) + offset.y % gridSize;j < rect.y + rect.height;j = j + gridSize)
        {
            this.DrawLine(new Vector2(rect.x, j), new Vector2(rect.x + rect.width, j));
        }
    }

    private void DrawLine(Vector2 p1, Vector2 p2)
    {
        GL.Vertex(p1);
        GL.Vertex(p2);
    }



    protected void UpdateScrollPosition(Vector2 position)
    {
        offset = offset + (scrollPosition - position);
        scrollPosition = position;

    }


    private void DrawNodes(FSMNode[] Nodes )
    {
        
        DrawTransitions();
        for (int i = 0; i < Nodes.Length; i++)
        {
            FSMNode node =Nodes[i];
            if (!selection.Contains(node))
            {
                DrawSingleNode(node);
            }
        }

        for (int i = 0; i < selection.Count; i++)
        {
            FSMNode node = selection[i];
            DrawSingleNode(node);
        }
        HandleNodeEvents();
        NodeContextMenu();
    }

    private void HandleNodeEvents()
    {
        if (currentEvent.type == EventType.MouseDown)
        {

            if (mouseOverNode != null && currentEvent.button == 0)
            {
                selectNode = mouseOverNode;
                //!!!!Set current selected note set as Unity's selection,so that Unity's Inspector will show our selection
                //The beauty of this The Inpector can Even inpector sth that is not a single asset lying in Project folder,
                //
                Selection.objects = new Object[] {selectNode};
                Repaint();
            }

            if (mouseOverTransition)
            {
                selectTransition = mouseOverTransition;
                Selection.objects = new Object[] { selectTransition };
            }
            else
            {
                selectTransition = null;
                Repaint();
            }

            if (startNode)
            {
                if (mouseOverNode)
                {
                    NodeUiUtil.CreateTransition(startNode,mouseOverNode);
                }
                startNode = null;
            }
        }else if (currentEvent.type == EventType.mouseDrag)
        {
            if (selectNode)
            {
                selectNode.position += currentEvent.delta;
                Repaint();
            }
        }else if (currentEvent.type == EventType.mouseUp)
        {
            
        }else if (currentEvent.type == EventType.mouseMove)
        {
            if (startNode)
            {
                Repaint();
            }
        }
    }

    private void DrawSingleNode(FSMNode node)
    {
        GUIStyle style = LinFsmGUIStyle.GetNodeStyle(node.color, selection.Contains(node),
            node.GetType() == typeof(LinStateMachine));
        Color tempColor = GUI.color;
        GUI.color = node == selectNode ? Color.yellow : tempColor;
        GUI.Box(OffSetRect(node.PosRect), node.Name, style);
        GUI.color = tempColor;
    }

    Rect OffSetRect(Rect oldRect)
    {
        return new Rect(oldRect.x+offset.x,oldRect.y+offset.y,oldRect.width,oldRect.height);
    }

    Vector2 OffSetPos(Vector2 pos)
    {
        return new Vector2(pos.x+offset.x,pos.y+offset.y);
    }

    private void DrawTransitions()
    {
        foreach (FSMNode fsmNode in Nodes)
        {
            DrawTransition(fsmNode);
        }

        if (startNode)
        {
            NodeUiUtil.DrawConnection(OffSetPos(startNode.position), mousePosition, Color.green, 1, false);
            Repaint();
        }
    }

    private void DrawTransition(FSMNode node)
    {
        for (int i=0;i<node.Transitions.Length;i++)
        {
            var transition = node.Transitions[i];
            Color color = transition == selectTransition ? Color.cyan : Color.white;
            NodeUiUtil.DrawConnection(OffSetPos(node.position), OffSetPos(transition.TagetState.position),color, 1, true);
        }
       
    }


    private void NodeContextMenu()
    {
        if (currentEvent.type != EventType.MouseDown || currentEvent.button != 1 || currentEvent.clickCount != 1)
        {
            return;
        }
        GenericMenu nodeMenu = new GenericMenu();
   
        if (mouseOverNode == null)
        {

            if (mouseOverTransition)
            {
                nodeMenu.AddItem(new GUIContent("Delete"), false, delegate()
                {
                    DeleteTransition(mouseOverTransition);
                    Repaint();
                });
            }
            else
            {
                nodeMenu.AddItem(new GUIContent("Create state"), false, delegate()
                {
                    var state = NodeUiUtil.AddNode<FSMState>(targetStateMachine);
                    state.position = GetNodesCenter();
                    Repaint();
                });
            }
        }
        else
        {
            nodeMenu.AddItem(FsmContent.makeTransition, false, delegate()
            {
                startNode = mouseOverNode;
            });


            if (!mouseOverNode.IsStartNode && !(mouseOverNode is AnyState))
            {
                nodeMenu.AddItem(FsmContent.setAsDefault, false, delegate()
                {
                    mouseOverNode.IsStartNode = true;
                });
            }
            else
            {
                nodeMenu.AddDisabledItem(FsmContent.setAsDefault);
            }

            if (mouseOverNode.GetType() == typeof(FSMState))
            {
                FSMState state = mouseOverNode as FSMState;
                nodeMenu.AddItem(FsmContent.sequence, state.IsSequence, delegate()
                {
                    state.IsSequence =
                        !state.IsSequence;
                });
            }

            if (mouseOverNode.GetType() != typeof(AnyState))
            {
                nodeMenu.AddItem(FsmContent.copy, false, delegate()
                {
                    //Pasteboard.Copy(node);
                });

                nodeMenu.AddItem(FsmContent.delete, false, delegate()
                {
                   
                    targetStateMachine.DeleteNode(mouseOverNode);
                
                    // EditorUtility.SetDirty(FsmEditor.Active);
                });
            }
            else
            {
                nodeMenu.AddDisabledItem(FsmContent.copy);
                nodeMenu.AddDisabledItem(FsmContent.delete);
            }
        }
       
       
        nodeMenu.ShowAsContext();
        Event.current.Use();
    }

    private FSMNode MouseOverNode()
    {
        for (int i = 0; i < Nodes.Length; i++)
        {
            FSMNode node = Nodes[i];
            if (OffSetRect(node.PosRect).Contains(mousePosition))
            {
                return node;
            }
        }
        return null;
    }

    private FSMTransition MouseOverTransition()
    {
        for (int i = 0; i < Nodes.Length; i++)
        {
            FSMNode node = Nodes[i];
            foreach (var transition in Nodes[i].Transitions)
            {
                Vector3 offsetStart, offsetEnd;
                NodeUiUtil.GetOffetTransitionPoints(node.PosRect.center, transition.TagetState.position,out offsetStart,out offsetEnd);
                if (NodeUiUtil.IsPosOnLine(OffSetPos(offsetStart), OffSetPos(offsetEnd),
                    mousePosition))
                {
                    return transition;
                }
            }
        }

        return null;
    }

    void DeleteTransition(FSMTransition transition)
    {
        foreach (var fsmNode in Nodes)
        {
            int index = ArrayUtility.FindIndex(fsmNode.Transitions, delegate(FSMTransition t)
            {
                return t == transition;
            });
            if (index != -1)
            {
                ArrayUtility.RemoveAt(ref fsmNode.Transitions,index);
                return;
            }

        }
    }

    Vector2 GetNodesCenter()
    {
        float x=0, y = 0;
        foreach (var node in Nodes)
        {
            x += node.position.x;
            y += node.position.y;
        }
        if (Nodes.Length > 0)
        {
            x = x/Nodes.Length;
            y = y/Nodes.Length;
        }
        return new Vector2(x,y);
    }
}
