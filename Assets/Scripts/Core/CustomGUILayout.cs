using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public static class CustomGUILayout
{
    private static HashSet<string> ToggledButtons = new HashSet<string>();

    public enum CustomSyles
    {
        Button
    }

    // TODO: Uniq identifier
    public static bool Button(string text)
    {
        GUI.skin = SkinWorker.MySkin;

        // TODO: Rect
        GUILayout.Toggle(ToggledButtons.Contains(text), text);
        //GUILayout.Toggle(ToggledButtons.Contains(text), text, GUI.skin.customStyles[(int)CustomSyles.Button]);
        var r = GUILayoutUtility.GetLastRect();
        Debug.Log(r); // TODO

        bool clicked = false;
        Event e = Event.current;
        if (r.Contains(e.mousePosition) && e.type == EventType.MouseDown)
        {
            ToggledButtons.Add(text);
            clicked = true;
        }
        else if (!r.Contains(e.mousePosition) && e.type == EventType.MouseDown)
        {
            ToggledButtons.Remove(text);
        }

        return clicked;
    }
}