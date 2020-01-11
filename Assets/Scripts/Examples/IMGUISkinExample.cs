using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMGUISkinExample : MonoBehaviour
{
    //[SerializeField]
    //private GUISkin skin;

    private Rect windowPos;

    // Start is called before the first frame update
    private void Start()
    {
        windowPos = new Rect(0, 0, 200, 200);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnGUI()
    {
        GUI.skin = SkinWorker.MySkin;
        windowPos = GUI.Window(0, windowPos, ExampleWindow, "Title");
    }

    private void ExampleWindow(int id)
    {
        GUI.DragWindow();
        CustomGUILayout.Button("This is a test");
    }
}