$(document).ready(function () {
    var ReportName = "~/adventureworks.xml";

    $("#reportviewersample").c1reportviewer({
        reportServiceUrl: "/reportService.ashx",
        fileName: $('#HFReportFileName').val(),
        reportName: ReportName,
        zoom: "100%",
        collapseToolsPanel: true,
        width: 950,
        height: 600
    });
});
