using System.Text;

namespace YayZent.Framework.Core.Helper;

public static class StreamHelper
{
    public static Stream StringToStream(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        return new MemoryStream(bytes);
    }
}