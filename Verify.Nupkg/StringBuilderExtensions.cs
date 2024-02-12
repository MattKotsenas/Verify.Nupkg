using System.Text;

namespace VerifyTests;

internal static class StringBuilderExtensions
{
    public static void TrimStart(this StringBuilder sb)
    {
        int count = 0;
        for (int i = 0; i < sb.Length; i++)
        {
            if (char.IsWhiteSpace(sb[i]))
            {
                count++;
            }
            else
            {
                break;
            }
        }

        sb.Remove(0, count);
    }
}
