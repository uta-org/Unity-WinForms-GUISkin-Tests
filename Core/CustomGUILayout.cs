using System;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_2020 && !UNITY_2019 && !UNITY_2018 && !UNITY_2017 && !UNITY_5

using Unity_WinForms_Premium.CommonCore;

#else

using uzLib.Lite.Core;

#endif

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    public class CustomGUILayout
    {
        private CustomGUILayout()
        {
        }

        public CustomGUILayout(GUISkin skin)
        {
            Skin = skin;
        }

        private GUISkin Skin { get; }

        private Dictionary<int, bool> IsToggled { get; } = new Dictionary<int, bool>();
        private Rect buttonRect { get; set; }
        private int InternalCounter { get; set; }

        private EventType LastEvent { get; set; } = EventType.Layout;

        public GUIStyle PaginationStyle => Skin.customStyles[(int)CustomSyles.ButtonEnabled];
        // new GUIStyle("button") { normal = GUI.skin.button.active };

        public enum CustomSyles
        {
            ButtonDisabled,
            ButtonEnabled,
            Tooltip,
            TextField
        }

        private int InternalCount()
        {
            // TODO: Button counter gets resetted on click, so I can't control which buttons get's active
            Event e = Event.current;
            var currentType = e.type;

            bool isCountable = true; // currentType == EventType.Repaint || currentType == EventType.Layout;
            int count = isCountable ? InternalCounter++ : InternalCounter;

            if (currentType != LastEvent && isCountable)
            {
                InternalCounter = 0;
                count = 0;
            }

            LastEvent = currentType;
            return count;
        }

        // TODO: Uniq identifier
        public bool Button(string text, params GUILayoutOption[] options)
        {
#if UNITY_EDITOR
            return GUILayout.Button(text, options);
#else

            Event e = Event.current;

            int count = InternalCount();
            // Debug.Log($"Type: {e.type}; {count}");

            bool contains = IsToggled.ContainsKey(count);
            bool isToggled = contains && IsToggled[count];

            if (!contains)
                IsToggled.Add(count, false);

            bool isHover = buttonRect.Contains(e.mousePosition);

            bool @return = GUILayout.Button(text,
                !isToggled || isHover
                    ? Skin.customStyles[(int)CustomSyles.ButtonDisabled]
                    : Skin.customStyles[(int)CustomSyles.ButtonEnabled], options);

            if (e.type == EventType.Repaint)
                buttonRect = GUILayoutUtility.GetLastRect();

            if (@return)
                IsToggled[count] = true;

            if (e.type == EventType.MouseUp && isToggled && !@return)
                IsToggled[count] = false;

            return @return;
#endif
        }

        public bool Button(string text, Func<GUIStyle, GUIStyle> transformStyle, params GUILayoutOption[] options)
        {
            if (transformStyle == null)
                throw new ArgumentNullException(nameof(transformStyle));

#if UNITY_EDITOR
            return GUILayout.Button(text, transformStyle(null), options);
#else

            Event e = Event.current;

            int count = InternalCount();
            // Debug.Log($"Type: {e.type}; {count}");

            bool contains = IsToggled.ContainsKey(count);
            bool isToggled = contains && IsToggled[count];

            if (!contains)
                IsToggled.Add(count, false);

            bool isHover = buttonRect.Contains(e.mousePosition);

            bool @return = GUILayout.Button(text,
                transformStyle(!isToggled || isHover
                    ? Skin.customStyles[(int)CustomSyles.ButtonDisabled]
                    : Skin.customStyles[(int)CustomSyles.ButtonEnabled]), options);

            if (e.type == EventType.Repaint)
                buttonRect = GUILayoutUtility.GetLastRect();

            if (@return)
                IsToggled[count] = true;

            if (e.type == EventType.MouseUp && isToggled && !@return)
                IsToggled[count] = false;

            return @return;
#endif
        }
    }
}