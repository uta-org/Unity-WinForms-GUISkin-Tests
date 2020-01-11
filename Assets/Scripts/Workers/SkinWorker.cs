using System;
using System.Collections;
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
        string dir = Path.Combine(Application.dataPath, "Saved Skins/");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        List<string> enums = new List<string>();
        Array vals = Enum.GetValues(typeof(EditorSkin));
        foreach (EditorSkin es in vals)
        {
            GUISkin skin = Instantiate(EditorGUIUtility.GetBuiltinSkin(es));

            string n = $"SceneSkin{es}.guiskin";
            enums.Add(n);
            n = "Assets/Saved Skins/" + n;

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

        // Start Button
        var buttonWorker = CreateWorker("ButtonStyle", 16, 16)
            .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
            .Fill(SystemColors.Control.ToUnityColor())
            .Apply();

        skin.button.normal.background = buttonWorker.Texture;
        skin.button.normal.textColor = control.ForeColor.ToUnityColor();
        // End Button

        // Start Window
        var windowWorker = CreateWorker("WindowStyle", 16, 16)
            .Fill(Color.gray)
            .SmartFill(new RectInt(0, 0, 16, 4), Color.black)
            .Apply();

        skin.window.normal.background = windowWorker.Texture;
        // skin.window.border = new RectOffset();
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