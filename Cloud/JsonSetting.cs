using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Cloud
{
  public static class JsonSetting
  {

    public static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
    {
      TypeNameHandling = TypeNameHandling.None,//ignore field not found in class (serialize)
      NullValueHandling = NullValueHandling.Ignore,//ignore null (serialize)
      ContractResolver = new GetOnlyContractResolver()//serialize and donot deserialize in tag [GetOnlyJsonProperty]
    };
  }

  [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
  public class GetOnlyJsonPropertyAttribute : Attribute
  {
  }
  public class GetOnlyContractResolver : DefaultContractResolver
  {
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      var property = base.CreateProperty(member, memberSerialization);
      if (property != null && property.Writable)
      {
        var attributes = property.AttributeProvider.GetAttributes(typeof(GetOnlyJsonPropertyAttribute), true);
        if (attributes != null && attributes.Count > 0)
          property.Writable = false;
      }
      return property;
    }
  }
}