using litefeel;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class MyTest
{
    private static readonly string RootPath = "Assets/Plugins/BitmapFontImporter/Examples/";

    private static string Path(string filename)
    {
        return RootPath + filename;
    }

    [Test]
    public static void Test () {
        AssetDatabase.DeleteAsset(Path("Font1/font1.fontsettings"));
        AssetDatabase.DeleteAsset(Path("Font2/font2.fontsettings"));
        
        BFImporter.DoImportBitmapFont(Path("Font1/font1.fnt"));
        BFImporter.DoImportBitmapFont(Path("Font2/font2.fnt"));
    }

    [UnityTest]
    public static IEnumerator TestMuilTexture()
    {
        AssetDatabase.DeleteAsset(Path("font3/font3.fontsettings"));
        BFImporter.DoImportBitmapFont(Path("font3/font3.fnt"));

        var beginDate = DateTime.Now;
        var delta = new TimeSpan(0, 0, 0, 1);
        while (DateTime.Now - beginDate < delta)
            yield return null;

        var mat = AssetDatabase.LoadAssetAtPath<Material>(Path("font3/font3.fontsettings"));
        Assert.IsNotNull(mat, "Cannot generate FontMaterial for font3");
        Assert.AreEqual("BFI/Font2", mat.shader.name);

        var texture1 = AssetDatabase.LoadAssetAtPath<Texture2D>(Path("font3/font3_0.png"));
        var texture2 = AssetDatabase.LoadAssetAtPath<Texture2D>(Path("font3/font3_1.png"));
        Assert.IsNotNull(texture1, "Not found font3/font3_0.png");
        Assert.IsNotNull(texture2, "Not found font3/font3_1.png");
        Assert.AreSame(texture1, mat.GetTexture("_MainTex"), "Incorrect fontMaterial's _MainTex");
        Assert.AreSame(texture2, mat.GetTexture("_MainTex2"), "Incorrect fontMaterial's _MainTex2");
    }
}
