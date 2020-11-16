using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AMANA.PDF.Converter
{
    public class PdfConverter
    {
        private readonly string _pdfToHtmlExLocation;
        private readonly string _pdfToHtmlExPath;

        private StringBuilder _processOutput;
        public string ProcessOutput => _processOutput.ToString();

        public Dictionary<string, string> Pdf2HtmlExOptions = new Dictionary<string, string>();

        public ZoomRatio ZoomRatio { get; set; }
        public FontFormat FontFormat { get; set; }
        public UnicodeHandling UnicodeHandling { get; set; }
        public AutoHintOptions AutoHintOptions { get; set; }
        public string DockerMountPoint { get; set; }

        public PdfConverter(string pdfToHtmlExLocation)
        {
            _pdfToHtmlExLocation = pdfToHtmlExLocation;
            _pdfToHtmlExPath = Path.Combine(_pdfToHtmlExLocation, "pdf2htmlEX.exe");
            DockerMountPoint = "pdf";
        }

        private string Convert(ProcessStartInfo processStartInfo, string sourceFilePath, string targetFilePath = null)
        {
            string sourceFile = Path.Combine(_pdfToHtmlExLocation, "input.pdf");
            string destFile = Path.Combine(_pdfToHtmlExLocation, "input.html");
            File.Copy(sourceFilePath, sourceFile, true);

            processStartInfo.WorkingDirectory = _pdfToHtmlExLocation;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardInput = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            Process pdf2HtmlEx = new Process
            {
                StartInfo = processStartInfo
            };

            _processOutput = new StringBuilder();
            pdf2HtmlEx.OutputDataReceived += ProcessOutputHandler;
            pdf2HtmlEx.ErrorDataReceived += ProcessOutputHandler;

            Debug.WriteLine($"Run {pdf2HtmlEx.StartInfo.FileName} {pdf2HtmlEx.StartInfo.Arguments}");

            pdf2HtmlEx.Start();
            pdf2HtmlEx.BeginOutputReadLine();
            pdf2HtmlEx.BeginErrorReadLine();

            pdf2HtmlEx.WaitForExit();

            File.Delete(sourceFile);

            string compatibilityMinJsName = Path.Combine(_pdfToHtmlExLocation, "compatibility.min.js");
            if (File.Exists(compatibilityMinJsName))
                File.Delete(compatibilityMinJsName);
            string pdf2HtmlExMinJsName = Path.Combine(_pdfToHtmlExLocation, "pdf2htmlEX.min.js");
            if (File.Exists(pdf2HtmlExMinJsName))
                File.Delete(pdf2HtmlExMinJsName);

            if (pdf2HtmlEx.ExitCode <= 0 &&
                File.Exists(destFile))
            {
                if (!string.IsNullOrEmpty(targetFilePath))
                {
                    File.Copy(destFile, targetFilePath, overwrite: true);
                    File.Delete(destFile);
                    destFile = targetFilePath;
                }
            }

            return destFile;
        }

        public string Convert(string sourceFilePath, string targetFilePath = null)
        {
            return Convert(
                new ProcessStartInfo()
                {
                    FileName = _pdfToHtmlExPath,
                    Arguments = $"--no-drm 1 {GetZoomOption()} {GetFontFormatOption()} --decompose-ligature 1 --tounicode {GetUnicodeHandlingOption()} {GetAutoHintOption()} {GetOtherOptions()} input.pdf",
                },
                sourceFilePath,
                targetFilePath);
        }

        public string DockerConvert(string containerName, string sourceFilePath, string targetFilePath = null)
        {
            return Convert(
                new ProcessStartInfo()
                {
                    FileName = "docker",
                    Arguments = $"run --rm -it -v {GetVolumeString()} {containerName} --no-drm 1 {GetZoomOption()} {GetFontFormatOption()} --decompose-ligature 1 --tounicode {GetUnicodeHandlingOption()} {GetAutoHintOption()} {GetOtherOptions()} input.pdf",
                },
                sourceFilePath,
                targetFilePath);
        }

        private void ProcessOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
            {
                _processOutput.Append(Environment.NewLine);
                _processOutput.Append(outLine.Data);
            }
        }

        private string GetVolumeString()
        {
            return $"{_pdfToHtmlExLocation.Replace("\\", "/")}:/{DockerMountPoint}";
        }

        private string GetAutoHintOption()
        {
            if (AutoHintOptions == AutoHintOptions.UseAutoHint)
                return "--auto-hint 1";
            return "";
        }

        private string GetZoomOption()
        {
            switch (ZoomRatio)
            {
                case ZoomRatio.Ratio1:
                    return "--zoom 1.1";

                case ZoomRatio.Ratio2:
                    return "--zoom 1.3";

                case ZoomRatio.Ratio3:
                    return "--zoom 1.5";

                case ZoomRatio.Ratio4:
                    return "--zoom 1.75";

                case ZoomRatio.Ratio5:
                    return "--zoom 2";

                case ZoomRatio.Ratio6:
                    return "--zoom 2.5";

                case ZoomRatio.Ratio7:
                    return "--zoom 3";

                default:
                    return "";
            }
        }

        private string GetFontFormatOption()
        {
            switch (FontFormat)
            {
                case FontFormat.OTF:
                    return "--font-format otf";
                case FontFormat.TTF:
                    return "--font-format ttf";
                default:
                    return "";
            }
        }

        private string GetUnicodeHandlingOption()
        {
            switch (UnicodeHandling)
            {
                case UnicodeHandling.Force:
                    return "1";
                case UnicodeHandling.Ignore:
                    return "-1";
                default:
                case UnicodeHandling.Auto:
                    return "0";
            }
        }

        private string GetOtherOptions()
        {
            if (Pdf2HtmlExOptions.Count == 0)
                return "";
            StringBuilder sb = new StringBuilder();
            foreach (var pair in Pdf2HtmlExOptions)
            {
                bool ignore = false;
                switch (pair.Key)
                {
                    case "zoom":
                    case "auto-hint":
                    case "font-format":
                    case "no-drm":
                    case "decompose-ligature":
                    case "tounicode":
                    case "embed-css":
                    case "embed-font":
                    case "embed-image":
                    case "tmp-dir":
                    case "data-dir":
                        ignore = true;
                        break;
                }
                if (ignore)
                    continue;
                if (sb.Length > 0)
                    sb.Append(' ');
                if (pair.Key.Length == 1)
                    sb.Append("-");
                else
                    sb.Append("--");
                sb.Append(pair.Key);
                sb.Append(' ');
                sb.Append(pair.Value);
            }
            return sb.ToString();
        }
    }
}