using System.Net.Mail;

namespace MiniCrm.Application.Common;

public static class EmailValidator
{
    public static bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return true;
        }

        var trimmed = email.Trim();
        return MailAddress.TryCreate(trimmed, out var address) &&
               string.Equals(address.Address, trimmed, StringComparison.OrdinalIgnoreCase);
    }
}
