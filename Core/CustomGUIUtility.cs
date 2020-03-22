using System;
using System.Collections.Generic;
using UnityEngine;

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    public static class CustomGUIUtility
    {
        public enum CustomSyles
        {
            ButtonDisabled,
            ButtonEnabled,
            Tooltip,
            TextField
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