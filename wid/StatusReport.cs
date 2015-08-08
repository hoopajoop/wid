using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Mail;

namespace wid
{
    public class StatusReport
    {
        
        public StatusReport(string aCUID)
        {
            ReportUser = aCUID;
            theFormats["flat"] = new FlatLineItemReport();
            theFormats["rollup-withdate"] = new RollUpItemsByDayByAppReport();
            theFormats["rollup-descr"] = new RollUpDescriptionOnlyByAppReport();
        }

        #region REGION: Data Members
        private string theID = "0";
        private string theApp = "";
        private string theProj = "";
        private string theCat = "";
        private string theDescr = "";
        private string theDateString = "";
        private string theCUID = "";
        private string theReportUser = "";
        private string theNotifyList = "";
        private DateTime theDate;
        private string theLastError = "";
        private string theReportStyle = "";

        private Dictionary<string, StatusReportFormat> theFormats = new Dictionary<string, StatusReportFormat>();
        #endregion

        #region REGION: Accessors
        public string CUID
        {
            get { return theCUID; }
            set { theCUID = value; }
        }
        public string ReportUser
        {
            get { return theReportUser; }
            set { theReportUser = value; }
        }
        public string ID
        {
            get { return theID; }
            set { theID = value.Trim(); }
        }
        public string ApplicationName
        {
            get { return theApp; }
            set { theApp = value.Trim(); }
        }
        public string ProjectName
        {
            get { return theProj; }
            set { theProj= value.Trim(); }
        }
        public string Category
        {
            get { return theCat; }
            set { theCat = value.Trim(); }
        }
        public string Description
        {
            get { return theDescr; }
            set { theDescr = value.Trim(); }
        }
        public string NotifyList
        {
            get { return theNotifyList; }
            set { theNotifyList = value.Trim(); }
        }
        public string DateString
        {
            get { return theDateString; }
            set
            {
                theDateString = value.Trim();
                theDate = GetDate(theDateString);
            }
        }
        public DateTime Date
        {
            get { return theDate; }

        }
        public string ReportStyle
        {
            get { return theReportStyle; }
            set { theReportStyle = value.Trim(); } 
        }
        #endregion

        #region REGION: Support Functions
        private void ThrowError(string aMessage, params object[] aArgList)
        {
            string msg = "";
            try
            {
                msg = String.Format(aMessage, aArgList);
            }
            catch
            {
            }

            throw new Exception(msg);
        }
        private DateTime GetDate(string aDate)
        {
            DateTime dt = DateTime.Now;
            switch (aDate)
            {
                case "yesterday": dt -= new TimeSpan(24, 0, 0); break;
                case "lastweek": dt -= new TimeSpan(7, 0, 0, 0); break;
                case "lastmonth": dt -= new TimeSpan(30, 0, 0, 0); break;
                default:
                    if (DateTime.TryParse(aDate, out dt) == false)
                        return dt;
                    break;
            }

            return dt;
        }
        private bool SendReport(StringBuilder aReport)
        {
            string[] list = theNotifyList.Split(';');
            if (list.Length == 0)
                list = theNotifyList.Split(',');

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("Network Project Portal (" + ReportUser + ") <" + ReportUser+ "@qwest.com>");

            foreach (string cuid in list)
            {
                msg.To.Add(new MailAddress(cuid + "@qwest.com"));
            }

            msg.IsBodyHtml = false;
            msg.Subject = "Status Report";

            msg.Body = aReport.ToString();

            SmtpClient smtp = new SmtpClient("mailgate.qintra.com");
            smtp.Send(msg);

            return true;
        }
        private bool UpdateLineItem(string aAction)
        {
            int reportID = 0;
            Int32.TryParse(ID, out reportID);
            if ((reportID == 0 && ID != "0"))
                ThrowError("Invalid ReportID #{0}; must by >= 0", ID);

            if (reportID == 0 && Description.Length == 0)
                ThrowError("You must provide some report text.");

            if (DateString.Length > 0 && Date == DateTime.MinValue)
                ThrowError("Invalid date provided: {0}", DateString);

            int? ec = 0;
            string err = "";
            int? ov = 0;
            try
            {
                DateTime? date = (DateString.Length > 0 ? Date : (DateTime?)null);
                DALTableAdapters.QueriesTableAdapter db = new wid.DALTableAdapters.QueriesTableAdapter();
                db.UpdateStatusReport(ref ec, ref err, ref ov, ReportUser
                    , aAction
                    , reportID
                    , ApplicationName
                    , ProjectName
                    , Category
                    , Description
                    , date
                    );
            }
            catch (Exception ex)
            {
                ec = -1;
                err = ex.Message;
                ov = 0;
            }

            if (ec != 0)
            {
                theLastError = err;
                return false;
            }

            ID = ((int)ov).ToString();
            
            return true;
        }
        #endregion

        #region REGION: Public Methods
        public override string ToString()
        {
            return GetListing().ToString();
        }
        public StringBuilder GetApplicationList()
        {
            StringBuilder listing = new StringBuilder();
            DALTableAdapters.ApplicationListTableAdapter db = new wid.DALTableAdapters.ApplicationListTableAdapter();
            DAL.ApplicationListDataTable list = (ApplicationName.Length == 0 ? db.GetData() : db.Search(ApplicationName));
            foreach (DAL.ApplicationListRow app in list.Rows)
            {
                listing.AppendFormat("\n{0}", app.Name);
            }
            return listing;
        }
        public StringBuilder GetProjectList()
        {
            StringBuilder listing = new StringBuilder();
            DALTableAdapters.ProjectListTableAdapter db = new wid.DALTableAdapters.ProjectListTableAdapter();
            DAL.ProjectListDataTable list = (ProjectName.Length == 0 ? db.GetData() : db.Search(ProjectName));
            foreach (DAL.ProjectListRow app in list.Rows)
            {
                listing.AppendFormat("\n{0}", app.Name);
            }
            return listing;
        }
        public bool Update()
        {
            return UpdateLineItem("UPDATE");
        }
        public bool Delete()
        {
            return UpdateLineItem("DELETE");
        }
        public StringBuilder GetListing()
        {
            int? ec = 0;
            string err = "";
            int? ov = 0;

            DAL.StatusReportDataTable listing = null;
            string descr = "%" + Description + "%";

            if (DateString.Length > 0 && Date == DateTime.MinValue)
                ThrowError("Invalid date provided: {0}", DateString);
            else if (Date == DateTime.MinValue)
                DateTime.TryParse("1901-01-01", out theDate);
            try
            {
                DALTableAdapters.StatusReportTableAdapter db = new wid.DALTableAdapters.StatusReportTableAdapter();
                listing = db.GetData(ref ec, ref err, ref ov, ReportUser
                    , true
                    , CUID
                    , ApplicationName
                    , ProjectName
                    , Category
                    , descr
                    , Date);
            }
            catch (Exception ex)
            {
                ec = -1;
                err = ex.Message;
                ov = 0;
            }

            if (ec != 0)
                ThrowError(err);

            if (theFormats.ContainsKey(ReportStyle) == false)
                ReportStyle = "flat";

            StringBuilder report = theFormats[ReportStyle].Display(ref listing);
            if( NotifyList.Length > 0)
                SendReport(report);

            return report;
        }
        #endregion
    }
}
