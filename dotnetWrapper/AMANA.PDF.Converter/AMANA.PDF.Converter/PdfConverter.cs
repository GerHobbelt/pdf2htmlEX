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

        public PdfConverter(string pdfToHtmlExLocation)
        {
            _pdfToHtmlExLocation = pdfToHtmlExLocation;
            _pdfToHtmlExPath = Path.Combine(_pdfToHtmlExLocation, "pdf2htmlEX.exe");
        }

        public string Convert(string sourceFilePath, string targetFilePath = null)
        {
            string sourceFile = Path.Combine(_pdfToHtmlExLocation, "input.pdf");
            string destFile = Path.Combine(_pdfToHtmlExLocation, "input.html");
            File.Copy(sourceFilePath, sourceFile, true);

            Process pdf2HtmlEx = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = _pdfToHtmlExLocation,
                    FileName = _pdfToHtmlExPath,
                    Arguments =
                        $"--no-drm 1 --embed-javascript 0 {ZoomRatio} {FontFormat} --decompose-ligature 1 --tounicode {UnicodeHandling} {AutoHintOptions} {GetOtherOptions()} input.pdf",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
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

        private void ProcessOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
            {
                _processOutput.Append(Environment.NewLine);
                _processOutput.Append(outLine.Data);
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
                    case "embed-javascript":
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