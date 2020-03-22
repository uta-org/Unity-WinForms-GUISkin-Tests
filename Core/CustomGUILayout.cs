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

        //private Dictionary<int, bool> IsToggled { get; } = new Dictionary<int, bool>();
        //private Rect buttonRect { get; set; }
        //private int InternalCounter { get; set; }

        //private EventType LastEvent { get; set; } = EventType.Layout;

        public static GUIStyle PaginationStyle => Skin.customStyles[(int)CustomSyles.ButtonEnabled];
        // new GUIStyle("button") { normal = GUI.skin.button.active };

        public static bool IsEditor => !ScenePlaybackDetector.IsPlaying;
        //Application.isEditor && !Application.isPlaying;

        public enum CustomSyles
        {
            ButtonDisabled,
            ButtonEnabled,
            Tooltip,
            TextField
        }

        //private int InternalCount()
        //{
        //    // TODO: Button counter gets resetted on click, so I can't control which buttons get's active
        //    Event e = Event.current;
        //    var currentType = e.type;

        //    bool isCountable = true; // currentType == EventType.Repaint || currentType == EventType.Layout;
        //    int count = isCountable ? InternalCounter++ : InternalCounter;

        //    if (currentType != LastEvent && isCountable)
        //    {
        //        InternalCounter = 0;
        //        count = 0;
        //    }

        //    LastEvent = currentType;
        //    return count;
        //}

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

            //int count = InternalCount();
            //// Debug.Log($"Type: {e.type}; {count}");

            //bool contains = IsToggled.ContainsKey(count);
            //bool isToggled = contains && IsToggled[count];

            //if (!contains)
            //    IsToggled.Add(count, false);

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

            //var lastControlID = GUIUtility.GetControlID(FocusType.Passive);
            //Debug.Log(lastControlID);

            if (e.type == EventType.Repaint && instance.ButtonRect == default)
                instance.ButtonRect = GUILayoutUtility.GetLastRect();

            if (@return)
                instance.Toggled = true;

            if (e.type == EventType.MouseUp && isToggled && !@return)
                instance.Toggled = false;

            return @return;
        }

        //    public static bool Button(string text, Func<GUIStyle, GUIStyle> transformStyle)
        //    {
        //        return Button(text, transformStyle, -1, new GUILayoutOption[] { });
        //    }

        //    public static bool Button(string text, Func<GUIStyle, GUIStyle> transformStyle, params GUILayoutOption[] options)
        //    {
        //        return Button(text, transformStyle, -1, options);
        //    }

        //    public static bool Button(string text, Func<GUIStyle, GUIStyle> transformStyle, int altId, params GUILayoutOption[] options)
        //    {
        //        if (transformStyle == null)
        //            throw new ArgumentNullException(nameof(transformStyle));

        //        if (IsEditor)
        //            return GUILayout.Button(text, transformStyle(null), options);

        //        Event e = Event.current;

        //        int count = InternalCount();
        //        // Debug.Log($"Type: {e.type}; {count}");

        //        bool contains = IsToggled.ContainsKey(count);
        //        bool isToggled = contains && IsToggled[count];

        //        if (!contains)
        //            IsToggled.Add(count, false);

        //        bool isHover = buttonRect.Contains(e.mousePosition);

        //        bool @return;
        //        try
        //        {
        //            if (!options.IsNullOrEmpty())
        //                @return = GUILayout.Button(text,
        //                    !isToggled || isHover
        //                        ? Skin?.customStyles?[(int)CustomSyles.ButtonDisabled]
        //                        : Skin?.customStyles?[(int)CustomSyles.ButtonEnabled], options);
        //            else
        //                @return = GUILayout.Button(text,
        //                    !isToggled || isHover
        //                        ? Skin?.customStyles?[(int)CustomSyles.ButtonDisabled]
        //                        : Skin?.customStyles?[(int)CustomSyles.ButtonEnabled]);
        //        }
        //        catch
        //        {
        //            @return = false;
        //            Debug.LogWarning("Exception occurred drawing button on CustomGUILayout!");
        //        }

        //        if (e.type == EventType.Repaint)
        //            buttonRect = GUILayoutUtility.GetLastRect();

        //        if (@return)
        //            IsToggled[count] = true;

        //        if (e.type == EventType.MouseUp && isToggled && !@return)
        //            IsToggled[count] = false;

        //        return @return;
        //    }
        //}
    }
}