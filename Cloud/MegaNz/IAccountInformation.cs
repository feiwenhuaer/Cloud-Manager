namespace Cloud.MegaNz
{
  public interface IAccountInformation
  {
    long TotalQuota { get; }

    long UsedQuota { get; }
  }
}