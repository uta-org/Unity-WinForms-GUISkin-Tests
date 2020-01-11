using UnityEngine;
using System.IO;
using UnityEditor;

public static class DumpEditorTextures
{
    private const string AssetsFolder = "Assets";
    private const string TexturesDestFolderNamePro = "TexturesPro";
    private const string TexturesDestFolderNameNormal = "TexturesNormal";
    private static readonly string TexturesDestPathPro = Path.Combine(AssetsFolder, TexturesDestFolderNamePro);
    private static readonly string TexturesDestPathNormal = Path.Combine(AssetsFolder, TexturesDestFolderNameNormal);

    public class DumperWindow : EditorWindow
    {
        [MenuItem("Window/Editor Textures Dumper")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            DumperWindow window = (DumperWindow)GetWindow(typeof(DumperWindow));
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Dump Textures"))
            {
                CreateFolders();

                DumpAllTextures();
                AssetDatabase.Refresh();
            }
        }

        private static void CreateFolders()
        {
            if (!AssetDatabase.IsValidFolder(TexturesDestPathPro))
            {
                AssetDatabase.CreateFolder(AssetsFolder, TexturesDestFolderNamePro);
            }
            if (!AssetDatabase.IsValidFolder(TexturesDestPathNormal))
            {
                AssetDatabase.CreateFolder(AssetsFolder, TexturesDestFolderNameNormal);
            }
        }

        private void DumpAllTextures()
        {
            var path = EditorGUIUtility.isProSkin ? TexturesDestPathPro : TexturesDestPathNormal;

            SaveDefaultStyleTextures(path);

            foreach (var style in GUI.skin.customStyles)
            {
                SaveStyleTextures(style, path);
            }
        }

        private static void SaveStyleTextures(GUIStyle style, string path)
        {
            SaveTexture(style.normal.background, path);
            SaveTexture(style.hover.background, path);
            SaveTexture(style.active.background, path);
            SaveTexture(style.focused.background, path);

            SaveTexture(style.onNormal.background, path);
            SaveTexture(style.onHover.background, path);
            SaveTexture(style.onActive.background, path);
            SaveTexture(style.onFocused.background, path);
        }

        private void SaveDefaultStyleTextures(string path)
        {
            SaveStyleTextures(GUI.skin.box, path);
            SaveStyleTextures(GUI.skin.button, path);
            SaveStyleTextures(GUI.skin.toggle, path);
            SaveStyleTextures(GUI.skin.label, path);
            SaveStyleTextures(GUI.skin.textArea, path);
            SaveStyleTextures(GUI.skin.textField, path);
            SaveStyleTextures(GUI.skin.window, path);

            SaveStyleTextures(GUI.skin.horizontalSlider, path);
            SaveStyleTextures(GUI.skin.horizontalSliderThumb, path);

            SaveStyleTextures(GUI.skin.verticalSlider, path);
            SaveStyleTextures(GUI.skin.verticalSliderThumb, path);

            SaveStyleTextures(GUI.skin.horizontalScrollbar, path);
            SaveStyleTextures(GUI.skin.horizontalScrollbarThumb, path);
            SaveStyleTextures(GUI.skin.horizontalScrollbarLeftButton, path);
            SaveStyleTextures(GUI.skin.horizontalScrollbarRightButton, path);

            SaveStyleTextures(GUI.skin.verticalScrollbar, path);
            SaveStyleTextures(GUI.skin.verticalScrollbarThumb, path);
            SaveStyleTextures(GUI.skin.verticalScrollbarUpButton, path);
            SaveStyleTextures(GUI.skin.verticalScrollbarDownButton, path);

            SaveStyleTextures(GUI.skin.scrollView, path);
        }
    }

    // Credits: https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-
    public static void SaveTexture(Texture2D tex, string path)
    {
        if (tex == null)
        {
            return;
        }

        // Create a temporary RenderTexture of the same size as the texture
        RenderTexture tmp = RenderTexture.GetTemporary(
            tex.width,
            tex.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        // Blit the pixels on texture to the RenderTexture
        Graphics.Blit(tex, tmp);

        // Backup the currently set RenderTexture
        RenderTexture previous = RenderTexture.active;

        // Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tmp;

        // Create a new readable Texture2D to copy the pixels to it
        Texture2D toSave = new Texture2D(tex.width, tex.height);
        // Copy the pixels from the RenderTexture to the new Texture
        toSave.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        toSave.Apply();

        // Reset the active RenderTexture
        RenderTexture.active = previous;

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(tmp);

        byte[] bytes = toSave.EncodeToPNG();
        var fileName = string.Format("{0}-{1}.png", tex.name, tex.GetInstanceID());
        var filePath = Path.Combine(path, fileName);
        File.WriteAllBytes(filePath, bytes);
    }
}