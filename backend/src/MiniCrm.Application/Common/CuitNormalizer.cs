using System.Text;

namespace MiniCrm.Application.Common;

public static class CuitNormalizer
{
    public static string Normalize(string? cuit)
    {
        if (string.IsNullOrWhiteSpace(cuit))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(11);
        foreach (var character in cuit)
        {
            if (char.IsDigit(character))
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}
