using System;
using System.Collections.Generic;
using UnityEngine;
using uzLib.Lite.ExternalCode.Extensions;
using uzLib.Lite.ExternalCode.Unity.Utils;
using uzLib.Lite.ExternalCode.WinFormsSkins.Workers;
using Object = UnityEngine.Object;

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    public static class CustomGUI
    {
        //private CustomGUI()
        //{
        //}

        //public CustomGUI(GUISkin skin)
        //{
        //    Skin = Object.Instantiate(skin);
        //}

        //private GUISkin Skin { get; }

        //private Dictionary<int, bool> IsToggled { get; } = new Dictionary<int, bool>();
        //private Rect buttonRect { get; set; }
        //private int InternalCounter { get; set; }

        //private EventType LastEvent { get; set; } = EventType.Layout;
        ////private int MaxCount { get; set; }

        //public static bool IsEditor => !ScenePlaybackDetector.IsPlaying;

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

        //public bool Button(Rect rect, GUIContent content, Func<GUIStyle, GUIStyle> transformStyle)
        //{
        //    if (transformStyle == null)
        //        throw new ArgumentNullException(nameof(transformStyle));

        //    if (IsEditor)
        //        return GUI.Button(rect, content, transformStyle(null));

        //    Event e = Event.current;

        //    int count = InternalCount();

        //    bool contains = IsToggled.ContainsKey(count);
        //    bool isToggled = contains && IsToggled[count];

        //    if (!contains)
        //        IsToggled.Add(count, false);

        //    bool isHover = buttonRect.Contains(e.mousePosition);

        //    bool @return = GUI.Button(rect, content,
        //        transformStyle(!isToggled || isHover
        //            ? Skin.customStyles[(int)CustomGUILayout.CustomSyles.ButtonDisabled]
        //            : Skin.customStyles[(int)CustomGUILayout.CustomSyles.ButtonEnabled]));

        //    if (e.type == EventType.Repaint)
        //        buttonRect = GUILayoutUtility.GetLastRect();

        //    if (@return)
        //        IsToggled[count] = true;

        //    if (e.type == EventType.MouseUp && isToggled && !@return)
        //        IsToggled[count] = false;

        //    return @return;
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
                ? Skin?.customStyles?[(int)CustomGUILayout.CustomSyles.ButtonDisabled]
                : Skin?.customStyles?[(int)CustomGUILayout.CustomSyles.ButtonEnabled];

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
    }
}