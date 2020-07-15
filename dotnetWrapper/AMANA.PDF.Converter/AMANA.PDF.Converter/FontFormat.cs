using System.ComponentModel;

namespace AMANA.PDF.Converter
{
    public enum FontFormat
    {
        /// <summary>
        /// Default value. WOFF font format
        /// </summary>
        [Description("WOFF")]
        WOFF = 0,

        /// <summary>
        /// OTF font format
        /// </summary>
        [Description("OTF")]
        OTF = 1,

        /// <summary>
        /// OTF font format
        /// </summary>
        [Description("TTF")]
        TTF = 2
    }
}
