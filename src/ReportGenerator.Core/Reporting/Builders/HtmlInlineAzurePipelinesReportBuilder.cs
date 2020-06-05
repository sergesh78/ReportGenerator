using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using Palmmedia.ReportGenerator.Core.Reporting.Builders.Rendering;

namespace Palmmedia.ReportGenerator.Core.Reporting.Builders
{
    /// <summary>
    /// Creates report in HTML format.
    /// </summary>
    public class HtmlInlineAzurePipelinesReportBuilder : ReportBuilderBase
    {
        private static readonly HashSet<string> RenderedCodeFiles = new HashSet<string>();

        /// <summary>
        /// Gets the report type.
        /// </summary>
        /// <value>
        /// The report format.
        /// </value>
        public override string ReportType => "HtmlInline_AzurePipelines";

        /// <summary>
        /// Creates a class report.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="fileAnalyses">The file analyses that correspond to the class.</param>
        public override void CreateClassReport(Class @class, IEnumerable<FileAnalysis> fileAnalyses)
        {
            Parallel.ForEach(fileAnalyses, fileAnalysis =>
            {
                if (string.IsNullOrEmpty(fileAnalysis.Error))
                {
                    using (var renderer = new HtmlRenderer(false, HtmlMode.InlineCssAndJavaScript))
                    {
                        var codeFile = @class.Files.First(f => f.Path.Equals(fileAnalysis.Path));
                        string localPath = HtmlRenderer.GetFileReportFilename(codeFile.Path);

                        bool shouldGenerateReport;
                        lock (RenderedCodeFiles)
                        {
                            shouldGenerateReport = RenderedCodeFiles.Add(localPath);
                        }

                        if (shouldGenerateReport)
                        {
                            this.CreateFileReport(renderer, codeFile, fileAnalysis);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Creates the summary report.
        /// </summary>
        /// <param name="summaryResult">The summary result.</param>
        public override void CreateSummaryReport(SummaryResult summaryResult)
        {
            using (var renderer = new HtmlRenderer(false, HtmlMode.InlineCssAndJavaScript, "custom-azurepipelines.css"))
            {
                this.CreateSummaryReport(renderer, summaryResult);
            }

            foreach (var assembly in summaryResult.Assemblies)
            {
                using (var renderer = new HtmlRenderer(false, HtmlMode.InlineCssAndJavaScript))
                {
                    this.CreateAssemblyReport(renderer, summaryResult, assembly);
                }
            }

            File.Copy(
                Path.Combine(this.ReportContext.ReportConfiguration.TargetDirectory, "index.html"),
                Path.Combine(this.ReportContext.ReportConfiguration.TargetDirectory, "index.htm"),
                true);
        }
    }
}