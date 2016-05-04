using UnityEngine;
using System.Xml;
using System;
using System.Collections.Generic;

namespace litefeel
{
    public class FntParse
    {
        public int textureWidth;
        public int textureHeight;
        public string textureName;

        public string fontName;
        public int fontSize;
        public int lineHeight;
        public int lineBaseHeight;

        public CharacterInfo[] charInfos { get; private set; }

        public static FntParse GetFntParse(ref string text)
        {
            FntParse parse = null;
            if (text.StartsWith("info"))
            {
                parse = new FntParse();
                parse.DoTextParse(ref text);
            }
            else if (text.StartsWith("<"))
            {
                parse = new FntParse();
                parse.DoXMLPase(ref text);
            }
            return parse;
        }

        #region xml
        public void DoXMLPase(ref string content)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(content);

            XmlNode info = xml.GetElementsByTagName("info")[0];
            XmlNode common = xml.GetElementsByTagName("common")[0];
            XmlNode page = xml.GetElementsByTagName("pages")[0].FirstChild;
            XmlNodeList chars = xml.GetElementsByTagName("chars")[0].ChildNodes;

            fontName = info.Attributes.GetNamedItem("face").InnerText;
            fontSize = ToInt(info, "size");

            lineHeight = ToInt(common, "lineHeight");
            lineBaseHeight = ToInt(common, "base");
            textureWidth = ToInt(common, "scaleW");
            textureHeight = ToInt(common, "scaleH");
            textureName = page.Attributes.GetNamedItem("file").InnerText;

            charInfos = new CharacterInfo[chars.Count];
            for (int i = 0; i < chars.Count; i++)
            {
                XmlNode charNode = chars[i];
                charInfos[i] =  CreateCharInfo(
                    ToInt(charNode, "id"),
                    ToInt(charNode, "x"),
                    ToInt(charNode, "y"),
                    ToInt(charNode, "width"),
                    ToInt(charNode, "height"),
                    ToInt(charNode, "xoffset"),
                    ToInt(charNode, "yoffset"),
                    ToInt(charNode, "xadvance"));
            }
        }


        private static int ToInt(XmlNode node, string name)
        {
            return int.Parse(node.Attributes.GetNamedItem(name).InnerText);
        }
        #endregion

        #region text
        public void DoTextParse(ref string content)
        {
            string[] lines = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            ReadTextInfo(ref lines[0]);
            ReadTextCommon(ref lines[1]);
            ReadTextPage(ref lines[2]);
            // don't use count of chars, count is incorrect if has space 
            //ReadTextCharCount(ref lines[3]);
            List<CharacterInfo> list = new List<CharacterInfo>();
            for (int i = 4, l = lines.Length; i < l; i++)
            {
                if (!ReadTextChar(i - 4, ref lines[i], ref list))
                    break;
            }
            charInfos = list.ToArray();
        }

        private void ReadTextInfo(ref string line)
        {
            string[] keys;
            string[] values;
            SplitParts(line, out keys, out values);
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                switch (keys[i])
                {
                    case "face": fontName = values[i].Trim('"'); break;
                    case "size": fontSize = int.Parse(values[i]); break;
                }
            }
        }

        private void ReadTextCommon(ref string line)
        {
            string[] keys;
            string[] values;
            SplitParts(line, out keys, out values);
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                switch (keys[i])
                {
                    case "lineHeight": lineHeight = int.Parse(values[i]); break;
                    case "base": lineBaseHeight = int.Parse(values[i]); break;
                    case "scaleW": textureWidth = int.Parse(values[i]); break;
                    case "scaleH": textureHeight = int.Parse(values[i]); break;
                }
            }
        }

        private void ReadTextPage(ref string line)
        {
            string[] keys;
            string[] values;
            SplitParts(line, out keys, out values);
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                switch (keys[i])
                {
                    case "file": textureName = values[i].Trim('"'); break;
                }
            }
        }

        private void ReadTextCharCount(ref string line)
        {
            string[] keys;
            string[] values;
            SplitParts(line, out keys, out values);
            int count = 0;
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                switch (keys[i])
                {
                    case "count": count = int.Parse(values[i]); break;
                }
            }
            charInfos = new CharacterInfo[count];
        }

        private bool ReadTextChar(int idx, ref string line, ref List<CharacterInfo> list)
        {
            if (!line.StartsWith("char")) return false;
            string[] keys;
            string[] values;
            SplitParts(line, out keys, out values);
            int id = 0, x = 0, y = 0, w = 0, h = 0, xo = 0, yo = 0, xadvance = 0;
            for (int i = keys.Length - 1; i >= 0; i--)
            {
                switch (keys[i])
                {
                    case "id": id = int.Parse(values[i]); break;
                    case "x": x = int.Parse(values[i]); break;
                    case "y": y = int.Parse(values[i]); break;
                    case "width": w = int.Parse(values[i]); break;
                    case "height": h = int.Parse(values[i]); break;
                    case "xoffset": xo = int.Parse(values[i]); break;
                    case "yoffset": yo = int.Parse(values[i]); break;
                    case "xadvance": xadvance = int.Parse(values[i]); break;
                }
            }
            list.Add(CreateCharInfo(id, x, y, w, h, xo, yo, xadvance));
            return true;
        }

        private bool SplitParts(string line, out string[] keys, out string[] values)
        {
            string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            keys = new string[parts.Length - 1];
            values = new string[parts.Length - 1];
            for (int i = parts.Length - 2; i >= 0; i--)
            {
                string part = parts[i + 1];
                int pos = part.IndexOf('=');
                keys[i] = part.Substring(0, pos);
                values[i] = part.Substring(pos + 1);
            }
            return true;
        }

        #endregion

        private CharacterInfo CreateCharInfo(int id, int x, int y, int w, int h, int xo, int yo, int xadvance)
        {
            CharacterInfo charInfo = new CharacterInfo();
            Rect r = new Rect();
            r.x = (float)x / textureWidth;
            r.y = (float)y / textureHeight;
            r.width = (float)w / textureWidth;
            r.height = (float)h / textureHeight;
            r.y = 1f - r.y - r.height;
            charInfo.uvBottomLeft = new Vector2(r.xMin, r.yMin);
            charInfo.uvBottomRight = new Vector2(r.xMax, r.yMin);
            charInfo.uvTopLeft = new Vector2(r.xMin, r.yMax);
            charInfo.uvTopRight = new Vector2(r.xMax, r.yMax);

            r.x = xo;
            r.y = yo;
            r.width = w;
            r.height = h;
            r.y = -r.y;
            r.height = -r.height;
            charInfo.minX = (int)r.xMin;
            charInfo.maxX = (int)r.xMax;
            charInfo.minY = (int)r.yMax;
            charInfo.maxY = (int)r.yMin;

            charInfo.bearing = (int)r.x;
            charInfo.index = id;
            charInfo.advance = xadvance;

            return charInfo;
        }
    }

}
