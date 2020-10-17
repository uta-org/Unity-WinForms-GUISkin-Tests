﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Unity.API;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;

public class SkinWorker : MonoBehaviour
{
    internal enum UIState
    {
        Normal,
        Hover,
        Active,
        Focused
    }

    public static SkinWorker Instance { get; private set; }

    // TODO: Method to create a new GUISkin instance
    public static GUISkin MySkin => Instance.skin;

    private static Dictionary<string, TextureWorker> Workers { get; } = new Dictionary<string, TextureWorker>();

    [SerializeField]
    private Unity.API.UnityWinForms winForms;

    [SerializeField]
    private GUISkin skin;

    private Control control = new Control();

    [MenuItem("Window/Get Builtin skin...")]
    public static void GetSkin()
    {
        string dir = Path.Combine(Application.dataPath, "Resources", "Saved Skins/");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        List<string> enums = new List<string>();
        Array values = Enum.GetValues(typeof(EditorSkin));
        foreach (EditorSkin es in values)
        {
            GUISkin skin = Instantiate(EditorGUIUtility.GetBuiltinSkin(es));

            string name = $"SceneSkin{es}.guiskin";
            enums.Add(name);
            name = "Assets/Resources/Saved Skins/" + name;

            AssetDatabase.CreateAsset(skin, name);
        }
        EditorUtility.DisplayDialog("API Message",
            $"GUI Skin saved in 'Saved Skins' folder all {values.Length} scripts with names: ({string.Join(", ", enums.ToArray())})!", "Ok");
    }

    private void Awake()
    {
        Instance = this;
        skin = Instantiate(skin);

        // Start modifying the skin

        // TODO: Add regions

        int buttonDisabledStyleIndex = (int)CustomGUILayout.CustomSyles.ButtonDisabled;
        int buttonEnabledStyleIndex = (int)CustomGUILayout.CustomSyles.ButtonEnabled;

        // Debug.Log($"Custom styles length = {skin.customStyles.Length} ({string.Join(", ", skin.customStyles.Select(style => style.name))})");

        var copy = new GUIStyle[skin.customStyles.Length];
        Array.Copy(skin.customStyles, copy, skin.customStyles.Length);

        int enumLength = Enum.GetNames(typeof(CustomGUILayout.CustomSyles)).Length;
        skin.customStyles = new GUIStyle[enumLength + skin.customStyles.Length];

        // Start Button disabled
        var buttonDisabledWorkerNormal = CreateWorker(CreateStyle(buttonDisabledStyleIndex, skin.button), 16, 16)
            .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
            .Fill(SystemColors.Control.ToUnityColor())
            .Apply();

        skin.customStyles[buttonDisabledStyleIndex].normal.background = buttonDisabledWorkerNormal.Texture;
        skin.customStyles[buttonDisabledStyleIndex].normal.textColor = control.ForeColor.ToUnityColor();

        var buttonDisabledWorkerHover = CreateWorker(GetName(CustomGUILayout.CustomSyles.ButtonDisabled, UIState.Hover), 16, 16)
            .SetBorders(SkinColors.BorderHoverColor, 1)
            .Fill(SkinColors.HoverColor)
            .Apply();

        skin.customStyles[buttonDisabledStyleIndex].hover.background = buttonDisabledWorkerHover.Texture;
        skin.customStyles[buttonDisabledStyleIndex].hover.textColor = control.ForeColor.ToUnityColor();

        // End Button disabled

        // Start Button enabled

        var buttonEnabledWorkerNormal = CreateWorker(CreateStyle(buttonEnabledStyleIndex, skin.button), 16, 16)
            .SetBorders(SkinColors.BorderHoverColor, 1)
            .Fill(SystemColors.Control.ToUnityColor())
            .Apply();

        skin.customStyles[buttonEnabledStyleIndex].normal.background = buttonEnabledWorkerNormal.Texture;
        skin.customStyles[buttonEnabledStyleIndex].normal.textColor = control.ForeColor.ToUnityColor();

        // End Button enabled

        // Start Common styles

        skin.customStyles[buttonDisabledStyleIndex].onNormal = skin.customStyles[buttonDisabledStyleIndex].normal;
        skin.customStyles[buttonDisabledStyleIndex].active = skin.customStyles[buttonDisabledStyleIndex].hover;

        // TODO: This is not working on UILayout
        skin.customStyles[buttonEnabledStyleIndex].hover = skin.customStyles[buttonDisabledStyleIndex].hover;

        skin.customStyles[buttonEnabledStyleIndex].onNormal = skin.customStyles[buttonEnabledStyleIndex].normal;
        skin.customStyles[buttonEnabledStyleIndex].active = skin.customStyles[buttonEnabledStyleIndex].normal;

        // End Common styles

        // Start Window

        var windowWorker = CreateWorker("WindowStyle", 16, 16)
            .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
            .Fill(SystemColors.Control.ToUnityColor())
            // .SmartFill(new RectInt(0, 0, 16, 4), Color.black)
            .Apply();

        skin.window.normal.background = windowWorker.Texture;
        skin.window.normal.textColor = control.ForeColor.ToUnityColor();

        skin.window.onNormal.background = null;

        // TODO: Fix this
        //skin.window.border = new RectOffset();
        //skin.window.padding = new RectOffset();

        // End Window

        // Start Box

        var boxWorker = CreateWorker("BoxNormal", 16, 16)
            .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
            .Fill(UnityEngine.Color.clear)
            .Apply();

        skin.box.normal.background = boxWorker.Texture;
        skin.box.normal.textColor = control.ForeColor.ToUnityColor();

        // End Box

        // Start HScrollBar

        // HScrollBar

        var hScrollBarWorker = CreateWorker("HScrollBarNormal", 16, 16)
            .Fill(SystemColors.Control.ToUnityColor())
            .Apply();

        skin.horizontalScrollbar.normal.background = hScrollBarWorker.Texture;

        // HScrollBar Left Button
        skin.horizontalScrollbarLeftButton.normal.background = winForms.Resources.Images.ArrowLeft;

        // HScrollBar Right Button
        skin.horizontalScrollbarRightButton.normal.background = winForms.Resources.Images.ArrowRight;

        // HScrollSlider
        var hScrollSliderNormalWorker = CreateWorker("HScrollSliderNormal", 16, 16)
            .Fill(SystemColors.ScrollBar.ToUnityColor())
            .Apply();

        skin.horizontalScrollbarThumb.normal.background = hScrollSliderNormalWorker.Texture;

        var hScrollSliderHoverWorker = CreateWorker("HScrollSliderHover", 16, 16)
            .Fill(SkinColors.ScrollHoverColor)
            .Apply();

        skin.horizontalSlider.hover.background = hScrollSliderHoverWorker.Texture;

        // End HScrollBar

        // Start HScrollBar

        // HScrollBar

        var vScrollBarWorker = CreateWorker("VScrollBarNormal", 16, 16)
            .Fill(SystemColors.Control.ToUnityColor())
            .Apply();

        skin.verticalScrollbar.normal.background = vScrollBarWorker.Texture;

        // VScrollBar Up Button
        skin.verticalScrollbarUpButton.normal.background = winForms.Resources.Images.ArrowUp;

        // VScrollBar Down Button
        skin.verticalScrollbarDownButton.normal.background = winForms.Resources.Images.ArrowDown;

        // VScrollSlider
        var vScrollSliderNormalWorker = CreateWorker("VScrollSliderNormal", 16, 16)
            .Fill(SystemColors.ScrollBar.ToUnityColor())
            .Apply();

        skin.verticalScrollbarThumb.normal.background = vScrollSliderNormalWorker.Texture;

        var vScrollSliderHoverWorker = CreateWorker("VScrollSliderHover", 16, 16)
            .Fill(SkinColors.ScrollHoverColor)
            .Apply();

        skin.verticalSlider.hover.background = vScrollSliderHoverWorker.Texture;

        // End VScrollBar

        InsertAt(skin.customStyles, enumLength, copy);

        // Debug.Log($"Custom styles length = {skin.customStyles.Length} ({string.Join(", ", skin.customStyles.Select(style => style.name))})");
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private static TextureWorker CreateWorker(string name, int width, int height)
    {
        var worker = new TextureWorker(width, height);
        Workers.Add(name, worker);

        return worker;
    }

    private static void InsertAt<T>(T[] array, int index, T[] subArray)
    {
        if (index + subArray.Length > array.Length)
            throw new ArgumentException($"subArray length cannot be grater than array. ({index} + {subArray.Length} > {array.Length})");

        for (int i = index; i < index + subArray.Length; i++)
        {
            array[i] = subArray[i - index];
        }
    }

    private static string GetName(CustomGUILayout.CustomSyles customStyle, UIState state)
        => $"{customStyle}_{state}";

    private static string CreateStyle(int index, GUIStyle other)
    // , UIState? state = null)
    {
        string name = ((CustomGUILayout.CustomSyles)index).ToString();
        // Debug.Log($"{index}; {name}");

        MySkin.customStyles[index] = new GUIStyle(other)
        {
            // name = state.HasValue ? $"{name}_{state.Value}" : name
            name = name
        };

        return name;
    }

    public void SetLabelTextColor(UnityEngine.Color textColor)
        => SetTextColor("label", textColor);

    public void SetTextColor(string styleName, UnityEngine.Color textColor)
    {
        skin.GetStyle(styleName).normal.textColor = textColor;
    }

    public GUIStyle GetCustomStyle(CustomGUILayout.CustomSyles customStyle)
    {
        return skin.customStyles[(int)customStyle];
    }
}