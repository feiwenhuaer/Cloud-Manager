using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud
{
  public static class Extensions
  {
    public static BigInteger FromMPINumber(this byte[] data)
    {
      // First 2 bytes defines the size of the component
      int dataLength = (data[0] * 256 + data[1] + 7) / 8;

      byte[] result = new byte[dataLength];
      Array.Copy(data, 2, result, 0, result.Length);

      return new BigInteger(result);
    }
    
  }
}
