using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net.Mail;
using ProjectPortal;

namespace wid
{
    class Program
    {
        #region REGION: Accessors
        static string AppName
        {
            get
            {
                return Process.GetCurrentProcess().MainModule.ModuleName.ToLower().Replace(".exe", "");
            }
        }
        static string CUID
        {
            get
            {
                return Environment.UserName.ToLower().Replace("ad\\", "");
            }
        }
        static string DriveLetter
        {
            get
            {
                string dir = Directory.GetCurrentDirectory();
                return dir.Substring(0, dir.IndexOf("\\") + 1);
            }
        }
        #endregion

        static int Main(string[] args)
        {
            StatusReportManager mgr = new StatusReportManager(AppName, Environment.UserName);

            Console.WriteLine(mgr.Run(args));
            return 0;
        }
       
    }
    public class StatusReportManager
    {
        protected string theAppName;
        protected string theCUID;
        protected StatusReport theReport;

        protected Dictionary<string, AppOption> theOptions = new Dictionary<string, AppOption>();
        protected List<AppOption> theList = new List<AppOption>();
        public StatusReportManager(string aAppName, string aCUID)
        {
            theAppName = aAppName.ToLower();
            theCUID = aCUID.ToLower();

            theReport = new StatusReport(theCUID);

            Add(new SetOptionDescription(theReport));

            Add(new SetOptionAppName(theReport));
            Add(new SetOptionProjName(theReport));
            Add(new SetOptionCategory(theReport));

            Add(new SetOptionListing(theReport));
            Add(new SetOptionProjListingType(theReport));
            Add(new SetOptionAppListingType(theReport));
            Add(new SetOptionReportStyle(theReport));

            Add(new SetOptionReportId(theReport));
            Add(new SetOptionDeleteFlag(theReport));

            Add(new SetOptionDate(theReport));

            string[] dow = { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };
            foreach (string opt in dow)
                Add(new SetOptionDate(theReport, opt, "Calculate the date for day-of-week value for updating or filtering."));

            string[] when = { "lastmonth", "lastweek", "yesterday", "today" };
            foreach (string opt in when)
                Add(new SetOptionDate(theReport, opt, "Use " + opt + "'s date for updating or filtering."));

            Add(new SetOptionUser(theReport));

            Add(new SetOptionNotify(theReport));

            Add(new SetOptionReportTest(theReport));

        }

        public void Add(AppOption aOpt)
        {
            theList.Add(aOpt);

            theOptions[aOpt.ShortName] = aOpt;
            theOptions[aOpt.LongName] = aOpt;
        }
        public string Usage()
        {
            StringBuilder usage = new StringBuilder();

            AppOption descr = theOptions["descr"];

            usage.AppendFormat("usage: {0} \"{1}\"", AppName, descr.HelpText);
            usage.Append("\n");
            usage.AppendFormat("\n See options below for more details:");
            usage.Append("\n");

            foreach (AppOption opt in theList)
                usage.AppendFormat("\n  {0}", opt.Usage("-"));

            return usage.ToString();
        }
        public string Run(string[] aArgs)
        {
            if (aArgs.Length == 0)
                return Usage();

            ConfigureReport(aArgs);

            string rv = "";

            try
            {
                if (theReport.DebugFlag)
                    Console.WriteLine(theReport.GetSettings());
                else if (theReport.ListingType.Contains("app"))
                    Console.WriteLine(theReport.GetApplicationList().ToString());
                else if (theReport.ListingType.Contains("proj"))
                    Console.WriteLine(theReport.GetProjectList().ToString());
                else if (theReport.ListingType.Contains("list"))
                    Console.WriteLine(theReport.ToString());
                else if (theReport.DeleteFlag)
                {
                    //theReport.ID = deleteRecordID;
                    theReport.Delete();
                }
                else
                {
                    rv = (theReport.Update() == false ? theReport.LastError : theReport.ToString());
                }
            }
            catch (Exception ex)
            {
                rv = ex.Message;
            }
            return rv;
        }
        public void ConfigureReport(string[] aArgs)
        {
            AppOption descr = theOptions["descr"];

            string optPrefix = "-";
            AppOption opt;
            string arg = "";
            string val = "";
            string next = "";
            int len = aArgs.Length;
            for (int i = 0; i < len; i++)
            {
                arg = aArgs[i];
                val = arg;

                if (arg.StartsWith(optPrefix))
                {
                    arg = arg.Replace(optPrefix, "").Replace("/", "");
                    next = (i + 1 < len ? aArgs[i + 1] : "");
                    val = (next.Length > 0 && next.StartsWith(optPrefix) == false ? aArgs[++i] : "");
                }

                if (theOptions.ContainsKey(arg))
                {
                    opt = theOptions[arg];
                    opt.Set(arg, val);
                }
                else
                    descr.Set(arg, val);
            }
        }

        public string AppName { get { return theAppName; } }
        public string CUID { get { return theCUID; } }
    }
    public class Option
    {
        public enum EValueType { String, Integer, Date };

        public string LongName;
        public string ShortName;
        public EValueType ValueType;
        public string ValueDescr;
        public bool Required;
        public StringBuilder Description = new StringBuilder();
        public delegate void SetterProc(string aArg, string aVal);

        private SetterProc theSetterProc = null;

        public Option(SetterProc aSetProc, string aLongName, string aShortName, EValueType aValueType, string aValueDescr, bool aRequired, string aDescription)
        {
            LongName = aLongName;
            ShortName = aShortName;
            ValueType = aValueType;
            ValueDescr = aValueDescr;
            Required = aRequired;
            if (aDescription.Length > 0)
                Description.Append(aDescription);

            theSetterProc = aSetProc;
        }

        public void SetParam(string aArg, string aVal)
        {
            if (theSetterProc != null)
                theSetterProc(aArg, aVal);
        }
        public override string ToString()
        {
            string opt = (LongName + (ShortName.Length == 0 ? "" : (LongName == ShortName ? "" : " | " + ShortName)));
            return String.Format("{0}{1}{2}{3} - {4}"
                , (Required ? "< " : "[ ")
                , opt
                , (ValueDescr.Length == 0
                    ? ""
                    : " " + (ValueType != EValueType.Integer ? "\"" : "")
                        + ValueDescr
                        + (ValueType != EValueType.Integer ? "\"" : "")
                  )
                , (Required ? " >" : " ]")
                , Description.ToString()
                );

        }
    }
}
