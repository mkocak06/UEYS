using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICryptoService
    {
        string CreatePassword(int length);
        bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

        string Encrypt(string toEncrypt, bool useHashing);
        string Decrypt(string cipherString, bool useHashing);
    }
}
