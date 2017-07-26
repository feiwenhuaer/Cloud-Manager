using System.IO;
namespace Cloud.GoogleDrive
{
  public interface IRequestReturn
  {
    string HeaderResponse { get; }
    string DataTextResponse { get; }
    Stream stream { get; }
    T GetObjectResponse<T>();
  }
}
