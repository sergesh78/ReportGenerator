﻿using System.Collections.Generic;
using System.IO;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using Palmmedia.ReportGenerator.Core.Reporting.Builders.Rendering;

namespace Palmmedia.ReportGenerator.Core.Reporting.Builders
{
    /// <summary>
    /// Creates report in HTML format.
    /// </summary>
    public class HtmlReportBuilder : ReportBuilderBase
    {
        /// <summary>
        /// Defines how CSS and JavaScript are referenced.
        /// </summary>
        private readonly HtmlMode htmlMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReportBuilder" /> class.
        /// </summary>
        public HtmlReportBuilder()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReportBuilder" /> class.
        /// </summary>
        /// <param name="externalCssAndJavaScriptWithQueryStringHandling">Defines how CSS and JavaScript are referenced.</param>
        public HtmlReportBuilder(bool externalCssAndJavaScriptWithQueryStringHandling)
        {
            this.htmlMode = externalCssAndJavaScriptWithQueryStringHandling ? HtmlMode.ExternalCssAndJavaScriptWithQueryStringHandling
                : HtmlMode.ExternalCssAndJavaScript;
        }

        /// <summary>
        /// Gets the report type.
        /// </summary>
        /// <value>
        /// The report format.
        /// </value>
        public override string ReportType => "Html";


        /// <summary>
        /// Creates a class report.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="fileAnalyses">The file analyses that correspond to the class.</param>
        public override void CreateClassReport(Class @class, IEnumerable<FileAnalysis> fileAnalyses)
        {
            using (var renderer = new HtmlRenderer(false, this.htmlMode))
            {
                this.CreateClassReport(renderer, @class, fileAnalyses);
            }
        }

        /// <summary>
        /// Creates the summary report.
        /// </summary>
        /// <param name="summaryResult">The summary result.</param>
        public override void CreateSummaryReport(SummaryResult summaryResult)
        {
            using (var renderer = new HtmlRenderer(false, this.htmlMode))
            {
                this.CreateSummaryReport(renderer, summaryResult);
            }

            foreach (var assembly in summaryResult.Assemblies)
            {
                using (var renderer = new HtmlRenderer(false, this.htmlMode))
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
