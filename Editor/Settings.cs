using UnityEditor;

namespace litefeel.BFImporter.Editor
{
    public enum TextureType
    {
        GUI,
        Texture,
        Sprite,
    }
    public static class Settings
    {
        private const string TextureTypeKey = "litefeel.BFI.TextureType";

        [InitializeOnLoadMethod]
        private static void Init()
        {
            _TextureType = (TextureType)EditorPrefs.GetInt(TextureTypeKey, 0);
        }

        private static TextureType _TextureType;
        public static TextureType TextureType
        {
            get { return _TextureType; }
            set
            {
                if (value != _TextureType)
                {
                    _TextureType = value;
                    EditorPrefs.SetInt(TextureTypeKey, (int)value);
                }
            }
        }

        public static TextureImporterType TextureImporterType
        {
            get
            {
                switch (_TextureType)
                {
                    case TextureType.GUI: return TextureImporterType.GUI;
                    case TextureType.Texture: return TextureImporterType.Default;
                    case TextureType.Sprite: return TextureImporterType.Sprite;
                    default: return TextureImporterType.GUI;
                }
            }
        }
    }
}
