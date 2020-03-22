using System;
using UnityEngine;
using uzLib.Lite.ExternalCode.Unity.Utils;
using uzLib.Lite.ExternalCode.WinFormsSkins.Workers;
using static uzLib.Lite.ExternalCode.WinFormsSkins.Core.CustomGUIUtility;

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    public static class CustomGUI
    {
        public static GUISkin Skin { get; set; } = SkinWorker.MySkin;

        public static bool IsEditor => !ScenePlaybackDetector.IsPlaying;

        public static bool Button(Rect rect, string text)
        {
            int @ref = -1;
            return Button(rect, text, ref @ref, null);
        }

        public static bool Button(Rect rect, string text, ref int altId)
        {
            return Button(rect, text, ref altId, null);
        }

        public static bool Button(Rect rect, string text, Func<GUIStyle, GUIStyle> transformStyle)
        {
            return Button(rect, text, transformStyle);
        }

        public static bool Button(Rect rect, string text, ref int altId, Func<GUIStyle, GUIStyle> transformStyle)
                   => Button(rect, new GUIContent(text), ref altId, transformStyle);

        public static bool Button(Rect rect, GUIContent content)
        {
            return Button(rect, content, null);
        }

        public static bool Button(Rect rect, GUIContent content, Func<GUIStyle, GUIStyle> transformStyle)
        {
            int @ref = -1;
            return Button(rect, content, ref @ref, transformStyle);
        }

        public static bool Button(Rect rect, GUIContent content, ref int altId, Func<GUIStyle, GUIStyle> transformStyle)
        {
            if (IsEditor)
                return GUI.Button(rect, content);

            Event e = Event.current;

            var instance = AddOrGetButtonInstance(GetID(ref altId));

            bool isHover = instance.ButtonRect.Contains(e.mousePosition);
            bool isToggled = instance.Toggled;

            var style = !isToggled || isHover
                ? Skin?.customStyles?[(int)CustomStyles.ButtonDisabled]
                : Skin?.customStyles?[(int)CustomStyles.ButtonEnabled];

            bool @return;
            try
            {
                @return = GUI.Button(rect, content, transformStyle == null ? style : transformStyle(style));
            }
            catch
            {
                @return = false;
                Debug.LogWarning("Exception occurred drawing button on CustomGUILayout!");
            }

            if (e.type == EventType.Repaint && instance.ButtonRect == default)
                instance.ButtonRect = GUILayoutUtility.GetLastRect();

            if (@return)
                instance.Toggled = true;

            if (e.type == EventType.MouseUp && isToggled && !@return)
                instance.Toggled = false;

            return @return;
        }
    }
}