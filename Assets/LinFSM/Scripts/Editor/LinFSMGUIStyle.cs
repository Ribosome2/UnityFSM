using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class LinFsmGUIStyle  {
   static  LinFsmGUIStyle()
    {
        LinFsmGUIStyle.nodeStyleCache = new Dictionary<string, GUIStyle>();

       nodeStyleCache = new Dictionary<string, GUIStyle>();
       gridMinorColor = EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.18f) : new Color(0f, 0f, 0f, 0.1f);
       gridMajorColor = EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.28f) : new Color(0f, 0f, 0f, 0.15f);

       popupIcon = EditorGUIUtility.FindTexture("_popup");
       helpIcon = EditorGUIUtility.FindTexture("_help");
       errorIcon = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
       warnIcon = EditorGUIUtility.FindTexture("console.warnicon");
       infoIcon = EditorGUIUtility.FindTexture("console.infoicon");
       toolbarPlus = EditorGUIUtility.FindTexture("Toolbar Plus");
       toolbarMinus = EditorGUIUtility.FindTexture("Toolbar Minus");

       canvasBackground = "flow background";
       selectionRect = "SelectionRect";
       elementBackground = new GUIStyle("PopupCurveSwatchBackground")
        {
            padding = new RectOffset()
        };
       breadcrumbLeft = "GUIEditor.BreadcrumbLeft";
       breadcrumbMiddle = "GUIEditor.BreadcrumbMid";
       wrappedLabel = new GUIStyle("label")
        {
            fixedHeight = 0,
            wordWrap = true
        };
       wrappedLabelLeft = new GUIStyle("label")
        {
            fixedHeight = 0,
            wordWrap = true,
            alignment = TextAnchor.UpperLeft
        };
       variableHeader = "flow overlay header lower left";
       label = "label";
       inspectorTitle = "IN Title";
       inspectorTitleText = "IN TitleText";
       iCodeLogo = (Texture2D)Resources.Load("ICodeLogo");
       stateLabelGizmo = new GUIStyle("HelpBox")
        {
            alignment = TextAnchor.UpperCenter,
            fontSize = 21
        };
        centeredLabel = new GUIStyle("Label"){alignment = TextAnchor.UpperCenter,};
        instructionLabel = new GUIStyle("TL Selection H2"){
            padding = new RectOffset(3, 3, 3, 3),
            contentOffset = wrappedLabel.contentOffset,
            alignment = TextAnchor.UpperLeft,
            fixedHeight = 0,
            wordWrap = true
        };
       shortcutLabel = new GUIStyle("ObjectPickerLargeStatus")
        {
            padding = new RectOffset(3, 3, 3, 3),
            alignment = TextAnchor.UpperLeft
        };
        browserPopup = new GUIStyle("label")
        {
            contentOffset = new Vector2(0, 2)
        };
    }

   public static GUIStyle canvasBackground;
   public static GUIStyle selectionRect;
   public static GUIStyle elementBackground;
   public static GUIStyle breadcrumbLeft;
   public static GUIStyle breadcrumbMiddle;
   public static GUIStyle wrappedLabel;
   public static GUIStyle wrappedLabelLeft;
   public static GUIStyle variableHeader;
   public static GUIStyle label;
   public static GUIStyle centeredLabel;
   public static GUIStyle inspectorTitle;
   public static GUIStyle inspectorTitleText;
   public static GUIStyle stateLabelGizmo;
   public static GUIStyle instructionLabel;
   public static GUIStyle shortcutLabel;
   public static GUIStyle browserPopup;

   public static Texture2D popupIcon;
   public static Texture2D helpIcon;
   public static Texture2D errorIcon;
   public static Texture2D warnIcon;
   public static Texture2D infoIcon;
   public static Texture2D toolbarPlus;
   public static Texture2D toolbarMinus;
   public static Texture2D iCodeLogo;

   public static Color gridMinorColor;
   public static Color gridMajorColor;

   public static int fsmColor;
   public static int startNodeColor;
   public static int anyStateColor;
   public static int defaultNodeColor;

    private static string[] styleCache =
		{
			"flow node 0",
			"flow node 1",
			"flow node 2",
			"flow node 3",
			"flow node 4",
			"flow node 5",
			"flow node 6"
		};

    private static string[] styleCacheHex =
		{
			"flow node hex 0",
			"flow node hex 1",
			"flow node hex 2",
			"flow node hex 3",
			"flow node hex 4",
			"flow node hex 5",
			"flow node hex 6"
		};

    private static Dictionary<string, GUIStyle> nodeStyleCache;

    public static GUIStyle GetNodeStyle(int color, bool on, bool hex)
    {
        return GetNodeStyle(hex ? styleCacheHex[color] : styleCache[color], on, hex ? 8f : 2f);
    }

    private static GUIStyle GetNodeStyle(string styleName, bool on, float offset)
    {
        string str = on ? string.Concat(styleName, " on") : styleName;
        if (!nodeStyleCache.ContainsKey(str))
        {
            GUIStyle style = new GUIStyle(str);
            style.contentOffset = new Vector2(0, style.contentOffset.y - offset);
            if (on)
            {
                style.fontStyle = FontStyle.Bold;
            }
            nodeStyleCache[str] = style;
        }
        return nodeStyleCache[str];
    }
}
