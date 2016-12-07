using SupDataDll;
using System;
using System.IO;
using System.Reflection;

namespace Core.StaticClass
{
    public static class LoadDllUI
    {
        private static Assembly myLibrary;
        public static void Load()
        {
            myLibrary = Assembly.LoadFile(Directory.GetCurrentDirectory() + "\\" + AppSetting.settings.GetSettingsAsString(SettingsKey.UI_dll_file));
        }

        public static Type[] GetExportedTypes()
        {
            return myLibrary.GetExportedTypes();
        }

        public static Type GetTypeClassName(string name)
        {
            foreach (Type t in GetExportedTypes())
            {
                if (t.Name == name) return t;
            }
            return null;
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
