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

    private bool IsToggled { get; set; }

    public enum CustomSyles
    {
        ButtonDisabled,
        ButtonEnabled
    }

    // TODO: Uniq identifier
    public bool Button(string text)
    {
        // TODO: Rect
        bool @return = GUILayout.Button(text,
            !IsToggled
                ? Skin.customStyles[(int)CustomSyles.ButtonDisabled]
                : Skin.customStyles[(int)CustomSyles.ButtonEnabled]);

        if (@return)
            IsToggled = true;

        Event e = Event.current;
        if (e.type == EventType.MouseUp && IsToggled && !@return)
            IsToggled = false;

        return @return;
    }
}