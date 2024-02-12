using System.Text;

namespace VerifyTests;

internal static class StringExtensions
{
    public static string GetLeadingWhiteSpace(this string text)
    {
        StringBuilder sb = new();

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsWhiteSpace(c))
            {
                sb.Append(c);
            }
            else
            {
                break;
            }
        }

        return sb.ToString();
    }
}
