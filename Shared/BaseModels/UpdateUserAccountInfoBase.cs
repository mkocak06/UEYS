using System;
using Shared.Types;

namespace Shared.BaseModels;

public class UpdateUserAccountInfoBase
{
    public string Password { get; set; }
    public DateTime? LockedUntil { get; set; }
    public bool? PasswordChangeRequired { get; set; }
}