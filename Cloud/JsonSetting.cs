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
    static List<Type> types = new List<Type>() { typeof(IDrive2_File),typeof(IDrive2_Files_list) };
    public static readonly JsonSerializerSettings _settings_serialize = new JsonSerializerSettings
    {
      TypeNameHandling = TypeNameHandling.None,//ignore field not found in class (serialize)
      NullValueHandling = NullValueHandling.Ignore,//ignore null (serialize)
      ContractResolver = new JsonIgnoreSerializeResolver(types)//custom ContractResolver
    };
  }

  #region serialize and donot deserialize in tag [JsonIgnoreSerialize]
  [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
  public class JsonIgnoreSerialize : Attribute
  {
  }

  public class JsonIgnoreSerializeResolver : DefaultContractResolver
  {
    readonly List<Type> types = new List<Type>();
    public JsonIgnoreSerializeResolver() { }
    public JsonIgnoreSerializeResolver(List<Type> types) { this.types = types; }
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      JsonProperty property = base.CreateProperty(member, memberSerialization);

      #region deserialize and donot serialize in tag [JsonIgnoreSerialize]
      var member_attribute = member.GetCustomAttributes(typeof(JsonIgnoreSerialize), true);
      if ((property != null && property.Writable) | (member_attribute != null))
      {
        IList<Attribute> attributes = property.AttributeProvider.GetAttributes(typeof(JsonIgnoreSerialize), true);
        if ((attributes != null && attributes.Count > 0 ) |  member_attribute.Length > 0)
          property.ShouldSerialize = Instance => { return false; };
      }
      #endregion

      return property;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {  
      if (types.Count != 0)// find interface
      {
        Type found = null;
        foreach (Type Ti in type.GetInterfaces()) found = types.Find(t => t == Ti);
        if(found != null) return base.CreateProperties(found, memberSerialization);
      }
      return base.CreateProperties(type, memberSerialization);
    }
  }
  #endregion
}