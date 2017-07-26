using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Cloud
{
  public static class JsonSetting
  {
    public static readonly JsonSerializerSettings _settings_serialize = new JsonSerializerSettings
    {
      TypeNameHandling = TypeNameHandling.None,//ignore field not found in class (serialize)
      NullValueHandling = NullValueHandling.Ignore,//ignore null (serialize)
      //ReferenceResolver = new IgnoreJsonSerializeReferenceResolver()
      //ContractResolver = new GetOnlyContractResolver()//serialize and donot deserialize in tag [GetOnlyJsonProperty]
    };
  }

  #region serialize and donot deserialize in tag [GetOnlyJsonProperty]
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
  #endregion

  [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
  public class IgnoreJsonSerialize: Attribute
  {

  }

  public class IgnoreJsonSerializeReferenceResolver : IReferenceResolver
  {
    public void AddReference(object context, string reference, object value)
    {
      throw new NotImplementedException();
    }

    public string GetReference(object context, object value)
    {
      throw new NotImplementedException();
    }

    public bool IsReferenced(object context, object value)
    {
      throw new NotImplementedException();
    }

    public object ResolveReference(object context, string reference)
    {
      throw new NotImplementedException();
    }
  }
}