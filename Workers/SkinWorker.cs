// ReSharper disable RedundantUsingDirective
using _System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Unity.API;
using UnityEngine;
using uzLib.Lite.ExternalCode.Unity.Utils;
using uzLib.Lite.ExternalCode.WinFormsSkins.Core;
using Application = UnityEngine.Application;

#if UNITY_EDITOR

using UnityEditor;

#endif

#if UNITY_2020 || UNITY_2019 || UNITY_2018 || UNITY_2017 || UNITY_5

using UnityEngine.Core;

#else

using uzLib.Lite.ExternalCode.Core;

#endif

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Workers
{
    [AutoInstantiate]
    public class SkinWorker : MonoSingleton<SkinWorker>
    {
        internal enum UIState
        {
            Normal,
            Hover,
            Active,
            Focused
        }

        private static GUISkin editorSkin;

        // TODO: Method to create a new GUISkin instance
        // If Instance?.skin == null, we are still on the Editor, so we will force GetSkinForEditor method
        public static GUISkin MySkin => (ScenePlaybackDetector.IsPlaying ? Instance?.skin : GetSkinForEditor()) ?? GetSkinForEditor();

        // Not needed by the moment, editor skin is used in MySkin
        // public static GUISkin DefaultSkin => Instance.defaultSkin;

        private static Dictionary<string, TextureWorker> Workers { get; } = new Dictionary<string, TextureWorker>();

        [SerializeField]
        private global::Unity.API.UnityWinForms winForms;

        [SerializeField]
        private GUISkin skin;

        // private GUISkin defaultSkin;

        private readonly Control control = new Control();

        [MenuItem("Window/Get Builtin skin...")]
        public static void GetSkin()
        {
#if UNITY_EDITOR
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
#endif
        }

        public static GUISkin GetSkinForEditor()
        {
#if UNITY_EDITOR
            if (editorSkin != null)
                return editorSkin;

            editorSkin = Instantiate(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene));
            return editorSkin;
#else
            return null;
#endif
        }

        private void Awake()
        {
            Instance = this;
            skin = Instantiate(skin);
            // defaultSkin = Instantiate(skin);

            // Start modifying the skin

            // TODO: Add regions

            int buttonDisabledStyleIndex = (int)CustomGUIUtility.CustomStyles.ButtonDisabled;
            int buttonEnabledStyleIndex = (int)CustomGUIUtility.CustomStyles.ButtonEnabled;

            // Debug.Log($"Custom styles length = {skin.customStyles.Length} ({string.Join(", ", skin.customStyles.Select(style => style.name))})");

            var copy = new GUIStyle[skin.customStyles.Length];
            Array.Copy(skin.customStyles, copy, skin.customStyles.Length);

            int enumLength = Enum.GetNames(typeof(CustomGUIUtility.CustomStyles)).Length;

            int length = enumLength + skin.customStyles.Length;
            skin.customStyles = new GUIStyle[length];

            for (int i = 0; i < length; i++)
                skin.customStyles[i] = new GUIStyle();

            // Start Button disabled
            var buttonDisabledWorkerNormal = CreateWorker(CreateStyle(buttonDisabledStyleIndex, skin.button), 16, 16)
                .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
                .Fill(SystemColors.Control.ToUnityColor())
                .Apply();

            skin.customStyles[buttonDisabledStyleIndex].normal.background = buttonDisabledWorkerNormal.Texture;
            skin.customStyles[buttonDisabledStyleIndex].normal.textColor = control.ForeColor.ToUnityColor();

            var buttonDisabledWorkerHover = CreateWorker(GetName(CustomGUIUtility.CustomStyles.ButtonDisabled, UIState.Hover), 16, 16)
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
                .Fill(SystemColors.Control.ToUnityColor()) // TODO: Color.clear?
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

            // Start Tooltip

            int tooltipIndex = (int)CustomGUIUtility.CustomStyles.Tooltip;

            var tooltipWorker = CreateWorker(CreateStyle(tooltipIndex, skin.box), 16, 16)
                .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
                .Fill(UnityEngine.Color.white)
                .Apply();

            skin.customStyles[tooltipIndex].normal.background = tooltipWorker.Texture;
            skin.customStyles[tooltipIndex].normal.textColor = UnityEngine.Color.black;

            // End Tooltip

            // Start TextField

            /*

                BackColor = SystemColors.Window;
                uwfBorderColor = SystemColors.ActiveBorder;
                uwfBorderFocusedColor = Color.FromArgb(86, 157, 229);
                uwfBorderHoverColor = Color.FromArgb(126, 180, 234);

             */

            //int textFieldIndex = (int)CustomGUIUtility.CustomStyles.TextField;

            var textFieldNormalWorker = CreateWorker("textField", 16, 16)
                .SetBorders(SystemColors.ActiveBorder.ToUnityColor(), 1)
                .Fill(SystemColors.Window.ToUnityColor())
                .Apply();

            skin.textField.normal.background = textFieldNormalWorker.Texture;
            skin.textField.normal.textColor = control.ForeColor.ToUnityColor();

            // GetName(CustomGUIUtility.CustomStyles.ButtonDisabled, UIState.Hover)

            var textFieldHoverWorker = CreateWorker("textFieldHover", 16, 16)
                .SetBorders(new Color32(126, 180, 234, 255), 1)
                .Fill(SystemColors.Window.ToUnityColor())
                .Apply();

            skin.textField.hover.background = textFieldHoverWorker.Texture;
            skin.textField.hover.textColor = control.ForeColor.ToUnityColor();

            var textFieldFocusedWorker = CreateWorker("textFieldFocused", 16, 16)
                .SetBorders(new Color32(126, 180, 234, 255), 1)
                .Fill(SystemColors.Window.ToUnityColor())
                .Apply();

            skin.textField.focused.background = textFieldFocusedWorker.Texture;
            skin.textField.focused.textColor = control.ForeColor.ToUnityColor();

            // End TextField

            InsertAt(skin.customStyles, enumLength, copy);

            //Debug.Log($"Custom styles length = {skin.customStyles.Length} ({string.Join(", ", skin.customStyles.Select(style => style.name))})");
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public static TextureWorker CreateWorker(string name, int width, int height)
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

        private static string GetName(CustomGUIUtility.CustomStyles customStyle, UIState state)
            => $"{customStyle}_{state}";

        private static string CreateStyle(int index, GUIStyle other)
        {
            string name = ((CustomGUIUtility.CustomStyles)index).ToString();

            Instance.skin.customStyles[index] = new GUIStyle(other)
            {
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

        public GUIStyle GetCustomStyle(CustomGUIUtility.CustomStyles customStyle)
        {
            var style = skin.customStyles[(int)customStyle];
            // Debug.Log($"{customStyle} == {style.name}");
            return style;
        }
    }
}