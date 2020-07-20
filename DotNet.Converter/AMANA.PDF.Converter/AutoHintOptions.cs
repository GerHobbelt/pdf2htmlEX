using System.ComponentModel;

namespace AMANA.PDF.Converter
{
    /// <summary>
    /// PdfToHtmlEx will use fontforge autohint on fonts without hints
    /// </summary>
    public enum AutoHintOptions
    {
        [Description("Default")]
        Default = 0,
        [Description("Use AutoHint")]
        UseAutoHint = 1
    }
}
