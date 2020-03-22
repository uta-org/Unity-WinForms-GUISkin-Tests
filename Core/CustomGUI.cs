using System;
using UnityEngine;
using uzLib.Lite.ExternalCode.Unity.Utils;
using uzLib.Lite.ExternalCode.WinFormsSkins.Workers;

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    public static class CustomGUI
    {
        public static GUISkin Skin { get; set; } = SkinWorker.MySkin;

        public static bool IsEditor => !ScenePlaybackDetector.IsPlaying;

        private static int GetID(int altId)
        {
            GUI.Label(Rect.zero, string.Empty);

            var id = GUIUtility.GetControlID(FocusType.Passive);
            Debug.Log(id);
            return id == -1 ? altId : id;
        }

        public static bool Button(Rect rect, string text)
        {
            return Button(rect, text, -1, null);
        }

        public static bool Button(Rect rect, string text, int altId)
        {
            return Button(rect, text, altId, null);
        }

        public static bool Button(Rect rect, string text, Func<GUIStyle, GUIStyle> transformStyle)
        {
            return Button(rect, text, transformStyle);
        }

        public static bool Button(Rect rect, string text, int altId, Func<GUIStyle, GUIStyle> transformStyle)
                   => Button(rect, new GUIContent(text), altId, transformStyle);

        public static bool Button(Rect rect, GUIContent content)
        {
            return Button(rect, content, null);
        }

        public static bool Button(Rect rect, GUIContent content, Func<GUIStyle, GUIStyle> transformStyle)
        {
            return Button(rect, content, 0, transformStyle);
        }

        public static bool Button(Rect rect, GUIContent content, int altId, Func<GUIStyle, GUIStyle> transformStyle)
        {
            if (IsEditor)
                return GUI.Button(rect, content);

            Event e = Event.current;

            var instance = CustomGUIUtility.AddOrGetButtonInstance(GetID(altId));

            bool isHover = instance.ButtonRect.Contains(e.mousePosition);
            bool isToggled = instance.Toggled;

            var style = !isToggled || isHover
                ? Skin?.customStyles?[(int)CustomGUIUtility.CustomStyles.ButtonDisabled]
                : Skin?.customStyles?[(int)CustomGUIUtility.CustomStyles.ButtonEnabled];

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