using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using Micajah.Common.Application;
using Micajah.Common.WebControls;
using Micajah.FileService.Dal.MainDataSetTableAdapters;
using Microsoft.Reporting.WebForms;

namespace Micajah.FileService.Management
{
    public partial class VendorsUsedSpacePage : Page
    {
        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            MagicForm.ApplyColorScheme(VendorsUsedSpaceReportTable, WebApplicationSettings.DefaultColorScheme);
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            VendorsUsedSpaceReportViewer.Reset();

            DateTime? startDate = (VendorsUsedSpaceReportDatesRange.DateStartIsDefault ? new DateTime?() : VendorsUsedSpaceReportDatesRange.DateStart);
            DateTime? endDate = (VendorsUsedSpaceReportDatesRange.DateEndIsDefault ? new DateTime?() : VendorsUsedSpaceReportDatesRange.DateEnd);

            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("StartDate", (startDate.HasValue ? startDate.Value.ToString("d", CultureInfo.CurrentCulture) : string.Empty)));
            parameters.Add(new ReportParameter("EndDate", (endDate.HasValue ? endDate.Value.ToString("d", CultureInfo.CurrentCulture) : string.Empty)));
            parameters.Add(new ReportParameter("VendorName", VendorName.Text));

            VendorsUsedSpaceReportViewer.LocalReport.ReportPath = this.Server.MapPath("~/Reports/VendorsUsedSpace.rdlc");
            VendorsUsedSpaceReportViewer.LocalReport.SetParameters(parameters);

            using (VendorTableAdapter adapter = new VendorTableAdapter())
            {
                VendorsUsedSpaceReportViewer.LocalReport.DataSources.Add(new ReportDataSource("VendorsUsedSpaceDataSource", adapter.GetVendorsUsedSpace(startDate, endDate, VendorName.Text)));
            }

            VendorsUsedSpaceReportViewer.CurrentPage = 1;
        }

        #endregion
    }
}