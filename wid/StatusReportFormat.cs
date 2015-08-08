using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wid
{
    public class StatusReportFormat
    {
        public StringBuilder theReport = new StringBuilder();
        public virtual StringBuilder Display(ref DAL.StatusReportDataTable aListing)
        {
            return theReport;
        }
    }

    public class FlatLineItemReport : StatusReportFormat
    {
        public override StringBuilder Display(ref DAL.StatusReportDataTable aListing)
        {
            foreach (DAL.StatusReportRow row in aListing.Rows)
            {
                theReport.AppendFormat("\n {0,5}) {1:yyyy-MM-dd hh:mm tt} / {2} / {3}: {4}"
                    , row.ReportID
                    , row.TS
                    , row.Name
                    , row.Cat
                    , row.Descr
                    );
            }
            return theReport;
        }
    }
    public class RollUpDescriptionOnlyByAppReport : StatusReportFormat
    {
        public override StringBuilder Display(ref DAL.StatusReportDataTable aListing)
        {
            string appName = "";
            foreach (DAL.StatusReportRow row in aListing.Rows)
            {
                if (appName != row.Name)
                {
                    theReport.AppendFormat("\n{0}{1}: {2}"
                        , (appName.Length == 0 ? "" : "\n")
                        , row.Name
                        , row.URL);
                    appName = row.Name;
                }

                theReport.AppendFormat("\n* {0 }", row.Descr );
            }
            if (aListing.Rows.Count == 0)
                theReport.Append("No status found.");
            return theReport;
        }
    }
    public class RollUpItemsByDayByAppReport : StatusReportFormat
    {
        public override StringBuilder Display(ref DAL.StatusReportDataTable aListing)
        {
            string appName = "";
            foreach (DAL.StatusReportRow row in aListing.Rows)
            {
                if (appName != row.Name)
                {
                    theReport.AppendFormat("\n{0}{1}: {2}"
                        , (appName.Length == 0 ? "" : "\n")
                        , row.Name
                        , row.URL);
                    appName = row.Name;
                }

                theReport.AppendFormat("\n* {0:yyyy-MM-dd} {1} {2}: {3}"
                    , row.TS
                    , row.CUID
                    , row.Cat.ToUpper()
                    , row.Descr
                    );
            }
            if (aListing.Rows.Count == 0)
                theReport.Append("No status found.");
            return theReport;
        }
    }
}
