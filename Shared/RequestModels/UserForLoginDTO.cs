using System.ComponentModel.DataAnnotations;
using Shared.BaseModels;


namespace Shared.RequestModels
{
    public class UserForLoginDTO : UserForLoginBase
    {
        public string Password { get; set; }
    }
}
