using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace litefeel
{
    class BFImporter : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                //Debug.Log("Reimported Asset: " + str);
                DoImportBitmapFont(str);
            }
            //         foreach (string str in deletedAssets)
            //         {
            //             Debug.Log("Deleted Asset: " + str);
            //         }

            //         for (var i = 0; i < movedAssets.Length; i++)
            //         {
            //             Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
            //         }
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
            so.FindProperty("m_FontSize").floatValue = parse.fontSize;
            so.FindProperty("m_LineSpacing").floatValue = parse.lineBaseHeight;
            so.ApplyModifiedProperties();

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

            /*XmlNode kernings = xml.GetNode("kernings");
            int kerningsCount = kernings.GetInt("count");
            if (kerningsCount > 0)
            {
                SerializedProperty kerningsProp = so.FindProperty("m_KerningValues");
                for (int i = 0; i < kerningsCount; i++)
                {
                    kerningsProp.InsertArrayElementAtIndex(i);
                    XmlNode kerning = kernings.ChildNodes[i];
                    SerializedProperty kern = kerningsProp.GetArrayElementAtIndex(i);
                    kern.FindPropertyRelative("second").floatValue = kerning.GetFloat("amount");
                }
            }*/

            AssetDatabase.SaveAssets();
        }
    }

}

