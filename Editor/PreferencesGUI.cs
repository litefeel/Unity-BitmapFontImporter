using UnityEditor;

namespace litefeel.BFImporter.Editor
{
    public static class PreferencesGUI
    {

#if UNITY_2018_3_OR_NEWER
        private class MyPrefSettingsProvider : SettingsProvider
        {
            public MyPrefSettingsProvider(string path, SettingsScope scopes = SettingsScope.User)
            : base(path, scopes)
            { }

            public override void OnGUI(string searchContext)
            {
                PreferencesGUI.OnGUI();
            }
        }

        [SettingsProvider]
        static SettingsProvider NewPreferenceItem()
        {
            return new MyPrefSettingsProvider("Preferences/Bitmap Font Importer");
        }
#else
        [PreferenceItem("Bitmap Font Importer")]
#endif
        public static void OnGUI()
        {
            EditorGUILayout.LabelField("Bitmap font texture type by default");
            Settings.TextureType = (TextureType)EditorGUILayout.EnumPopup("Type", Settings.TextureType);
        }
    }
}