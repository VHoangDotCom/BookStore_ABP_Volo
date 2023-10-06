using CloudinaryTest.Entities;

namespace CloudinaryTest.Helper
{
    public static class CloudinaryFormatMapper
    {
        public static string MapToCloudinaryFormat(FormatType format)
        {
            switch (format)
            {
                case FormatType.JPG:
                    return "jpg";
                case FormatType.JPGE:
                    return "jpge";
                case FormatType.GIF:
                    return "gif";
                case FormatType.PNG:
                    return "png";
                case FormatType.JPE:
                    return "jpe";
                case FormatType.TIF:
                    return "tif";
                default:
                    throw new ArgumentException("Unsupported format type");
            }
        }
    }

}
