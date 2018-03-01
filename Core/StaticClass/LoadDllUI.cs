using CloudManagerGeneralLib;
using System;
using System.IO;
using System.Reflection;

namespace Core.StaticClass
{
  public static class LoadDllUI
  {
    private static Assembly myLibrary;
    private static string Working_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
    public static bool Load()
    {
      try
      {
        string filename = AppSetting.settings.GetSettingsAsString(SettingsKey.UI_dll_file);
        myLibrary = Assembly.LoadFile(Working_dir + filename);
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
      foreach (Type type in GetExportedTypes())
      {
        if (TypeInterface.IsAssignableFrom(type)) return type;
      }
      return null;
    }
  }
}
