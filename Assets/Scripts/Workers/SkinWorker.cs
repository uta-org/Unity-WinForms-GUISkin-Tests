using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Unity.API;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
using Color = UnityEngine.Color;

public class SkinWorker : MonoBehaviour
{
    public static SkinWorker Instance { get; private set; }
    public static GUISkin MySkin => Instance.skin;

    private static Dictionary<string, TextureWorker> Workers { get; } = new Dictionary<string, TextureWorker>();

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
        Array vals = Enum.GetValues(typeof(EditorSkin));
        foreach (EditorSkin es in vals)
        {
            GUISkin skin = Instantiate(EditorGUIUtility.GetBuiltinSkin(es));

            string n = $"SceneSkin{es}.guiskin";
            enums.Add(n);
            n = "Assets/Resources/Saved Skins/" + n;

            AssetDatabase.CreateAsset(skin, n); //There should be an dialog to set the name!
        }
        EditorUtility.DisplayDialog("API Message",
            $"GUI Skin saved in 'Saved Skins' folder all {vals.Length} scripts with names: ({string.Join(", ", enums.ToArray())})!", "Ok");
    }

    private void Awake()
    {
        Instance = this;
        skin = Instantiate(skin);

        // Start modifying the skin

        // TODO: Add regions

        int buttonStyleIndex = (int)CustomGUILayout.CustomSyles.Button;

        skin.customStyles[buttonStyleIndex] = skin.button;

        // Start Button
        var buttonWorkerNormal = CreateWorker("ButtonStyleNormal", 16, 16)
            .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
            .Fill(SystemColors.Control.ToUnityColor())
            .Apply();

        skin.customStyles[buttonStyleIndex].normal.background = buttonWorkerNormal.Texture;
        skin.customStyles[buttonStyleIndex].normal.textColor = control.ForeColor.ToUnityColor();

        //var buttonWorkerOnNormal = CreateWorker("ButtonStyleOnNormal", 16, 16)
        //    .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
        //    .Fill(SystemColors.Control.ToUnityColor())
        //    .Apply();

        skin.customStyles[buttonStyleIndex].onNormal.background = buttonWorkerNormal.Texture;
        skin.customStyles[buttonStyleIndex].onNormal.textColor = control.ForeColor.ToUnityColor();

        var buttonWorkerHovered = CreateWorker("ButtonStyleHovered", 16, 16)
            .SetBorders(SkinColors.BorderHoverColor, 1)
            .Fill(SkinColors.HoverColor)
            .Apply();

        skin.customStyles[buttonStyleIndex].hover.background = buttonWorkerHovered.Texture;
        skin.customStyles[buttonStyleIndex].hover.textColor = control.ForeColor.ToUnityColor();

        //var buttonWorkerOnHovered = CreateWorker("ButtonStyleOnHovered", 16, 16)
        //    .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
        //    .Fill(SystemColors.Control.ToUnityColor())
        //    .Apply();

        skin.customStyles[buttonStyleIndex].onHover.background = buttonWorkerHovered.Texture;
        skin.customStyles[buttonStyleIndex].onHover.textColor = control.ForeColor.ToUnityColor();

        var buttonWorkerActive = CreateWorker("ButtonStyleActive", 16, 16)
            .SetBorders(SkinColors.BorderHoverColor, 1)
            .Fill(SystemColors.Control.ToUnityColor())
            .Apply();

        skin.customStyles[buttonStyleIndex].onActive.background = buttonWorkerActive.Texture;
        skin.customStyles[buttonStyleIndex].onActive.textColor = control.ForeColor.ToUnityColor();

        skin.customStyles[buttonStyleIndex].onNormal.textColor = Color.red;
        skin.customStyles[buttonStyleIndex].onActive.textColor = Color.green;
        skin.customStyles[buttonStyleIndex].onHover.textColor = Color.blue;
        skin.customStyles[buttonStyleIndex].onFocused.textColor = Color.yellow;

        // End Button

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
}

public static class SkinColors
{
    public static Color BorderHoverColor => new Color32(126, 180, 234, 255);
    public static Color HoverColor => new Color32(223, 238, 252, 255);
}