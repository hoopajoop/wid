using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectPortal;

namespace wid
{
    public class StatusReportManager
    {
        private StatusReport theReport = null;
        private List<Option> theOptions = new List<Option>();
        private string theAppName = "";
        private bool theShowUsageFlag = false;
        public StatusReportManager(string aAppName, string aCUID)
        {
            theAppName = aAppName;
            theReport = new StatusReport(aCUID);

            theOptions.Add(new Option(SetListingType, "-list", "-l", Option.EValueType.String, "", false, "Displays a status report list based on provided filers."));
            theOptions.Add(new Option(SetAppName, "-app", "-a", Option.EValueType.String, "AppName", false, "Binds the entry to the specified application or acts as a filter for the list option"));
            theOptions.Add(new Option(SetProjName, "-proj", "-p", Option.EValueType.String, "ProjName", false, "Binds the entry to the specified project or acts as a filter for the list option"));
            theOptions.Add(new Option(SetCategory, "-cat", "-c", Option.EValueType.String, "category", false, "Marks the entry to the specified category (free form text) or acts as a filter for the list option"));
            theOptions.Add(new Option(SetUserID, "-cuid", "-cuid", Option.EValueType.String, "cuid", false, "Assigns the entry to the specified cuid or acts as a filter for the list option"));
            theOptions.Add(new Option(SetDescription, "-descr", "-descr", Option.EValueType.String, "category", false, "Marks the entry to the specified category (free form text) or acts as a filter for the list option"));

            theOptions.Add(new Option(SetRecordID, "-id", "-id", Option.EValueType.String, "category", false, "Marks the entry to the specified category (free form text) or acts as a filter for the list option"));
            theOptions.Add(new Option(SetDeleteFlag, "-delete", "-del", Option.EValueType.String, "rec-id", false, "Deletes the record provided by rec id; you can get the rec id by using the list option"));

            theOptions.Add(new Option(SetListingType, "-applist", "-al", Option.EValueType.String, "[\"filter\"]", false, "Displays a (filtered) list of application names"));
            theOptions.Add(new Option(SetListingType, "-projlist", "-pl", Option.EValueType.String, "[\"filter\"]", false, "Displays a (filtered) list of project names"));

            theOptions.Add(new Option(SetDateString, "-date", "-d", Option.EValueType.String, "any valid date format", false, "Uses the provided date when updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-since", "-since", Option.EValueType.String, "any valid date format", false, "Uses the provided date when updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-sun", "-sun", Option.EValueType.String, "", false, "Determines the date for the day of week for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-mon", "-mon", Option.EValueType.String, "", false, "Determines the date for the day of week for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-tue", "-tue", Option.EValueType.String, "", false, "Determines the date for the day of week for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-wed", "-wed", Option.EValueType.String, "", false, "Determines the date for the day of week for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-thu", "-thu", Option.EValueType.String, "", false, "Determines the date for the day of week for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-fri", "-fri", Option.EValueType.String, "", false, "Determines the date for the day of week for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-sat", "-sat", Option.EValueType.String, "", false, "Determines the date for the day of week for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-today", "-today", Option.EValueType.String, "", false, "Used today's date for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-yesterday", "-yd", Option.EValueType.String, "", false, "Used yesterday's date for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-lastweek", "-lw", Option.EValueType.String, "", false, "Used lastweek's date for updating or filtering"));
            theOptions.Add(new Option(SetDateString, "-lastmonth", "-lm", Option.EValueType.String, "", false, "Used lastmonth's date for updating or filtering"));

            theOptions.Add(new Option(SetNotifyList, "-notify", "-notify", Option.EValueType.String, "<cuid1,cuid2,cuid3,...,cuidN>", false, "Marks the entry to the specified category (free form text) or acts as a filter for the list option"));
            theOptions.Add(new Option(SetNotifyList, "-sendto", "-sendto", Option.EValueType.String, "<cuid1,cuid2,cuid3,...,cuidN>", false, "Marks the entry to the specified category (free form text) or acts as a filter for the list option"));

            theOptions.Add(new Option(SetReportType, "-report", "-r", Option.EValueType.String, "(flat | rollup)", false, "Formats the listing based on report type provided"));

            theOptions.Add(new Option(ShowUsage, "-help", "-?", Option.EValueType.String, "", false, "Displays usage information"));

            if (aCUID == "mxjoh18")
                theOptions.Add(new Option(SetDebug, "-debug", "-dbg", Option.EValueType.String, "", false, "Doesn't do anything; just dumps settings..."));
        }
        public string Run(string[] aArgs)
        {
            string rv = "";
            if (aArgs.Length == 0)
                Usage();
            else
            {
                int len = aArgs.Length;
                for (int i = 0; i < len; i++)
                {
                    string arg = aArgs[i];
                    string val = "";
                    foreach (Option opt in theOptions)
                    {
                        if (opt.LongName == arg || opt.ShortName == arg)
                        {
                            if (opt.ValueDescr.Length > 0 && i + 1 < len)
                                val = aArgs[++i];
                            opt.SetParam(arg, val);
                            break;
                        }
                        if (arg.StartsWith("-") == false)
                            theReport.Description = arg;
                    }
                }
            }

            if (theShowUsageFlag)
                return "";
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
        public void ShowUsage(string aArgs, string aVal)
        {
            Usage();
        }
        public void SetDebug(string aArgs, string aVal)
        {
            theReport.DebugFlag = true;
        }
        public void Usage()
        {
            theShowUsageFlag = true;
            Console.WriteLine("\nusage: {0} -app AppName \"status report text...\"", theAppName);
            Console.WriteLine("\nOptions:\n");
            foreach (Option opt in theOptions)
            {
                Console.WriteLine(opt.ToString());
            }
            //Console.WriteLine("\nExamples:\n");
            //foreach (string example in UsageExamples)
            //    Console.WriteLine(example.Replace("{appname}", theAppName));
        }
        public void SetListingType(string aArg, string aVal) { theReport.ListingType = aArg.Trim(); theReport.Description = aVal.Trim(); }
        public void SetRecordID(string aArg, string aVal) { theReport.ID = aVal.Trim(); }
        public void SetAppName(string aArg, string aVal) { theReport.ApplicationName = aVal.Trim(); }
        public void SetProjName(string aArg, string aVal) { theReport.ProjectName = aVal.Trim(); }
        public void SetDeleteFlag(string aArg, string aVal) { theReport.ID = aVal; theReport.DeleteFlag = true; }
        public void SetUserID(string aArg, string aVal) { theReport.CUID = aVal.Trim(); }
        public void SetDescription(string aArg, string aVal) { theReport.Description = aVal.Trim(); }
        public void SetCategory(string aArg, string aVal) { theReport.Category = aVal.Trim(); }
        public void SetNotifyList(string aArg, string aVal) { theReport.NotifyList = aVal.Trim(); }
        public void SetDateString(string aArg, string aVal) { theReport.DateString = aArg.Trim(); }
        public void SetReportType(string aArg, string aVal) { theReport.ReportStyle = aVal.Trim(); }
    }
}
