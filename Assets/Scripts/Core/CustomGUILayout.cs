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
    private Rect buttonRect { get; set; }

    public enum CustomSyles
    {
        ButtonDisabled,
        ButtonEnabled
    }

    // TODO: Uniq identifier
    public bool Button(string text)
    {
        Event e = Event.current;
        bool isHover = buttonRect.Contains(e.mousePosition);

        bool @return = GUILayout.Button(text,
            !IsToggled || isHover
                ? Skin.customStyles[(int)CustomSyles.ButtonDisabled]
                : Skin.customStyles[(int)CustomSyles.ButtonEnabled]);

        if (e.type == EventType.Repaint)
            buttonRect = GUILayoutUtility.GetLastRect();

        if (@return)
            IsToggled = true;

        if (e.type == EventType.MouseUp && IsToggled && !@return)
            IsToggled = false;

        return @return;
    }
}