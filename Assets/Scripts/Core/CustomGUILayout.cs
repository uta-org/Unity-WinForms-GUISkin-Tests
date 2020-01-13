using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

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

    private bool IsToggled;

    public enum CustomSyles
    {
        ButtonDisabled,
        ButtonEnabled
    }

    // TODO: Uniq identifier
    public bool Button(string text)
    {
        // TODO: Rect
        var @return = GUILayout.Button(text,
            !IsToggled
                ? Skin.customStyles[(int)CustomSyles.ButtonDisabled]
                : Skin.customStyles[(int)CustomSyles.ButtonEnabled]);
        Event e = Event.current;

        if (@return)
        {
            IsToggled = true;
        }
        else if (IsToggled && e.type == EventType.MouseDown)
        {
            IsToggled = false;
        }

        return @return;
    }
}