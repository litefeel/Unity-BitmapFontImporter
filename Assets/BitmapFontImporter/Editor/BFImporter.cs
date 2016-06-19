using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

namespace litefeel
{
    public class BFImporter : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                //Debug.Log("Reimported Asset: " + str);
                DoImportBitmapFont(str);
            }
            foreach (string str in deletedAssets)
            {
                //Debug.Log("Deleted Asset: " + str);
                DelBitmapFont(str);
            }

            for (var i = 0; i < movedAssets.Length; i++)
            {
                //Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
                MoveBitmapFont(movedFromAssetPaths[i], movedAssets[i]);
            }
        }

        public static bool IsFnt(string path)
        {
            return path.EndsWith(".fnt", StringComparison.OrdinalIgnoreCase);
        }

        public static void DoImportBitmapFont(string fntPatn)
        {
            if (!IsFnt(fntPatn)) return;

            TextAsset fnt = AssetDatabase.LoadMainAssetAtPath(fntPatn) as TextAsset;
            string text = fnt.text;
            FntParse parse = FntParse.GetFntParse(ref text);
            if (parse == null) return;

            string fntName = Path.GetFileNameWithoutExtension(fntPatn);
            string rootPath = Path.GetDirectoryName(fntPatn);
            string fontPath = string.Format("{0}/{1}.fontsettings", rootPath, fntName);
            string texPath = string.Format("{0}/{1}", rootPath, parse.textureName);

            Font font = AssetDatabase.LoadMainAssetAtPath(fontPath) as Font;
            if (font == null)
            {
                font = new Font();
                AssetDatabase.CreateAsset(font, fontPath);
                font.material = new Material(Shader.Find("UI/Default"));
                font.material.name = "Font Material";
                AssetDatabase.AddObjectToAsset(font.material, font);
            }
            
            SerializedObject so = new SerializedObject(font);
            so.Update();
            so.FindProperty("m_FontSize").floatValue = parse.fontSize;
            so.FindProperty("m_LineSpacing").floatValue = parse.lineHeight;
            SerializedProperty kerningsProp = so.FindProperty("m_KerningValues");
            Debug.Log("arraySize " + kerningsProp.arraySize);
            // Clear kernings info, reset kernings on end.
            kerningsProp.ClearArray();

            so.ApplyModifiedProperties();
            so.SetIsDifferentCacheDirty();


            Texture2D texture = AssetDatabase.LoadMainAssetAtPath(texPath) as Texture2D;
            if (texture == null)
            {
                Debug.LogErrorFormat(fnt, "{0}: not found '{1}'.", typeof(BFImporter), texPath);
                return;
            }

            TextureImporter texImporter = AssetImporter.GetAtPath(texPath) as TextureImporter;
            texImporter.textureType = TextureImporterType.GUI;
            texImporter.mipmapEnabled = false;
            texImporter.SaveAndReimport();

            font.material.mainTexture = texture;
            font.material.mainTexture.name = "Font Texture";
            
            font.characterInfo = parse.charInfos;
            
            AssetDatabase.SaveAssets();

            // reset kernings by modify *.fontsettings file.
            string kerningValues = GetKerningsStr(parse.kernings);
            if (kerningValues != null)
            {
                string fontText = File.ReadAllText(fontPath);
                fontText = fontText.Replace("m_KerningValues: []", kerningValues);
                File.WriteAllText(fontPath, fontText);
                //AssetDatabase.ImportAsset(fontText, ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            
        }

        private static string GetKerningsStr(Kerning[] kernings)
        {
            if (kernings == null || kernings.Length == 0)
                return null;

            const string format = "  - first:\n      {0}: {1}\n    second: {2}\n";
            StringBuilder builder = new StringBuilder("m_KerningValues:\n", kernings.Length * 20);
            for (int i = 0; i < kernings.Length; i++)
            {
                builder.AppendFormat(format, kernings[i].first, kernings[i].second, kernings[i].amount);
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }


        private static void DelBitmapFont(string fntPath)
        {
            if (!IsFnt(fntPath)) return;

            string fontPath = fntPath.Substring(0, fntPath.Length - 4) + ".fontsettings";
            AssetDatabase.DeleteAsset(fontPath);
        }

        private static void MoveBitmapFont(string oldFntPath, string nowFntPath)
        {
            if (!IsFnt(nowFntPath)) return;

            string oldFontPath = oldFntPath.Substring(0, oldFntPath.Length - 4) + ".fontsettings";
            string nowFontPath = nowFntPath.Substring(0, nowFntPath.Length - 4) + ".fontsettings";
            AssetDatabase.MoveAsset(oldFontPath, nowFontPath);
        }
    }

}

