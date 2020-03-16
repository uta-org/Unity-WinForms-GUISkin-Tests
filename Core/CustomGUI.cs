using System;
using System.Collections.Generic;
using UnityEngine;

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    public class CustomGUI
    {
        private CustomGUI()
        {
        }

        public CustomGUI(GUISkin skin)
        {
            Skin = skin;
        }

        private GUISkin Skin { get; }

        private Dictionary<int, bool> IsToggled { get; } = new Dictionary<int, bool>();
        private Rect buttonRect { get; set; }
        private int InternalCounter { get; set; }

        private EventType LastEvent { get; set; } = EventType.Layout;
        //private int MaxCount { get; set; }

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

        public bool Button(Rect rect, GUIContent content, Func<GUIStyle, GUIStyle> transformStyle)
        {
            if (transformStyle == null)
                throw new ArgumentNullException(nameof(transformStyle));

#if UNITY_EDITOR
            return GUI.Button(rect, content, transformStyle(null));
#else

            Event e = Event.current;

            int count = InternalCount();

            bool contains = IsToggled.ContainsKey(count);
            bool isToggled = contains && IsToggled[count];

            if (!contains)
                IsToggled.Add(count, false);

            bool isHover = buttonRect.Contains(e.mousePosition);

            bool @return = GUI.Button(rect, content,
                transformStyle(!isToggled || isHover
                    ? Skin.customStyles[(int)CustomGUILayout.CustomSyles.ButtonDisabled]
                    : Skin.customStyles[(int)CustomGUILayout.CustomSyles.ButtonEnabled]));

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