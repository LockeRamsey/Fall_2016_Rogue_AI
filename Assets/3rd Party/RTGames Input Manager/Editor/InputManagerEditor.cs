using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class InputManagerEditor : EditorWindow
{
    int selectedMenu;
    int selectedPlayer;
    static string gameInput;
    bool showDelete = true;
    Vector2 scrollPosition = new Vector2();

    #region Player 1 Variables
    bool combineFoldout1 = false;
    public List<InputManagerCombines> combineInputs1 = new List<InputManagerCombines>();

    bool axisFoldout1 = false;
    public List<InputManagerAxes> axisInputs1 = new List<InputManagerAxes>();

    bool comboFoldout1 = false;
    public List<InputManagerCombos> comboInputs1 = new List<InputManagerCombos>();
    #endregion

    #region Player 2 Variables
    bool combineFoldout2 = false;
    public List<InputManagerCombines> combineInputs2 = new List<InputManagerCombines>();

    bool axisFoldout2 = false;
    public List<InputManagerAxes> axisInputs2 = new List<InputManagerAxes>();

    bool comboFoldout2 = false;
    public List<InputManagerCombos> comboInputs2 = new List<InputManagerCombos>();
    #endregion

    #region Player 3 Variables
    bool combineFoldout3 = false;
    public List<InputManagerCombines> combineInputs3 = new List<InputManagerCombines>();

    bool axisFoldout3 = false;
    public List<InputManagerAxes> axisInputs3 = new List<InputManagerAxes>();

    bool comboFoldout3 = false;
    public List<InputManagerCombos> comboInputs3 = new List<InputManagerCombos>();
    #endregion

    #region Player 4 Variables
    bool combineFoldout4 = false;
    public List<InputManagerCombines> combineInputs4 = new List<InputManagerCombines>();

    bool axisFoldout4 = false;
    public List<InputManagerAxes> axisInputs4 = new List<InputManagerAxes>();

    bool comboFoldout4 = false;
    public List<InputManagerCombos> comboInputs4 = new List<InputManagerCombos>();
    #endregion

    #region Settings Variables
    float MouseSensitivity = 0.05f;
    float ScrollWheelSensitivity = 0.1f;
    #endregion

    public List<AllKeysButtonsAndAxes> exclusionList = new List<AllKeysButtonsAndAxes>();

    Color defaultColor;

    [MenuItem("Window/RTGames/Input Manager")]
    public static void ShowWindow()
    {
        GetWindow(typeof(InputManagerEditor));
    }

    void OnEnable()
    {
        defaultColor = GUI.color;
        minSize = new Vector2(300, 200);
        ReadFromGameInput();
    }

    void OnGUI()
    {
        Undo.RecordObject(this, "RTGamesInputManager_ci1");

#if UNITY_5_0
        title = "Input Manager";
#else
        titleContent = new GUIContent("Input Manager");
#endif

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        selectedMenu = GUILayout.Toolbar(selectedMenu, new string[] { "Input Setup", "Exclusion List", "Settings" });
        GUILayout.EndHorizontal();

        if (selectedMenu == 0)
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Player: ");
            selectedPlayer = GUILayout.Toolbar(selectedPlayer, new string[] { "One", "Two", "Three", "Four" });
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            switch (selectedPlayer)
            {
                case 0:
                    PlayerOne();
                    break;
                case 1:
                    PlayerTwo();
                    break;
                case 2:
                    PlayerThree();
                    break;
                case 3:
                    PlayerFour();
                    break;
            }

            GUILayout.EndScrollView();
        }
        else if (selectedMenu == 1)
            ExclusionList();
        else
            Settings();

        EditorGUILayout.Space();
        GUILayout.FlexibleSpace();
        showDelete = EditorGUILayout.ToggleLeft("Show Delete Buttons", showDelete);
        GUI.color = new Color(1, 0.5f, 0.5f) * defaultColor;
        if (GUILayout.Button("Refresh From Input Manager"))
        {
            if (EditorUtility.DisplayDialog("Refresh Inputs?", "This will delete all changes since the last update to file.", "Yes", "Cancel"))
            {
                ReadFromGameInput();
            }
        }
        GUI.color = defaultColor;
        if (GUILayout.Button("Update Input Manager"))
            WriteToGameInput();
    }

    void PlayerOne()
    {
        #region Combine Inputs
        combineFoldout1 = EditorGUILayout.Foldout(combineFoldout1, "Combined Inputs");

        if (combineFoldout1)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerCombines ci in combineInputs1)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        combineInputs1.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');
                    for (int i = 0; i < ci.inputs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (showDelete)
                            if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                            {
                                ci.inputs.RemoveAt(i);
                                break;
                            }

                        if (GUILayout.Button(ci.inputs[i].ToString(), EditorStyles.layerMaskField, GUILayout.Height(16)))
                        {
                            InputSelectorEditor selector = new InputSelectorEditor();
                            selector.ValueToChange(ref ci.inputs, i);
                            PopupWindow.Show(new Rect((position.width - selector.GetWindowSize().x) / 2, 0, 0, 0), selector);
                        }

                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Add New Input Key"))
                        ci.inputs.Add(AllKeysButtonsAndAxes.A);

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Input"))
            {
                combineInputs1.Add(new InputManagerCombines(PlayerIndex.One));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Input Axes
        axisFoldout1 = EditorGUILayout.Foldout(axisFoldout1, "Input Axes");

        if (axisFoldout1)
        {
            List<string> combineNames = new List<string>();
            foreach (InputManagerCombines s in combineInputs1)
                combineNames.Add(s.Name);

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerAxes ci in axisInputs1)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        axisInputs1.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');
                    int pos = combineInputs1.FindIndex(x => x.Name == ci.positive.Name);
                    int neg = combineInputs1.FindIndex(x => x.Name == ci.negative.Name);
                    ci.positive = combineInputs1[EditorGUILayout.Popup("Positive", pos > -1 ? pos : 0, combineNames.ToArray())];
                    ci.negative = combineInputs1[EditorGUILayout.Popup("Negative", neg > -1 ? neg : 0, combineNames.ToArray())];

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Axis"))
            {
                axisInputs1.Add(new InputManagerAxes(PlayerIndex.One));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Key Combinations
        comboFoldout1 = EditorGUILayout.Foldout(comboFoldout1, "Input Combinations");

        if (comboFoldout1)
        {
            List<string> combineNames = new List<string>();
            foreach (InputManagerCombines s in combineInputs1)
                combineNames.Add(s.Name);

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerCombos ci in comboInputs1)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        comboInputs1.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');

                    for (int i = 0; i < ci.inputs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (showDelete)
                            if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                            {
                                ci.inputs.RemoveAt(i);
                                break;
                            }
                        int pos = combineInputs1.FindIndex(x => x.Name == ci.inputs[i].Name);
                        ci.inputs[i] = combineInputs1[EditorGUILayout.Popup("Required Key " + (i + 1), pos > -1 ? pos : 0, combineNames.ToArray())];
                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Add Input To Combination"))
                        ci.inputs.Add(combineInputs1[0]);

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Combination"))
            {
                comboInputs1.Add(new InputManagerCombos(PlayerIndex.One));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion
    }

    void PlayerTwo()
    {
        if (GUILayout.Button("Copy values from Player One."))
            CopyToTwo();

        #region Combine Inputs
        combineFoldout2 = EditorGUILayout.Foldout(combineFoldout2, "Combined Inputs");

        if (combineFoldout2)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerCombines ci in combineInputs2)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        combineInputs2.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');
                    for (int i = 0; i < ci.inputs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (showDelete)
                            if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                            {
                                ci.inputs.RemoveAt(i);
                                break;
                            }

                        if (GUILayout.Button(ci.inputs[i].ToString(), EditorStyles.layerMaskField, GUILayout.Height(16)))
                        {
                            InputSelectorEditor selector = new InputSelectorEditor();
                            selector.ValueToChange(ref ci.inputs, i);
                            PopupWindow.Show(new Rect((position.width - selector.GetWindowSize().x) / 2, 0, 0, 0), selector);
                        }

                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Add New Input Key"))
                        ci.inputs.Add(AllKeysButtonsAndAxes.A);

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Input"))
            {
                combineInputs2.Add(new InputManagerCombines(PlayerIndex.Two));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Input Axes
        axisFoldout2 = EditorGUILayout.Foldout(axisFoldout2, "Input Axes");

        if (axisFoldout2)
        {
            List<string> combineNames = new List<string>();
            foreach (InputManagerCombines s in combineInputs2)
                combineNames.Add(s.Name);

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerAxes ci in axisInputs2)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        axisInputs2.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');
                    int pos = combineInputs2.FindIndex(x => x.Name == ci.positive.Name);
                    int neg = combineInputs2.FindIndex(x => x.Name == ci.negative.Name);
                    ci.positive = combineInputs2[EditorGUILayout.Popup("Positive", pos > -1 ? pos : 0, combineNames.ToArray())];
                    ci.negative = combineInputs2[EditorGUILayout.Popup("Negative", neg > -1 ? neg : 0, combineNames.ToArray())];

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Axis"))
            {
                axisInputs2.Add(new InputManagerAxes(PlayerIndex.Two));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Key Combinations
        comboFoldout2 = EditorGUILayout.Foldout(comboFoldout2, "Input Combinations");

        if (comboFoldout2)
        {
            List<string> combineNames = new List<string>();
            foreach (InputManagerCombines s in combineInputs2)
                combineNames.Add(s.Name);

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerCombos ci in comboInputs2)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        comboInputs2.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');

                    for (int i = 0; i < ci.inputs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (showDelete)
                            if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                            {
                                ci.inputs.RemoveAt(i);
                                break;
                            }
                        int pos = combineInputs2.FindIndex(x => x.Name == ci.inputs[i].Name);
                        ci.inputs[i] = combineInputs2[EditorGUILayout.Popup("Required Key " + (i + 1), pos > -1 ? pos : 0, combineNames.ToArray())];
                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Add Input To Combination"))
                        ci.inputs.Add(combineInputs2[0]);

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Combination"))
            {
                comboInputs2.Add(new InputManagerCombos(PlayerIndex.Two));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion
    }

    void PlayerThree()
    {
        if (GUILayout.Button("Copy values from Player One."))
            CopyToThree();

        #region Combine Inputs
        combineFoldout3 = EditorGUILayout.Foldout(combineFoldout3, "Combined Inputs");

        if (combineFoldout3)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerCombines ci in combineInputs3)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        combineInputs3.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');
                    for (int i = 0; i < ci.inputs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (showDelete)
                            if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                            {
                                ci.inputs.RemoveAt(i);
                                break;
                            }

                        if (GUILayout.Button(ci.inputs[i].ToString(), EditorStyles.layerMaskField, GUILayout.Height(16)))
                        {
                            InputSelectorEditor selector = new InputSelectorEditor();
                            selector.ValueToChange(ref ci.inputs, i);
                            PopupWindow.Show(new Rect((position.width - selector.GetWindowSize().x) / 2, 0, 0, 0), selector);
                        }

                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Add New Input Key"))
                        ci.inputs.Add(AllKeysButtonsAndAxes.A);

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Input"))
            {
                combineInputs3.Add(new InputManagerCombines(PlayerIndex.Three));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Input Axes
        axisFoldout3 = EditorGUILayout.Foldout(axisFoldout3, "Input Axes");

        if (axisFoldout3)
        {
            List<string> combineNames = new List<string>();
            foreach (InputManagerCombines s in combineInputs3)
                combineNames.Add(s.Name);

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerAxes ci in axisInputs3)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        axisInputs3.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');
                    int pos = combineInputs3.FindIndex(x => x.Name == ci.positive.Name);
                    int neg = combineInputs3.FindIndex(x => x.Name == ci.negative.Name);
                    ci.positive = combineInputs3[EditorGUILayout.Popup("Positive", pos > -1 ? pos : 0, combineNames.ToArray())];
                    ci.negative = combineInputs3[EditorGUILayout.Popup("Negative", neg > -1 ? neg : 0, combineNames.ToArray())];

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Axis"))
            {
                axisInputs3.Add(new InputManagerAxes(PlayerIndex.Three));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Key Combinations
        comboFoldout3 = EditorGUILayout.Foldout(comboFoldout3, "Input Combinations");

        if (comboFoldout3)
        {
            List<string> combineNames = new List<string>();
            foreach (InputManagerCombines s in combineInputs3)
                combineNames.Add(s.Name);

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerCombos ci in comboInputs3)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        comboInputs3.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');

                    for (int i = 0; i < ci.inputs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (showDelete)
                            if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                            {
                                ci.inputs.RemoveAt(i);
                                break;
                            }
                        int pos = combineInputs3.FindIndex(x => x.Name == ci.inputs[i].Name);
                        ci.inputs[i] = combineInputs3[EditorGUILayout.Popup("Required Key " + (i + 1), pos > -1 ? pos : 0, combineNames.ToArray())];
                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Add Input To Combination"))
                        ci.inputs.Add(combineInputs3[0]);

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Combination"))
            {
                comboInputs3.Add(new InputManagerCombos(PlayerIndex.Three));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion
    }

    void PlayerFour()
    {
        if (GUILayout.Button("Copy values from Player One."))
            CopyToFour();

        #region Combine Inputs
        combineFoldout4 = EditorGUILayout.Foldout(combineFoldout4, "Combined Inputs");

        if (combineFoldout4)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerCombines ci in combineInputs4)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        combineInputs4.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');
                    for (int i = 0; i < ci.inputs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (showDelete)
                            if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                            {
                                ci.inputs.RemoveAt(i);
                                break;
                            }

                        if (GUILayout.Button(ci.inputs[i].ToString(), EditorStyles.layerMaskField, GUILayout.Height(16)))
                        {
                            InputSelectorEditor selector = new InputSelectorEditor();
                            selector.ValueToChange(ref ci.inputs, i);
                            PopupWindow.Show(new Rect((position.width - selector.GetWindowSize().x) / 2, 0, 0, 0), selector);
                        }

                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Add New Input Key"))
                        ci.inputs.Add(AllKeysButtonsAndAxes.A);

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Input"))
            {
                combineInputs4.Add(new InputManagerCombines(PlayerIndex.Four));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Input Axes
        axisFoldout4 = EditorGUILayout.Foldout(axisFoldout4, "Input Axes");

        if (axisFoldout4)
        {
            List<string> combineNames = new List<string>();
            foreach (InputManagerCombines s in combineInputs4)
                combineNames.Add(s.Name);

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerAxes ci in axisInputs4)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        axisInputs4.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');
                    int pos = combineInputs4.FindIndex(x => x.Name == ci.positive.Name);
                    int neg = combineInputs4.FindIndex(x => x.Name == ci.negative.Name);
                    ci.positive = combineInputs4[EditorGUILayout.Popup("Positive", pos > -1 ? pos : 0, combineNames.ToArray())];
                    ci.negative = combineInputs4[EditorGUILayout.Popup("Negative", neg > -1 ? neg : 0, combineNames.ToArray())];

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Axis"))
            {
                axisInputs4.Add(new InputManagerAxes(PlayerIndex.Four));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Key Combinations
        comboFoldout4 = EditorGUILayout.Foldout(comboFoldout4, "Input Combinations");

        if (comboFoldout4)
        {
            List<string> combineNames = new List<string>();
            foreach (InputManagerCombines s in combineInputs4)
                combineNames.Add(s.Name);

            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.BeginVertical();
            foreach (InputManagerCombos ci in comboInputs4)
            {
                GUILayout.BeginHorizontal();
                if (showDelete)
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        comboInputs4.Remove(ci);
                        break;
                    }
                ci.show = EditorGUILayout.Foldout(ci.show, ci.Name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (ci.show)
                {
                    GUILayout.BeginHorizontal();
                    if (showDelete)
                        GUILayout.Space(25);
                    GUILayout.BeginVertical(EditorStyles.textField);

                    ci.Name = EditorGUILayout.TextField("Name", ci.Name);
                    ci.Name = ci.Name.Replace(' ', '_');

                    for (int i = 0; i < ci.inputs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (showDelete)
                            if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                            {
                                ci.inputs.RemoveAt(i);
                                break;
                            }
                        int pos = combineInputs4.FindIndex(x => x.Name == ci.inputs[i].Name);
                        ci.inputs[i] = combineInputs4[EditorGUILayout.Popup("Required Key " + (i + 1), pos > -1 ? pos : 0, combineNames.ToArray())];
                        GUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Add Input To Combination"))
                        ci.inputs.Add(combineInputs4[0]);

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add New Combination"))
            {
                comboInputs4.Add(new InputManagerCombos(PlayerIndex.Four));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion
    }

    void CopyToTwo()
    {
        combineInputs2.Clear();
        axisInputs2.Clear();
        comboInputs2.Clear();
        foreach (InputManagerCombines ci in combineInputs1)
        {
            InputManagerCombines value = new InputManagerCombines(PlayerIndex.Two);
            value.Name = ci.Name;
            value.inputs = new List<AllKeysButtonsAndAxes>(ci.inputs);
            combineInputs2.Add(value);
        }
        foreach (InputManagerAxes ai in axisInputs1)
        {
            InputManagerAxes value = new InputManagerAxes(PlayerIndex.Two);
            value.Name = ai.Name;
            value.positive = combineInputs2.Find(x => x.Name == ai.positive.Name);
            value.negative = combineInputs2.Find(x => x.Name == ai.negative.Name);
            axisInputs2.Add(value);
        }
        foreach (InputManagerCombos ci in comboInputs1)
        {
            InputManagerCombos value = new InputManagerCombos(PlayerIndex.Two);
            value.Name = ci.Name;
            value.inputs = combineInputs2.FindAll(x => ci.inputs.Exists(y => y.Name == x.Name));
            comboInputs2.Add(value);
        }
    }

    void CopyToThree()
    {
        combineInputs3.Clear();
        axisInputs3.Clear();
        comboInputs3.Clear();
        foreach (InputManagerCombines ci in combineInputs1)
        {
            InputManagerCombines value = new InputManagerCombines(PlayerIndex.Three);
            value.Name = ci.Name;
            value.inputs = new List<AllKeysButtonsAndAxes>(ci.inputs);
            combineInputs3.Add(value);
        }
        foreach (InputManagerAxes ai in axisInputs1)
        {
            InputManagerAxes value = new InputManagerAxes(PlayerIndex.Three);
            value.Name = ai.Name;
            value.positive = combineInputs3.Find(x => x.Name == ai.positive.Name);
            value.negative = combineInputs3.Find(x => x.Name == ai.negative.Name);
            axisInputs3.Add(value);
        }
        foreach (InputManagerCombos ci in comboInputs1)
        {
            InputManagerCombos value = new InputManagerCombos(PlayerIndex.Three);
            value.Name = ci.Name;
            value.inputs = combineInputs3.FindAll(x => ci.inputs.Exists(y => y.Name == x.Name));
            comboInputs3.Add(value);
        }
    }

    void CopyToFour()
    {
        combineInputs4.Clear();
        axisInputs4.Clear();
        comboInputs4.Clear();
        foreach (InputManagerCombines ci in combineInputs1)
        {
            InputManagerCombines value = new InputManagerCombines(PlayerIndex.Four);
            value.Name = ci.Name;
            value.inputs = new List<AllKeysButtonsAndAxes>(ci.inputs);
            combineInputs4.Add(value);
        }
        foreach (InputManagerAxes ai in axisInputs1)
        {
            InputManagerAxes value = new InputManagerAxes(PlayerIndex.Four);
            value.Name = ai.Name;
            value.positive = combineInputs4.Find(x => x.Name == ai.positive.Name);
            value.negative = combineInputs4.Find(x => x.Name == ai.negative.Name);
            axisInputs4.Add(value);
        }
        foreach (InputManagerCombos ci in comboInputs1)
        {
            InputManagerCombos value = new InputManagerCombos(PlayerIndex.Four);
            value.Name = ci.Name;
            value.inputs = combineInputs4.FindAll(x => ci.inputs.Exists(y => y.Name == x.Name));
            comboInputs4.Add(value);
        }
    }

    void ExclusionList()
    {
        if (GUILayout.Button("Add New Exclusion"))
            exclusionList.Add(AllKeysButtonsAndAxes.A);

        for (int i = 0; i < exclusionList.Count; i++)
        {
            GUILayout.BeginHorizontal();

            if (showDelete)
                if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(16)))
                {
                    exclusionList.Remove(exclusionList[i]);
                    break;
                }

            if (GUILayout.Button(exclusionList[i].ToString(), EditorStyles.layerMaskField, GUILayout.Height(16)))
            {
                InputSelectorEditor selector = new InputSelectorEditor();
                selector.ValueToChange(ref exclusionList, i);
                PopupWindow.Show(new Rect((position.width - selector.GetWindowSize().x) / 2, 46, 0, 0), selector);
            }

            GUILayout.EndHorizontal();
        }
    }

    void Settings()
    {
        GUILayout.BeginVertical("Sensitivity", GUI.skin.box);
        GUILayout.Space(20);
        MouseSensitivity = EditorGUILayout.FloatField("Mouse", MouseSensitivity);
        MouseSensitivity = Mathf.Clamp(MouseSensitivity, 0, 5);

        ScrollWheelSensitivity = EditorGUILayout.FloatField("Scroll Wheel", ScrollWheelSensitivity);
        ScrollWheelSensitivity = Mathf.Clamp(ScrollWheelSensitivity, 0, 5);
        GUILayout.EndVertical();
    }

    void ReadFromGameInput()
    {
        combineInputs1.Clear();
        axisInputs1.Clear();
        comboInputs1.Clear();

        combineInputs2.Clear();
        axisInputs2.Clear();
        comboInputs2.Clear();

        combineInputs3.Clear();
        axisInputs3.Clear();
        comboInputs3.Clear();

        combineInputs4.Clear();
        axisInputs4.Clear();
        comboInputs4.Clear();

        exclusionList.Clear();

        string[] assets = AssetDatabase.FindAssets("GameInput");
        if (assets.Length > 0)
            gameInput = AssetDatabase.GUIDToAssetPath(assets[0]).Split(new string[] { "Assets" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
        else
        {
            Debug.LogWarning("Could not find GameInput.cs asset. Create a new one at a desired location for it to be overwritten.");
            return;
        }

        StreamReader sr = new StreamReader(Application.dataPath + gameInput);

        do
        {
            string line = sr.ReadLine();

            if (line.Contains("static CombineInputsArray"))
            {
                CombineInputsArrayParse(line);
            }
            else if (line.Contains("static InputAxisArray"))
            {
                InputAxesArrayParse(line);
            }
            else if (line.Contains("static KeyCombinationArray"))
            {
                KeyCombinationArrayParse(line);
            }
            else if (line.Contains("List<AllKeysButtonsAndAxes> ExclusionList"))
            {
                line = line.Substring(line.IndexOf('{') + 1, line.IndexOf('}') - line.IndexOf('{') - 1);
                string[] keys = line.Split(new string[] { ", ", "AllKeysButtonsAndAxes.", " " }, System.StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < keys.Length; i++)
                    exclusionList.Add((AllKeysButtonsAndAxes)Enum.Parse(typeof(AllKeysButtonsAndAxes), keys[i], true));
            }
            else if (line.Contains("public static float MouseSensitivity"))
            {
                line = line.Substring(line.LastIndexOf(' ') + 1, line.LastIndexOf('f') - line.LastIndexOf(' ') - 1);
                MouseSensitivity = float.Parse(line);
            }
            else if (line.Contains("public static float ScrollWheelSensitivity"))
            {
                line = line.Substring(line.LastIndexOf(' ') + 1, line.LastIndexOf('f') - line.LastIndexOf(' ') - 1);
                ScrollWheelSensitivity = float.Parse(line);
            }
        }
        while (!sr.EndOfStream);

        sr.Close();
    }

    void CombineInputsArrayParse(string line)
    {
        line = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - line.IndexOf("(") - 1);
        string[] CombineInputs = line.Split(new string[] { "new CombineInputs(", "), ", ")", "null, ", "null" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string ci in CombineInputs)
        {
            string[] keys = ci.Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries);
            InputManagerCombines value = new InputManagerCombines((PlayerIndex)Enum.Parse(typeof(PlayerIndex), keys[0].Split('.')[1], true));
            value.Name = keys[1].Replace("\"", "");
            for (int i = 2; i < keys.Length; i++)
                value.inputs.Add((AllKeysButtonsAndAxes)Enum.Parse(typeof(AllKeysButtonsAndAxes), keys[i].Split('.')[1], true));
            switch (value.PlayerIndex)
            {
                case PlayerIndex.One:
                    combineInputs1.Add(value);
                    break;
                case PlayerIndex.Two:
                    combineInputs2.Add(value);
                    break;
                case PlayerIndex.Three:
                    combineInputs3.Add(value);
                    break;
                case PlayerIndex.Four:
                    combineInputs4.Add(value);
                    break;
            }
        }
    }

    void InputAxesArrayParse(string line)
    {
        string Name = line.Replace("public static InputAxisArray ", "");
        Name = Name.Remove(Name.IndexOf(" = "));
        line = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - line.IndexOf("(") - 1);
        string[] InputAxis = line.Split(new string[] { "new InputAxis(", "), ", ")", "null, ", "null" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string ia in InputAxis)
        {
            string[] keys = ia.Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries);
            InputManagerAxes value = new InputManagerAxes((PlayerIndex)Enum.Parse(typeof(PlayerIndex), keys[0].Split('.')[1], true));
            value.Name = Name;
            string pos, neg;
            switch (value.PlayerIndex)
            {
                case PlayerIndex.One:
                    pos = keys[1].Remove(keys[1].IndexOf("["));
                    neg = keys[2].Remove(keys[2].IndexOf("["));
                    value.positive = combineInputs1.Find(x => x.Name == pos);
                    value.negative = combineInputs1.Find(x => x.Name == neg);
                    axisInputs1.Add(value);
                    break;
                case PlayerIndex.Two:
                    pos = keys[1].Remove(keys[1].IndexOf("["));
                    neg = keys[2].Remove(keys[2].IndexOf("["));
                    value.positive = combineInputs2.Find(x => x.Name == pos);
                    value.negative = combineInputs2.Find(x => x.Name == neg);
                    axisInputs2.Add(value);
                    break;
                case PlayerIndex.Three:
                    pos = keys[1].Remove(keys[1].IndexOf("["));
                    neg = keys[2].Remove(keys[2].IndexOf("["));
                    value.positive = combineInputs3.Find(x => x.Name == pos);
                    value.negative = combineInputs3.Find(x => x.Name == neg);
                    axisInputs3.Add(value);
                    break;
                case PlayerIndex.Four:
                    pos = keys[1].Remove(keys[1].IndexOf("["));
                    neg = keys[2].Remove(keys[2].IndexOf("["));
                    value.positive = combineInputs4.Find(x => x.Name == pos);
                    value.negative = combineInputs4.Find(x => x.Name == neg);
                    axisInputs4.Add(value);
                    break;
            }
        }
    }

    void KeyCombinationArrayParse(string line)
    {
        string Name = line.Replace("public static KeyCombinationArray ", "");
        Name = Name.Remove(Name.IndexOf(" = "));
        line = line.Substring(line.IndexOf("(") + 1, line.LastIndexOf(")") - line.IndexOf("(") - 1);
        string[] KeyCombo = line.Split(new string[] { "new KeyCombination(", "), ", ")", "null, ", "null" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string kc in KeyCombo)
        {
            string[] keys = kc.Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries);
            InputManagerCombos value = new InputManagerCombos((PlayerIndex)Enum.Parse(typeof(PlayerIndex), keys[0].Split('.')[1], true));
            value.Name = Name;

            switch (value.PlayerIndex)
            {
                case PlayerIndex.One:
                    for (int i = 1; i < keys.Length; i++)
                        value.inputs.Add(combineInputs1.Find(x => x.Name == keys[i].Remove(keys[i].IndexOf("["))));
                    comboInputs1.Add(value);
                    break;
                case PlayerIndex.Two:
                    for (int i = 1; i < keys.Length; i++)
                        value.inputs.Add(combineInputs2.Find(x => x.Name == keys[i].Remove(keys[i].IndexOf("["))));
                    comboInputs2.Add(value);
                    break;
                case PlayerIndex.Three:
                    for (int i = 1; i < keys.Length; i++)
                        value.inputs.Add(combineInputs3.Find(x => x.Name == keys[i].Remove(keys[i].IndexOf("["))));
                    comboInputs3.Add(value);
                    break;
                case PlayerIndex.Four:
                    for (int i = 1; i < keys.Length; i++)
                        value.inputs.Add(combineInputs4.Find(x => x.Name == keys[i].Remove(keys[i].IndexOf("["))));
                    comboInputs4.Add(value);
                    break;
            }
        }
    }

    void WriteToGameInput()
    {
        string[] assets = AssetDatabase.FindAssets("GameInput");
        if (assets.Length > 0)
            gameInput = AssetDatabase.GUIDToAssetPath(assets[0]).Split(new string[] { "Assets" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
        else
        {
            Debug.LogWarning("Could not find GameInput.cs asset. Create a new one at a desired location for it to be overwritten.");
            return;
        }

        StreamWriter sw = new StreamWriter(Application.dataPath + gameInput, false);

        sw.WriteLine("using UnityEngine;");
        sw.WriteLine("using System.Collections.Generic;");
        sw.WriteLine();
        sw.WriteLine("//Do not edit this file directly. To edit this file, use the visual editor window.");
        sw.WriteLine();
        sw.WriteLine("public static class GameInput {");
        sw.WriteLine("public static Vector2 MousePosition { get { return new Vector2(Input.mousePosition.x, Input.mousePosition.y); } }");
        sw.WriteLine("public static float MouseSensitivity = " + MouseSensitivity + "f;");
        sw.WriteLine("public static float ScrollWheelSensitivity = " + ScrollWheelSensitivity + "f;");
        sw.WriteLine();

        #region Exclusion List
        sw.Write("public static List<AllKeysButtonsAndAxes> ExclusionList = new List<AllKeysButtonsAndAxes>( new AllKeysButtonsAndAxes[] { ");
        for (int i = 0; i < exclusionList.Count; i++)
        {
            if (i == exclusionList.Count - 1)
                sw.Write("AllKeysButtonsAndAxes." + exclusionList[i].ToString());
            else
                sw.Write("AllKeysButtonsAndAxes." + exclusionList[i].ToString() + ", ");
        }
        sw.WriteLine(" } );");
        #endregion

        sw.WriteLine();

        List<InputManagerCombineArray> combineArrays = BuildCombineArrays();
        foreach (InputManagerCombineArray imca in combineArrays)
            sw.WriteLine(imca.ToString());

        List<InputManagerAxesArray> axesArrays = BuildAxesArrays();
        foreach (InputManagerAxesArray imaa in axesArrays)
            sw.WriteLine(imaa.ToString());

        List<InputManagerCombosArray> comboArrays = BuildComboArrays();
        foreach (InputManagerCombosArray imca in comboArrays)
            sw.WriteLine(imca.ToString());

        sw.Write("}");
        sw.Close();

        AssetDatabase.ImportAsset("Assets" + gameInput);
    }

    List<InputManagerCombineArray> BuildCombineArrays()
    {
        List<InputManagerCombineArray> AllCombines = new List<InputManagerCombineArray>();

        foreach (InputManagerCombines imc in combineInputs1)
        {
            InputManagerCombineArray imca = AllCombines.Find(x => x.Name == imc.Name);
            if (imca != null)
                imca.Inputs.Add(imc);
            else
            {
                imca = new InputManagerCombineArray(imc.Name);
                imca.Inputs.Add(imc);
                AllCombines.Add(imca);
            }
        }

        foreach (InputManagerCombines imc in combineInputs2)
        {
            InputManagerCombineArray imca = AllCombines.Find(x => x.Name == imc.Name);
            if (imca != null)
                imca.Inputs.Add(imc);
            else
            {
                imca = new InputManagerCombineArray(imc.Name);
                imca.Inputs.Add(imc);
                AllCombines.Add(imca);
            }
        }

        foreach (InputManagerCombines imc in combineInputs3)
        {
            InputManagerCombineArray imca = AllCombines.Find(x => x.Name == imc.Name);
            if (imca != null)
                imca.Inputs.Add(imc);
            else
            {
                imca = new InputManagerCombineArray(imc.Name);
                imca.Inputs.Add(imc);
                AllCombines.Add(imca);
            }
        }

        foreach (InputManagerCombines imc in combineInputs4)
        {
            InputManagerCombineArray imca = AllCombines.Find(x => x.Name == imc.Name);
            if (imca != null)
                imca.Inputs.Add(imc);
            else
            {
                imca = new InputManagerCombineArray(imc.Name);
                imca.Inputs.Add(imc);
                AllCombines.Add(imca);
            }
        }

        return AllCombines;
    }

    List<InputManagerAxesArray> BuildAxesArrays()
    {
        List<InputManagerAxesArray> AllAxes = new List<InputManagerAxesArray>();

        foreach (InputManagerAxes ai in axisInputs1)
        {
            InputManagerAxesArray imaa = AllAxes.Find(x => x.Name == ai.Name);
            if (imaa != null)
                imaa.Inputs.Add(ai);
            else
            {
                imaa = new InputManagerAxesArray(ai.Name);
                imaa.Inputs.Add(ai);
                AllAxes.Add(imaa);
            }
        }
        foreach (InputManagerAxes ai in axisInputs2)
        {
            InputManagerAxesArray imaa = AllAxes.Find(x => x.Name == ai.Name);
            if (imaa != null)
                imaa.Inputs.Add(ai);
            else
            {
                imaa = new InputManagerAxesArray(ai.Name);
                imaa.Inputs.Add(ai);
                AllAxes.Add(imaa);
            }
        }
        foreach (InputManagerAxes ai in axisInputs3)
        {
            InputManagerAxesArray imaa = AllAxes.Find(x => x.Name == ai.Name);
            if (imaa != null)
                imaa.Inputs.Add(ai);
            else
            {
                imaa = new InputManagerAxesArray(ai.Name);
                imaa.Inputs.Add(ai);
                AllAxes.Add(imaa);
            }
        }
        foreach (InputManagerAxes ai in axisInputs4)
        {
            InputManagerAxesArray imaa = AllAxes.Find(x => x.Name == ai.Name);
            if (imaa != null)
                imaa.Inputs.Add(ai);
            else
            {
                imaa = new InputManagerAxesArray(ai.Name);
                imaa.Inputs.Add(ai);
                AllAxes.Add(imaa);
            }
        }

        return AllAxes;
    }

    List<InputManagerCombosArray> BuildComboArrays()
    {
        List<InputManagerCombosArray> AllCombos = new List<InputManagerCombosArray>();

        foreach (InputManagerCombos ai in comboInputs1)
        {
            InputManagerCombosArray imca = AllCombos.Find(x => x.Name == ai.Name);
            if (imca != null)
                imca.Inputs.Add(ai);
            else
            {
                imca = new InputManagerCombosArray(ai.Name);
                imca.Inputs.Add(ai);
                AllCombos.Add(imca);
            }
        }
        foreach (InputManagerCombos ai in comboInputs2)
        {
            InputManagerCombosArray imca = AllCombos.Find(x => x.Name == ai.Name);
            if (imca != null)
                imca.Inputs.Add(ai);
            else
            {
                imca = new InputManagerCombosArray(ai.Name);
                imca.Inputs.Add(ai);
                AllCombos.Add(imca);
            }
        }
        foreach (InputManagerCombos ai in comboInputs3)
        {
            InputManagerCombosArray imca = AllCombos.Find(x => x.Name == ai.Name);
            if (imca != null)
                imca.Inputs.Add(ai);
            else
            {
                imca = new InputManagerCombosArray(ai.Name);
                imca.Inputs.Add(ai);
                AllCombos.Add(imca);
            }
        }
        foreach (InputManagerCombos ai in comboInputs4)
        {
            InputManagerCombosArray imca = AllCombos.Find(x => x.Name == ai.Name);
            if (imca != null)
                imca.Inputs.Add(ai);
            else
            {
                imca = new InputManagerCombosArray(ai.Name);
                imca.Inputs.Add(ai);
                AllCombos.Add(imca);
            }
        }

        return AllCombos;
    }
}

[Serializable]
public class InputManagerCombines
{
    [NonSerialized]
    public bool show;
    public string Name = "NewInput";
    public PlayerIndex PlayerIndex = PlayerIndex.One;
    public List<AllKeysButtonsAndAxes> inputs = new List<AllKeysButtonsAndAxes>();

    public InputManagerCombines() { }
    public InputManagerCombines(PlayerIndex index) { PlayerIndex = index; }

    public string GetConstructor()
    {
        string value = "new CombineInputs(PlayerIndex." + PlayerIndex.ToString() + ", \"" + Name + "\"";
        if (inputs.Count > 0)
            value += ", ";
        for (int i = 0; i < inputs.Count; i++)
        {
            if (i == inputs.Count - 1)
                value += "AllKeysButtonsAndAxes." + inputs[i].ToString();
            else
                value += "AllKeysButtonsAndAxes." + inputs[i].ToString() + ", ";
        }
        value += ")";
        if (PlayerIndex != global::PlayerIndex.Four)
            value += ", ";
            

        return value;
    }

    public override string ToString()
    {
        return Name;
    }
}

[Serializable]
public class InputManagerAxes
{
    [NonSerialized]
    public bool show;
    public string Name = "NewAxis";
    public PlayerIndex PlayerIndex = PlayerIndex.One;
    public InputManagerCombines positive, negative;

    public InputManagerAxes() { }
    public InputManagerAxes(PlayerIndex index) { PlayerIndex = index; }

    public string GetConstructor()
    {
        string value = "new InputAxis(PlayerIndex." + PlayerIndex.ToString() + ", " + positive.Name + "[PlayerIndex." + PlayerIndex.ToString() + "], " + negative.Name + "[PlayerIndex." + PlayerIndex.ToString() + "])";
        if (PlayerIndex != global::PlayerIndex.Four)
            value += ", ";
        return value;
    }
}

[Serializable]
public class InputManagerCombos
{
    [NonSerialized]
    public bool show;
    public string Name = "NewCombo";
    public PlayerIndex PlayerIndex = PlayerIndex.One;
    public List<InputManagerCombines> inputs = new List<InputManagerCombines>();

    public InputManagerCombos() { }
    public InputManagerCombos(PlayerIndex index) { PlayerIndex = index; }

    public string GetConstructor()
    {
        string value = "new KeyCombination(PlayerIndex." + PlayerIndex.ToString();
        if (inputs.Count > 0)
            value += ", ";
        for (int i = 0; i < inputs.Count; i++)
        {
            if (i == inputs.Count - 1)
                value += inputs[i].Name + "[PlayerIndex." + PlayerIndex.ToString() + "]";
            else
                value += inputs[i].Name + "[PlayerIndex." + PlayerIndex.ToString() + "], ";
        }
        value += ")";
        if (PlayerIndex != global::PlayerIndex.Four)
            value += ", ";

        return value;
    }
}

class InputManagerCombineArray
{
    public string Name = "";
    public List<InputManagerCombines> Inputs = new List<InputManagerCombines>();

    public InputManagerCombineArray(string name) { Name = name; }

    public override string ToString()
    {
        string value = "public static CombineInputsArray " + Name + " = new CombineInputsArray(";

        InputManagerCombines one = null, two = null, three = null, four = null;
        foreach (InputManagerCombines imc in Inputs)
        {
            switch (imc.PlayerIndex)
            {
                case PlayerIndex.One:
                    one = imc;
                    break;
                case PlayerIndex.Two:
                    two = imc;
                    break;
                case PlayerIndex.Three:
                    three = imc;
                    break;
                case PlayerIndex.Four:
                    four = imc;
                    break;
            }
        }

        if (one != null)
            value += one.GetConstructor();
        else
            value += "null, ";

        if (two != null)
            value += two.GetConstructor();
        else
            value += "null, ";

        if (three != null)
            value += three.GetConstructor();
        else
            value += "null, ";

        if (four != null)
            value += four.GetConstructor();
        else
            value += "null";

        value += ");";

        return value;
    }
}

class InputManagerAxesArray
{
    public string Name = "";
    public List<InputManagerAxes> Inputs = new List<InputManagerAxes>();

    public InputManagerAxesArray(string name) { Name = name; }

    public override string ToString()
    {
        string value = "public static InputAxisArray " + Name + " = new InputAxisArray(";

        InputManagerAxes one = null, two = null, three = null, four = null;
        foreach (InputManagerAxes imc in Inputs)
        {
            switch (imc.PlayerIndex)
            {
                case PlayerIndex.One:
                    one = imc;
                    break;
                case PlayerIndex.Two:
                    two = imc;
                    break;
                case PlayerIndex.Three:
                    three = imc;
                    break;
                case PlayerIndex.Four:
                    four = imc;
                    break;
            }
        }

        if (one != null)
            value += one.GetConstructor();
        else
            value += "null, ";

        if (two != null)
            value += two.GetConstructor();
        else
            value += "null, ";

        if (three != null)
            value += three.GetConstructor();
        else
            value += "null, ";

        if (four != null)
            value += four.GetConstructor();
        else
            value += "null";

        value += ");";

        return value;
    }
}

class InputManagerCombosArray
{
    public string Name = "";
    public List<InputManagerCombos> Inputs = new List<InputManagerCombos>();

    public InputManagerCombosArray(string name) { Name = name; }

    public override string ToString()
    {
        string value = "public static KeyCombinationArray " + Name + " = new KeyCombinationArray(";

        InputManagerCombos one = null, two = null, three = null, four = null;
        foreach (InputManagerCombos imc in Inputs)
        {
            switch (imc.PlayerIndex)
            {
                case PlayerIndex.One:
                    one = imc;
                    break;
                case PlayerIndex.Two:
                    two = imc;
                    break;
                case PlayerIndex.Three:
                    three = imc;
                    break;
                case PlayerIndex.Four:
                    four = imc;
                    break;
            }
        }

        if (one != null)
            value += one.GetConstructor();
        else
            value += "null, ";

        if (two != null)
            value += two.GetConstructor();
        else
            value += "null, ";

        if (three != null)
            value += three.GetConstructor();
        else
            value += "null, ";

        if (four != null)
            value += four.GetConstructor();
        else
            value += "null";

        value += ");";

        return value;
    }
}