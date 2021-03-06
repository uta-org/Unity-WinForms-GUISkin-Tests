﻿using System;
using System.Collections.Generic;
using UnityEngine;
using uzLib.Lite.ExternalCode.WinFormsSkins.Workers;

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Core
{
    public static class CustomGUIUtility
    {
        public enum CustomStyles
        {
            ButtonDisabled,
            ButtonEnabled,
            Tooltip
        }

        public static GUIStyle PaginationStyle => SkinWorker.MySkin.customStyles[(int)CustomStyles.ButtonEnabled];

        private static HashSet<int> m_IDs = new HashSet<int>();
        private static System.Random m_Random = new System.Random();

        public static int IdCount => m_IDs.Count;

        public static int GetID(ref int altId)
        {
            GUI.Label(Rect.zero, string.Empty);

            var id = GUIUtility.GetControlID(FocusType.Passive);
            var invalidId = id == -1;
            var realId = invalidId ? altId : id;

            if (invalidId)
            {
                if (m_IDs.Contains(altId) && altId > -1)
                    return altId;

                altId = GetFreeId();
                realId = altId;
            }
            else
            {
                if (!m_IDs.Contains(id))
                    m_IDs.Add(id);
            }

            //Debug.Log(id + " " + realId);
            return realId;
        }

        private static int GetFreeId()
        {
            var id = m_Random.Next();
            while (!m_IDs.Add(id))
                id = m_Random.Next();
            return id;
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

            // ReSharper disable once UnusedMember.Local
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