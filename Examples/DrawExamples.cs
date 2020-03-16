using UnityEngine;
using uzLib.Lite.ExternalCode.WinFormsSkins.Utils;
using uzLib.Lite.ExternalCode.WinFormsSkins.Workers;
using static uzLib.Lite.ExternalCode.WinFormsSkins.Workers.TextureWorker;

namespace uzLib.Lite.ExternalCode.WinFormsSkins.Examples
{
    public class DrawExamples : MonoBehaviour
    {
        private TextureWorker worker;
        private TextureWorker circleWorker;

        // Start is called before the first frame update
        private void Start()
        {
            worker = new TextureWorker(32, 32)
                .Fill(Color.clear)
                .CreateRoundedBorders(Color.red, 5)
                .Apply();

            circleWorker = new TextureWorker(32, 32)
                .SetBorders(Color.black, 1)
                .Fill(Color.red)
                .Apply();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnGUI()
        {
            DrawTextureWithTooltip(new Rect(5, 5, 32, 32), worker.Texture, worker.DrawnPixels.ToString());
            DrawTextureWithTooltip(new Rect(5, 42, 32, 32), circleWorker.Texture, circleWorker.DrawnPixels.ToString());

            GUI.Label(new Rect(Screen.width - 205, 5, 200, 25), $"Drawn pixels: {TextureUtils.drawnPixels}");
        }

        private void DrawTextureWithTooltip(Rect rect, Texture2D texture, string tooltip)
        {
            GUI.DrawTexture(rect, texture);

            Event e = Event.current;
            if (rect.Contains(e.mousePosition))
            {
                GUIContent content = new GUIContent(tooltip);

                GUIStyle style = GUI.skin.box;
                style.alignment = TextAnchor.MiddleCenter;

                // Compute how large the button needs to be.
                Vector2 size = style.CalcSize(content);

                GUI.Box(new Rect(e.mousePosition + Vector2.right * 20, size), tooltip);
            }
        }
    }
}