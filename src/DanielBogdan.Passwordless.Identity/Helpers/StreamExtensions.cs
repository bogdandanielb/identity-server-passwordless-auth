using System;
using System.IO;
using System.Linq;

namespace DanielBogdan.Passwordless.Identity.Helpers
{
    public static class StreamExtensions
    {
        public static string ToBase64(this Stream stream)
        {
            return Convert.ToBase64String(stream.ToArray());
        }

        public static byte[] ToArray(this Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
