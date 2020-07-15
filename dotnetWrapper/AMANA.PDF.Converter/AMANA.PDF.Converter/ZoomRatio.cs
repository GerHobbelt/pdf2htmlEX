using System.ComponentModel;

namespace AMANA.PDF.Converter
{
    /// <summary>
    /// This enum affects to PDF coversion zoom ration in PDFToHtmlEx
    /// </summary>
    public enum ZoomRatio
    {
        /// <summary>
        /// Zoom = 0
        /// </summary>
        [Description("None")]
        None,
        /// <summary>
        /// Zoom = 1.1
        /// </summary>
        [Description("110%")]
        Ratio1,
        /// <summary>
        /// Zoom = 1.3
        /// </summary>
        [Description("130%")]
        Ratio2,
        /// <summary>
        /// Zoom = 1.5
        /// </summary>
        [Description("150%")]
        Ratio3,
        /// <summary>
        /// Zoom = 1.75
        /// </summary>
        [Description("175%")]
        Ratio4,
        /// <summary>
        /// Zoom = 2
        /// </summary>
        [Description("200%")]
        Ratio5,
        /// <summary>
        /// Zoom = 2
        /// </summary>
        [Description("250%")]
        Ratio6,

        /// <summary>
        /// Zoom = 2
        /// </summary>
        [Description("300%")]
        Ratio7
    }
}