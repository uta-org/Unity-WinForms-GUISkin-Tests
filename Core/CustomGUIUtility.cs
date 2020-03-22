using System;
using System.Collections.Generic;
using UnityEngine;

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    public static class CustomGUIUtility
    {
        public enum CustomStyles
        {
            ButtonDisabled,
            ButtonEnabled,
            Tooltip,
            TextField
        }

        public static int GetID(int altId)
        {
            GUI.Label(Rect.zero, string.Empty);

            var id = GUIUtility.GetControlID(FocusType.Passive);
            Debug.Log(id);
            return id == -1 ? altId : id;
        }

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