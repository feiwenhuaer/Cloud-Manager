using Cloud.GoogleDrive;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cloud
{
  public static class JsonSetting
  {
    //static List<Type> types_interface_cast = new List<Type>() { typeof(IDrive2_File),typeof(IDrive2_Files_list) };
    static List<Type> type_attribute_ignore = new List<Type>() { typeof(JsonIgnoreSerialize) };
    public static readonly JsonSerializerSettings _settings_serialize = new JsonSerializerSettings
    {
      TypeNameHandling = TypeNameHandling.None,//ignore field not found in class (serialize)
      NullValueHandling = NullValueHandling.Ignore,//ignore null (serialize)
      ContractResolver = new SerializeResolver(){ types_attribute_ignore = type_attribute_ignore }//custom ContractResolver
    };

    public static readonly JsonSerializerSettings _settings_deserialize = new JsonSerializerSettings
    {
      ContractResolver = new DeserializeResolver()//custom ContractResolver
    };
  }
  
  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
  public class JsonIgnoreSerialize : Attribute
  {
  }

  public class SerializeResolver : DefaultContractResolver
  {
    public List<Type> types_interface_cast { get; set; } = new List<Type>();
    public List<Type> types_attribute_ignore { get; set; } = new List<Type>();
    public SerializeResolver() { }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      JsonProperty property = base.CreateProperty(member, memberSerialization);
      if (types_attribute_ignore.Count != 0)
      {
        foreach (Type type in types_attribute_ignore)
        {
          object[] member_attribute = member.GetCustomAttributes(typeof(JsonIgnoreSerialize), true);
          if (member_attribute != null && member_attribute.Length > 0)
          {
            property.ShouldSerialize = Instance => { return false; };
            break;
          }
          else if (property != null && property.Writable)
          {
            IList<Attribute> attributes = property.AttributeProvider.GetAttributes(typeof(JsonIgnoreSerialize), true);
            if (attributes != null && attributes.Count > 0)
            {
              property.ShouldSerialize = Instance => { return false; };
              break;
            }
          }
        }
      }
      return property;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      Type found = null;
      if (types_interface_cast.Count != 0) foreach (Type Ti in type.GetInterfaces())
        {
          found = types_interface_cast.Find(t => t == Ti);//cast class to interface if found in types_interface
          if (found != null) break;
        }
      return found == null ? base.CreateProperties(type, memberSerialization) : base.CreateProperties(found, memberSerialization);
    }
  }

  public class DeserializeResolver: DefaultContractResolver
  {
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      JsonProperty property = base.CreateProperty(member, memberSerialization);
      var propInfo = member as PropertyInfo;
      if(propInfo!=null) property.Writable = propInfo.CanWrite;
      return property;
    }
  }
}