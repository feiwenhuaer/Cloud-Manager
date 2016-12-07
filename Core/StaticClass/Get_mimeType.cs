using System.Xml;

namespace Core.StaticClass
{
    public static class Get_mimeType
    {
        private static XmlDocument mimetypelist;
        public static void Load()
        {
            mimetypelist = new XmlDocument();
            mimetypelist.LoadXml(global::Core.Properties.Resources.mimetypelist);
        }
        public static string Get_mimeType_From_FileExtension(string ExtensionName)
        {
            if (ExtensionName[0] != '.') ExtensionName = "." + ExtensionName;
            foreach (XmlNode node in mimetypelist.DocumentElement.ChildNodes)
            {
                if (node.Attributes["FileExtension"].Value == ExtensionName) return node.Attributes["Name"].Value;
            }
            return "application/octet-stream";
        }
    }
}
