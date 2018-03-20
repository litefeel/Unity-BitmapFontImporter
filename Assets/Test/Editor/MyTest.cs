using litefeel;
using UnityEditor;

public class MyTest {

    // Use this for initialization

    //[InitializeOnLoadMethod]
    public static void Test () {
        AssetDatabase.DeleteAsset("Assets/Plugins/BitmapFontImporter/Examples/Font1/font1.fontsettings");
        AssetDatabase.DeleteAsset("Assets/Plugins/BitmapFontImporter/Examples/Font2/font2.fontsettings");

        BFImporter.DoImportBitmapFont("Assets/Plugins/BitmapFontImporter/Examples/Font1/font1.fnt");
        BFImporter.DoImportBitmapFont("Assets/Plugins/BitmapFontImporter/Examples/Font2/font2.fnt");
    }
}
