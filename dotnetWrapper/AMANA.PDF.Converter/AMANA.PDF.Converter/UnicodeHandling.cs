using System.ComponentModel;

namespace AMANA.PDF.Converter
{
    /// <summary>
    /// This option control pdfToHTMLEx to Unicode Cmaps handling during PDF conversion
    /// </summary>
    public enum UnicodeHandling
    {
        /// <summary>
        /// Auto 
        /// </summary>
        [Description("Auto")]
        Auto = 0,
        /// <summary>
        /// Force
        /// </summary>
        [Description("Force")]
        Force = 1,
        /// <summary>
        /// Ignore
        /// </summary>
        [Description("Ignore")]
        Ignore = 2
    }
}
