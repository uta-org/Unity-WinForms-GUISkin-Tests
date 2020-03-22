using System;
using UnityEngine;
using uzLib.Lite.ExternalCode.Unity.Utils;
using uzLib.Lite.ExternalCode.WinFormsSkins.Workers;
using static uzLib.Lite.ExternalCode.WinFormsSkins.Core.CustomGUIUtility;

#if !(!UNITY_2020 && !UNITY_2019 && !UNITY_2018 && !UNITY_2017 && !UNITY_5)

using uzLib.Lite.Extensions;

#else

using uzLib.Lite.ExternalCode.Extensions;

#endif

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    // TODO: Make it static
    public static class CustomGUILayout
    {
        public static GUISkin Skin { get; set; } = SkinWorker.MySkin;

        public static bool IsEditor => !ScenePlaybackDetector.IsPlaying;

        public static bool Button(string text)
        {
            return Button(text, new GUILayoutOption[] { });
        }

        public static bool Button(string text, params GUILayoutOption[] options)
        {
            int @ref = -1;
            return Button(text, ref @ref, null, options);
        }

        public static bool Button(string text, ref int altId, params GUILayoutOption[] options)
        {
            return Button(text, ref altId, null, options);
        }

        public static bool Button(string text, Func<GUIStyle, GUIStyle> transformStyle)
        {
            return Button(text, transformStyle, new GUILayoutOption[] { });
        }

        public static bool Button(string text, Func<GUIStyle, GUIStyle> transformStyle, params GUILayoutOption[] options)
        {
            int @ref = -1;
            return Button(text, ref @ref, transformStyle, options);
        }

        public static bool Button(string text, ref int altId, Func<GUIStyle, GUIStyle> transformStyle,
            params GUILayoutOption[] options)
            => Button(new GUIContent(text), ref altId, transformStyle, options);

        public static bool Button(GUIContent content, ref int altId, Func<GUIStyle, GUIStyle> transformStyle, params GUILayoutOption[] options)
        {
            if (IsEditor)
                return GUILayout.Button(content, options);

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
                @return = !options.IsNullOrEmpty()
                    ? GUILayout.Button(content, transformStyle == null ? style : transformStyle(style), options)
                    : GUILayout.Button(content, transformStyle == null ? style : transformStyle(style));
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