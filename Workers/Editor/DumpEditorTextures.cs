using System;
using UnityEngine;
using System.IO;
using UnityEditor;

public static class DumpEditorTextures
{
    private const string AssetsFolder = "Assets";
    private const string TexturesDestFolderNamePro = "TexturesPro";
    private const string TexturesDestFolderNameNormal = "TexturesNormal";
    private const string ResourcesSubfolder = "Resources/Dumped Textures";

    private static string TexturesDestPathPro { get; } = Path.Combine(AssetsFolder, "{0}", TexturesDestFolderNamePro);
    private static string TexturesDestPathNormal { get; } = Path.Combine(AssetsFolder, "{0}", TexturesDestFolderNameNormal);

    private static void CreateFolders(EditorSkin skin)
    {
        string skinName = Path.Combine(ResourcesSubfolder, skin.ToString());
        if (EditorGUIUtility.isProSkin)
        {
            var proPath = string.Format(TexturesDestPathPro, skinName);
            if (!AssetDatabase.IsValidFolder(proPath))
                Directory.CreateDirectory(Path.Combine(AssetsFolder, skinName, TexturesDestFolderNamePro));
        }
        else
        {
            var normalPath = string.Format(TexturesDestPathNormal, skinName);
            if (!AssetDatabase.IsValidFolder(normalPath))
                Directory.CreateDirectory(Path.Combine(AssetsFolder, skinName, TexturesDestFolderNameNormal));
        }
    }

    [MenuItem("Window/Dump all EditorSkin textures...")]
    private static void DumpAllTextures()
    {
        Array values = Enum.GetValues(typeof(EditorSkin));
        foreach (EditorSkin editorSkin in values)
        {
            var path = string.Format(EditorGUIUtility.isProSkin ? TexturesDestPathPro : TexturesDestPathNormal, Path.Combine(ResourcesSubfolder, editorSkin.ToString()));

            GUISkin skin = UnityEngine.Object.Instantiate(EditorGUIUtility.GetBuiltinSkin(editorSkin));

            CreateFolders(editorSkin);
            SaveDefaultStyleTextures(skin, path);

            foreach (var style in skin.customStyles)
            {
                SaveStyleTextures(style, path);
            }
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

    private static void SaveDefaultStyleTextures(GUISkin skin, string path)
    {
        SaveStyleTextures(skin.box, path);
        SaveStyleTextures(skin.button, path);
        SaveStyleTextures(skin.toggle, path);
        SaveStyleTextures(skin.label, path);
        SaveStyleTextures(skin.textArea, path);
        SaveStyleTextures(skin.textField, path);
        SaveStyleTextures(skin.window, path);

        SaveStyleTextures(skin.horizontalSlider, path);
        SaveStyleTextures(skin.horizontalSliderThumb, path);

        SaveStyleTextures(skin.verticalSlider, path);
        SaveStyleTextures(skin.verticalSliderThumb, path);

        SaveStyleTextures(skin.horizontalScrollbar, path);
        SaveStyleTextures(skin.horizontalScrollbarThumb, path);
        SaveStyleTextures(skin.horizontalScrollbarLeftButton, path);
        SaveStyleTextures(skin.horizontalScrollbarRightButton, path);

        SaveStyleTextures(skin.verticalScrollbar, path);
        SaveStyleTextures(skin.verticalScrollbarThumb, path);
        SaveStyleTextures(skin.verticalScrollbarUpButton, path);
        SaveStyleTextures(skin.verticalScrollbarDownButton, path);

        SaveStyleTextures(skin.scrollView, path);
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