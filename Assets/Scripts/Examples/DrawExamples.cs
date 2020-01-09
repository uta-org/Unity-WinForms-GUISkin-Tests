using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DrawExamples : MonoBehaviour
{
    private TextureWorker worker;

    // Start is called before the first frame update
    private void Start()
    {
        worker = new TextureWorker(32, 32)
            .Fill(Color.clear)
            .CreateRoundedBorders(Color.red, 5)
            .Apply();

        // .DrawSector(16, new Range(0, 90), Color.red);

        // .DrawCircle(16, Color.red);
        // .Apply();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(5, 5, 32, 32), worker.GetTexture());

        GUI.Label(new Rect(Screen.width - 205, 5, 200, 25), $"Drawn pixels: {TextureUtils.drawnPixels}");
    }
}