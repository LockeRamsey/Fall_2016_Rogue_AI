using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class InputSelectorEditor : PopupWindowContent
{
    public AllKeysButtonsAndAxes Value;

    List<AllKeysButtonsAndAxes> array;
    int index;

    Vector2 scrollPos;
    string searchValue = "";

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        searchValue = EditorGUILayout.TextField(searchValue, GUI.skin.FindStyle("ToolbarSeachTextField"));
        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton"))) searchValue = "";
        GUILayout.EndHorizontal();
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
        foreach (AllKeysButtonsAndAxes k in Enum.GetValues(typeof(AllKeysButtonsAndAxes)))
        {
            if (searchValue == "" || k.ToString().ToLower().Contains(searchValue.ToLower()))
                if (k != array[index])
                {
                    if (GUILayout.Button(k.ToString(), GUI.skin.box, GUILayout.ExpandWidth(true)))
                    {
                        array[index] = k;
                        this.editorWindow.Close();
                    }
                }
                else
                {
                    Color temp = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(0f, 0.34f, 1.0f);
                    if (GUILayout.Button(k.ToString(), GUI.skin.box, GUILayout.ExpandWidth(true)))
                        this.editorWindow.Close();
                    GUI.backgroundColor = temp;
                }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    public void ValueToChange(ref List<AllKeysButtonsAndAxes> a, int i)
    {
        array = a;
        index = i;
    }
}