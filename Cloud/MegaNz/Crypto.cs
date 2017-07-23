using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cloud.MegaNz
{
  public static class Crypto
  {
    private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

    /// <summary>
    /// Encrypt the given string using AES.  The string can be decrypted using 
    /// DecryptStringAES().  The sharedSecret parameters must match.
    /// </summary>
    /// <param name="plainText">The text to encrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
    public static string EncryptStringAES(string plainText, string sharedSecret)
    {
      if (string.IsNullOrEmpty(plainText))
        throw new ArgumentNullException("plainText");
      if (string.IsNullOrEmpty(sharedSecret))
        throw new ArgumentNullException("sharedSecret");

      string outStr = null;                       // Encrypted string to return
      RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

      try
      {
        // generate the key from the shared secret and the salt
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

        // Create a RijndaelManaged object
        aesAlg = new RijndaelManaged();
        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

        // Create a decryptor to perform the stream transform.
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for encryption.
        using (MemoryStream msEncrypt = new MemoryStream())
        {
          // prepend the IV
          msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
          msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
          using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
              //Write all data to the stream.
              swEncrypt.Write(plainText);
            }
          }
          outStr = Convert.ToBase64String(msEncrypt.ToArray());
        }
      }
      finally
      {
        // Clear the RijndaelManaged object.
        if (aesAlg != null)
          aesAlg.Clear();
      }

      // Return the encrypted bytes from the memory stream.
      return outStr;
    }

    /// <summary>
    /// Decrypt the given string.  Assumes the string was encrypted using 
    /// EncryptStringAES(), using an identical sharedSecret.
    /// </summary>
    /// <param name="cipherText">The text to decrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
    public static string DecryptStringAES(string cipherText, string sharedSecret)
    {
      if (string.IsNullOrEmpty(cipherText))
        throw new ArgumentNullException("cipherText");
      if (string.IsNullOrEmpty(sharedSecret))
        throw new ArgumentNullException("sharedSecret");

      // Declare the RijndaelManaged object
      // used to decrypt the data.
      RijndaelManaged aesAlg = null;

      // Declare the string used to hold
      // the decrypted text.
      string plaintext = null;

      try
      {
        // generate the key from the shared secret and the salt
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

        // Create the streams used for decryption.                
        byte[] bytes = Convert.FromBase64String(cipherText);
        using (MemoryStream msDecrypt = new MemoryStream(bytes))
        {
          // Create a RijndaelManaged object
          // with the specified key and IV.
          aesAlg = new RijndaelManaged();
          aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
          // Get the initialization vector from the encrypted stream
          aesAlg.IV = ReadByteArray(msDecrypt);
          // Create a decrytor to perform the stream transform.
          ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
          using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))

              // Read the decrypted bytes from the decrypting stream
              // and place them in a string.
              plaintext = srDecrypt.ReadToEnd();
          }
        }
      }
      finally
      {
        // Clear the RijndaelManaged object.
        if (aesAlg != null)
          aesAlg.Clear();
      }

      return plaintext;
    }

    private static byte[] ReadByteArray(Stream s)
    {
      byte[] rawLength = new byte[sizeof(int)];
      if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
      {
        throw new SystemException("Stream did not contain properly formatted byte array");
      }

      byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
      if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
      {
        throw new SystemException("Did not read byte array properly");
      }

      return buffer;
    }







    private static readonly Rijndael RijndaelCbc;
    private static readonly byte[] DefaultIv = new byte[16];

    static Crypto()
    {
      RijndaelCbc = Rijndael.Create();
      RijndaelCbc.Padding = PaddingMode.None;
      RijndaelCbc.Mode = CipherMode.CBC;
    }

    #region Key

    public static byte[] DecryptKey(byte[] data, byte[] key)
    {
      byte[] result = new byte[data.Length];

      for (int idx = 0; idx < data.Length; idx += 16)
      {
        byte[] block = data.CopySubArray(16, idx);
        byte[] decryptedBlock = DecryptAes(block, key);
        Array.Copy(decryptedBlock, 0, result, idx, 16);
      }

      return result;
    }

    public static byte[] EncryptKey(byte[] data, byte[] key)
    {
      byte[] result = new byte[data.Length];

      for (int idx = 0; idx < data.Length; idx += 16)
      {
        byte[] block = data.CopySubArray(16, idx);
        byte[] encryptedBlock = EncryptAes(block, key);
        Array.Copy(encryptedBlock, 0, result, idx, 16);
      }

      return result;
    }

    public static void GetPartsFromDecryptedKey(byte[] decryptedKey, out byte[] iv, out byte[] metaMac, out byte[] fileKey)
    {
      // Extract Iv and MetaMac
      iv = new byte[8];
      metaMac = new byte[8];
      Array.Copy(decryptedKey, 16, iv, 0, 8);
      Array.Copy(decryptedKey, 24, metaMac, 0, 8);

      // For files, key is 256 bits long. Compute the key to retrieve 128 AES key
      fileKey = new byte[16];
      for (int idx = 0; idx < 16; idx++)
      {
        fileKey[idx] = (byte)(decryptedKey[idx] ^ decryptedKey[idx + 16]);
      }
    }

    #endregion

    #region Aes

    public static byte[] DecryptAes(byte[] data, byte[] key)
    {
      using (ICryptoTransform decryptor = RijndaelCbc.CreateDecryptor(key, DefaultIv))
      {
        return decryptor.TransformFinalBlock(data, 0, data.Length);
      }
    }

    public static byte[] EncryptAes(byte[] data, byte[] key)
    {
      using (ICryptoTransform encryptor = RijndaelCbc.CreateEncryptor(key, DefaultIv))
      {
        return encryptor.TransformFinalBlock(data, 0, data.Length);
      }
    }

    public static byte[] CreateAesKey()
    {
      using (Rijndael rijndael = Rijndael.Create())
      {
        rijndael.Mode = CipherMode.CBC;
        rijndael.KeySize = 128;
        rijndael.Padding = PaddingMode.None;
        rijndael.GenerateKey();
        return rijndael.Key;
      }
    }

    #endregion

    #region Attributes

    public static byte[] EncryptAttributes(Attributes attributes, byte[] nodeKey)
    {
      string data = "MEGA" + JsonConvert.SerializeObject(attributes, Formatting.None);
      byte[] dataBytes = data.ToBytes();
      dataBytes = dataBytes.CopySubArray(dataBytes.Length + 16 - (dataBytes.Length % 16));

      return EncryptAes(dataBytes, nodeKey);
    }

    public static Attributes DecryptAttributes(byte[] attributes, byte[] nodeKey)
    {
      byte[] decryptedAttributes = DecryptAes(attributes, nodeKey);

      // Remove MEGA prefix
      try
      {
        string json = decryptedAttributes.ToUTF8String().Substring(4);
        int nullTerminationIndex = json.IndexOf('\0');
        if (nullTerminationIndex != -1)
        {
          json = json.Substring(0, nullTerminationIndex);
        }

        return JsonConvert.DeserializeObject<Attributes>(json);
      }
      catch (Exception ex)
      {
        return new Attributes(string.Format("Attribute deserialization failed: {0}", ex.Message));
      }
    }

    #endregion

    #region Rsa

    public static BigInteger[] GetRsaPrivateKeyComponents(byte[] encodedRsaPrivateKey, byte[] masterKey)
    {
      // We need to add padding to obtain multiple of 16
      encodedRsaPrivateKey = encodedRsaPrivateKey.CopySubArray(encodedRsaPrivateKey.Length + (16 - encodedRsaPrivateKey.Length % 16));
      byte[] rsaPrivateKey = DecryptKey(encodedRsaPrivateKey, masterKey);

      // rsaPrivateKeyComponents[0] => First factor p
      // rsaPrivateKeyComponents[1] => Second factor q
      // rsaPrivateKeyComponents[2] => Private exponent d
      BigInteger[] rsaPrivateKeyComponents = new BigInteger[4];
      for (int i = 0; i < 4; i++)
      {
        rsaPrivateKeyComponents[i] = rsaPrivateKey.FromMPINumber();

        // Remove already retrieved part
        int dataLength = ((rsaPrivateKey[0] * 256 + rsaPrivateKey[1] + 7) / 8);
        rsaPrivateKey = rsaPrivateKey.CopySubArray(rsaPrivateKey.Length - dataLength - 2, dataLength + 2);
      }

      return rsaPrivateKeyComponents;
    }

    public static byte[] RsaDecrypt(BigInteger data, BigInteger p, BigInteger q, BigInteger d)
    {
      return data.modPow(d, p * q).getBytes();
    }

    #endregion
  }
}
