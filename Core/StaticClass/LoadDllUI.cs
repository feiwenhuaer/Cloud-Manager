using CloudManagerGeneralLib;
using System;
using System.IO;
using System.Reflection;

namespace Core.StaticClass
{
    public static class LoadDllUI
    {
        private static Assembly myLibrary;
        public static bool Load()
        {
            try
            {
                myLibrary = Assembly.LoadFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + AppSetting.settings.GetSettingsAsString(SettingsKey.UI_dll_file));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static Type[] GetExportedTypes()
        {
            return myLibrary.GetExportedTypes();
        }

        public static Type GetTypeInterface(Type TypeInterface)
        {
            foreach(Type type in GetExportedTypes())
            {
                if (TypeInterface.IsAssignableFrom(type)) return type;
            }
            return null;
        }
    }
}
