using System;
using System.Collections.Generic;
using UnityEngine;
using uzLib.Lite.ExternalCode.Extensions;
using uzLib.Lite.ExternalCode.Unity.Utils;
using uzLib.Lite.ExternalCode.Unity.Utils.Threading;
using uzLib.Lite.ExternalCode.WinFormsSkins.Workers;
using Object = UnityEngine.Object;

#if !(!UNITY_2020 && !UNITY_2019 && !UNITY_2018 && !UNITY_2017 && !UNITY_5)

using uzLib.Lite.Core;

#endif

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    // TODO: Make it static
    public static class CustomGUILayout
    {
        //[ThreadStatic]
        //private static readonly bool m_IsMainThread = true;

        //public CustomGUILayout()
        //{
        //}

        //public CustomGUILayout(GUISkin other)
        //{
        //    if (m_IsMainThread)
        //        Skin = Object.Instantiate(other);
        //    else
        //        throw new InvalidOperationException("CustomGUILayout must be called on the main thread!");

        //    //// TODO: This is not working...
        //    //Debug.LogWarning("This call was made outside of main thread, calling Dispatcher...");
        //    //Dispatcher.Invoke(() =>
        //    //{
        //    //    Debug.Log("Established Skin object!");
        //    //    Skin = Object.Instantiate(skin);
        //    //});
        //}

        public static GUISkin Skin { get; set; } = SkinWorker.MySkin;

        public static GUIStyle PaginationStyle => Skin.customStyles[(int)CustomSyles.ButtonEnabled];

        public static bool IsEditor => !ScenePlaybackDetector.IsPlaying;

        private static int GetID(int altId)
        {
            GUI.Label(Rect.zero, string.Empty);

            var id = GUIUtility.GetControlID(FocusType.Passive);
            Debug.Log(id);
            return id == -1 ? altId : id;
        }

        public static bool Button(string text)
        {
            return Button(text, new GUILayoutOption[] { });
        }

        public static bool Button(string text, params GUILayoutOption[] options)
        {
            return Button(text, -1, null, options);
        }

        public static bool Button(string text, int altId, params GUILayoutOption[] options)
        {
            return Button(text, altId, null, options);
        }

        public static bool Button(string text, Func<GUIStyle, GUIStyle> transformStyle)
        {
            return Button(text, transformStyle, new GUILayoutOption[] { });
        }

        public static bool Button(string text, Func<GUIStyle, GUIStyle> transformStyle, params GUILayoutOption[] options)
        {
            return Button(text, -1, transformStyle, options);
        }

        public static bool Button(string text, int altId, Func<GUIStyle, GUIStyle> transformStyle,
            params GUILayoutOption[] options)
            => Button(new GUIContent(text), altId, transformStyle, options);

        public static bool Button(GUIContent content, int altId, Func<GUIStyle, GUIStyle> transformStyle, params GUILayoutOption[] options)
        {
            if (IsEditor)
                return GUILayout.Button(content, options);

            Event e = Event.current;

            var instance = CustomGUIUtility.AddOrGetButtonInstance(GetID(altId));

            bool isHover = instance.ButtonRect.Contains(e.mousePosition);
            bool isToggled = instance.Toggled;

            var style = !isToggled || isHover
                ? Skin?.customStyles?[(int)CustomSyles.ButtonDisabled]
                : Skin?.customStyles?[(int)CustomSyles.ButtonEnabled];

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