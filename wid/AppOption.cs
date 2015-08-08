using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectPortal;

namespace wid
{
    public class AppOption
    {
        protected string theShortName;
        protected string theLongName;
        protected string theValueDescr;
        protected string theValue = "";
        protected string theArg = "";
        protected string theHelpText;
        protected bool theRequiredFlag;
        protected StatusReport theReport;

        public delegate void SetReportParamsEvent(object sender, EventArgs e);
        public event SetReportParamsEvent theSetReportParamsEvent;
        
        public AppOption(StatusReport aReport)
        {
            theReport = aReport;
            IsRequired = false;
        }
        public AppOption(StatusReport aReport, string aShortName, string aLongName, string aValueDescr, string aHelpText)
        {
            theReport = aReport;
            theShortName = aShortName;
            theLongName = aLongName;
            theValueDescr = aValueDescr;
            theHelpText = aHelpText;
            IsRequired = false;

            theSetReportParamsEvent += new SetReportParamsEvent(OnSetReportParamsEvent);
        }

        public bool IsRequired
        {
            get { return theRequiredFlag; }
            set { theRequiredFlag = value; }
        }
        public virtual void Set(string aArg, string aValue)
        {
            Arg = aArg.Trim();
            Value = aValue.Trim();
        }
        public virtual string Arg
        {
            get { return theArg; }
            set 
            {
                theArg = value.Trim();
                if (theSetReportParamsEvent != null)
                    theSetReportParamsEvent(null, null); 
            } 
        }
        public virtual string Value
        {
            get { return theValue; }
            set 
            { 
                theValue = value.Trim();
                if (theSetReportParamsEvent != null)
                    theSetReportParamsEvent(null, null); 
            }
        }
        public string ValueDescr
        {
            get { return theValueDescr; }
            set { theValueDescr = value.Trim(); } 
        }
        public bool IsValueRequired
        {
            get { return theValueDescr.Length > 0; } 
        }
        public string ShortName
        {
            get { return theShortName; }
            set { theShortName = value.Trim(); } 
        }
        public string LongName
        {
            get { return theLongName; }
            set { theLongName = value.Trim(); } 
        }
        public string HelpText
        {
            get { return theHelpText; }
            set { theHelpText = value.Trim(); }
        }
        public virtual void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            throw new Exception("Set report parameter event not implemented.");
        }

        public string Usage(string aPrefix)
        {
            string descr = (IsValueRequired ? "\""+ValueDescr+"\"" : "");
            string name = String.Format("[ {0,-30} ]", String.Format("{0}{1} {2}", aPrefix, LongName, descr));
            if (IsRequired)
                name = name.Replace("[", "<").Replace("]", ">");
            return String.Format("{0}: {1}", name, HelpText);
        }
    }

    public class SetOptionListing : AppOption
    {
        public SetOptionListing(StatusReport aReport)
            : base(aReport, "l", "list", "[filter]", "Displays a status report list based on provided filters.")
        {            
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.ListingType = Arg;
            theReport.Description = Value;
        }
    }
    public class SetOptionAppName : AppOption
    {
        public SetOptionAppName(StatusReport aReport)
            : base(aReport, "a", "app", "AppName", "Binds the entry to the specified application or acts as a filter for the list option.")
        {
        }

        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.ApplicationName = Value;
        }
    }
    public class SetOptionProjName : AppOption
    {
        public SetOptionProjName(StatusReport aReport)
            : base(aReport, "p","proj", "ProjectName", "Binds the entry to the specified project or acts as a filter for the list option.")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.ProjectName = Value;
        }
    }
    public class SetOptionCategory : AppOption
    {
        public SetOptionCategory(StatusReport aReport)
            : base(aReport, "c", "cat", "Category", "Marks the entry to the specified category (free form text) or acts as a filter for the list option.")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.Category = Value;
        }
    }
    public class SetOptionUser : AppOption
    {
        public SetOptionUser(StatusReport aReport)
            : base(aReport, "cuid", "cuid", "CUID", "Assigns the entry to the specified cuid or acts as a filter for the list option.")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.CUID = Value;
        }
    }
    public class SetOptionDescription : AppOption
    {
        public SetOptionDescription(StatusReport aReport)
            : base(aReport, "descr", "descr", "Text", "Description text for the report item.")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.Description = Value;
        }
    }
    public class SetOptionReportId : AppOption
    {
        public SetOptionReportId(StatusReport aReport)
            : base(aReport, "id", "id", "ID", "Marks the entry to the specified category (free form text) or acts as a filter for the list option.")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.ID = Value;
        }
    }
    public class SetOptionDeleteFlag : AppOption
    {
        public SetOptionDeleteFlag(StatusReport aReport)
            : base(aReport, "del", "delete", "ID", "Deletes the record provided by rec id; you can get the rec id by using the list option.")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.DeleteFlag = true;
        }
    }
    public class SetOptionAppListingType : AppOption
    {
        public SetOptionAppListingType(StatusReport aReport)
            : base(aReport, "al", "applist", "match-string", "Displays a (filtered) list of application names.")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.ListingType = Arg;
            theReport.Description = Value;
        }
    }
    public class SetOptionProjListingType : AppOption
    {
        public SetOptionProjListingType(StatusReport aReport)
            : base(aReport, "pl", "projlist", "match-string", "Displays a (filtered) list of project names.")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.ListingType = Arg;
            theReport.Description = Value;
        }
    }    
    public class SetOptionDate : AppOption
    {
        public SetOptionDate(StatusReport aReport)
            : base(aReport, "date", "date", "any valid date format", "Uses the provided date when updating or filtering.")
        {
        }
        public SetOptionDate(StatusReport aReport, string aName, string aHelpText)
            : base(aReport, aName, aName, "", aHelpText)
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.DateString = (Value.Length == 0 && Arg.Length > 0 ? Arg : Value); 
        }
    }
    public class SetOptionNotify : AppOption
    {
        public SetOptionNotify(StatusReport aReport)
            : base(aReport, "notify", "sendto", "cuid1[,cuid2,...]", "Sends the listing to the specified users.")

        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.NotifyList = Value;
        }
    }
    public class SetOptionReportStyle : AppOption
    {
        public SetOptionReportStyle(StatusReport aReport)
            : base(aReport, "style", "style", "<flat | rollup>", "Formats the listing based on report type provided.")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.ReportStyle = Value;
        }
    }
    public class SetOptionReportTest : AppOption
    {
        public SetOptionReportTest(StatusReport aReport)
            : base(aReport, "test", "test", "", "Show report settings")
        {
        }
        public override void OnSetReportParamsEvent(object sender, EventArgs e)
        {
            theReport.DebugFlag = true;
        }
    }
    
}
