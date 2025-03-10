using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Web;
using System.Globalization;

namespace Application.Services
{
    public class CryptoService : ICryptoService
    {
        readonly string hashKey = "joblastHashBigGame123HashKey";
        public string mySecret = "fat1hala3urfat1hala3urfat1hala3ur1";
        string IV = "";

        public CryptoService()
        {
            IV = InitSymmetricEncryptionKeyIV();
        }




        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new();
            Random rnd = new();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // Create hash using password salt.
                for (int i = 0; i < computedHash.Length; i++)
                { // Loop through the byte array
                    if (computedHash[i] != passwordHash[i]) return false; // if mismatch
                }
            }
            return true; //if no mismatches.
        }
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public string Encrypt(string toEncrypt, bool useHashing)
        {
            string str = "";
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

                //If hashing use get hashcode regards to your key
                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new();
                    keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(hashKey));
                    //Always release the resources and flush data
                    // of the Cryptographic service provide. Best Practice

                    hashmd5.Clear();
                }
                else
                    keyArray = Encoding.UTF8.GetBytes(hashKey);

                TripleDESCryptoServiceProvider tdes = new();
                //set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;
                //mode of operation. there are other 4 modes.
                //We choose ECB(Electronic code Book)
                tdes.Mode = CipherMode.ECB;
                //padding mode(if any extra byte added)

                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                //transform the specified region of bytes array to resultArray
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                //Release resources held by TripleDes Encryptor
                tdes.Clear();
                //Return the encrypted data into unreadable string format
                str = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch { }
            return str;
        }

        public string Decrypt(string cipherString, bool useHashing)
        {
            string str = "";
            try
            {
                byte[] keyArray;

                var cipherStringlocal = HttpUtility.UrlDecode(cipherString); //Uri.EscapeUriString(cipherString); //karakterleri düzenliyor(%2f to /)
                cipherStringlocal = cipherStringlocal.PadRight(cipherStringlocal.Length + (4 - cipherStringlocal.Length % 4) % 4, '='); //base64 te eksik olan == leri atıyor.

                //get byte of string
                byte[] toEncryptArray = Convert.FromBase64String(cipherStringlocal.Replace(" ", "+"));

                if (useHashing)
                {
                    //if hashing was used get the hash code with regards to your key
                    MD5CryptoServiceProvider hashmd5 = new();
                    keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(hashKey));
                    //release any resource held by the MD5CryptoServiceProvider

                    hashmd5.Clear();
                }
                else
                {
                    //if hashing was not implemented get the byte code of the key
                    keyArray = Encoding.UTF8.GetBytes(hashKey);
                }

                TripleDESCryptoServiceProvider tdes = new();
                //set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;
                //mode of operation. there are other 4 modes.
                //We choose ECB(Electronic code Book)

                tdes.Mode = CipherMode.ECB;
                //padding mode(if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;

                System.Security.Cryptography.ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(
                    toEncryptArray, 0, toEncryptArray.Length);
                //Release resources held by TripleDes Encryptor
                tdes.Clear();
                //return the Clear decrypted TEXT
                str = Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {

            }
            return str;
        }


        //
       
        public string GenerateEncryptedId()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Guid.NewGuid().ToString());
            sb.Append("-").Append(DateTime.Now.ToString("yyyyMMddHHmm"));
            var enc = Encrypt(sb.ToString());
            return enc;
        }
        public Guid GetIdFromEncrypted(string enc)
        {
            string tempId = Decrypt(enc);
            if (tempId.Length != 49)
                throw new Exception();
            var Id = tempId.Substring(0, 36);
            var creationTime = tempId.Substring(37, 12);
            DateTime timeConverted = new DateTime();
            if (!DateTime.TryParseExact(creationTime, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeConverted))
                throw new Exception();
            if (DateTime.Now.Subtract(timeConverted).TotalMinutes > 5)
                throw new Exception();
            Guid myId = new Guid();
            if (!Guid.TryParse(Id, out myId))
                throw new Exception();
            return myId;
        }

        public string Encrypt(string text)
        {
            Aes cipher = CreateCipher(mySecret);
            cipher.IV = Convert.FromBase64String(IV);

            ICryptoTransform cryptTransform = cipher.CreateEncryptor();
            byte[] plaintext = Encoding.UTF8.GetBytes(text);
            byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

            return Convert.ToBase64String(cipherText);
        }
        public string Decrypt(string encryptedText)
        {
            Aes cipher = CreateCipher(mySecret);
            cipher.IV = Convert.FromBase64String(IV);

            ICryptoTransform cryptTransform = cipher.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
        public string InitSymmetricEncryptionKeyIV()
        {
            mySecret = GetEncodedRandomString(32); // 256
            Aes cipher = CreateCipher(mySecret);
            var IVBase64 = Convert.ToBase64String(cipher.IV);
            return IVBase64;
        }

        private string GetEncodedRandomString(int length)
        {
            var base64 = Convert.ToBase64String(GenerateRandomBytes(length));
            return base64;
        }

        private Aes CreateCipher(string keyBase64)
        {
            // Default values: Keysize 256, Padding PKC27
            Aes cipher = Aes.Create();
            cipher.Mode = CipherMode.CBC;  // Ensure the integrity of the ciphertext if using CBC

            cipher.Padding = PaddingMode.ISO10126;
            cipher.Key = Convert.FromBase64String(keyBase64);

            return cipher;
        }

        private byte[] GenerateRandomBytes(int length)
        {
            var byteArray = new byte[length];
            RandomNumberGenerator.Fill(byteArray);
            return byteArray;
        }

    }
}
