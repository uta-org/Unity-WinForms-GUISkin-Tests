using System;
using System.Collections.Generic;
using UnityEngine;

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    public static class CustomGUIUtility
    {
        private static readonly Dictionary<int, ButtonInstance> m_ButtonInstances = new Dictionary<int, ButtonInstance>();
        private static readonly Dictionary<int, ButtonInstance> m_ButtonLayoutInstances = new Dictionary<int, ButtonInstance>();

        internal static ButtonInstance AddOrGetButtonInstance(int id, bool layoutMode = true)
        {
            if (id == -1) throw new ArgumentException(@"Invalid id provided", nameof(id));
            var instance = new ButtonInstance(id);

            if (GetDictionary(layoutMode).ContainsKey(id))
                return GetDictionary(layoutMode)[id];

            GetDictionary(layoutMode).Add(id, instance);
            return GetDictionary(layoutMode)[id];
        }

        //internal static bool GetToggleState(int id, bool layoutMode = true)
        //    => GetDictionary(layoutMode)[id].Toggled;

        //internal static void SetToggleState(int id, bool state, bool layoutMode = true)
        //    => GetDictionary(layoutMode)[id].Toggled = state;

        //internal static Rect GetButtonRect(int id, bool layoutMode = true)
        //    => GetDictionary(layoutMode)[id].ButtonRect;

        private static Dictionary<int, ButtonInstance> GetDictionary(bool layoutMode)
            => layoutMode ? m_ButtonLayoutInstances : m_ButtonInstances;

        internal sealed class ButtonInstance
        {
            public int Id { get; }
            public bool Toggled { get; set; }
            public Rect ButtonRect { get; set; }

            private ButtonInstance()
            {
            }

            public ButtonInstance(int id)
            {
                if (id == -1) throw new ArgumentException(@"Invalid id provided", nameof(id));
                Id = id;
            }
        }
    }
}