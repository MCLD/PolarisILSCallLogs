using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PolarisCallLog
{
    public class Parser
    {
        private string fileBlob = "Polaris Phone Notification_*.log";
        public string FileBlob {
            get {
                return fileBlob;
            }
            set {
                fileBlob = value;
            }
        }

        public CallLogSummary ReadLog(string logfile, bool insertInDatabase = true)
        {
            CallLogSummary summary = null;
            var log = NLog.LogManager.GetLogger(this.GetType().ToString());
            var calls = new Dictionary<string, CallLogRecord>();
            using (var reader = new System.IO.StreamReader(logfile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var elements = line.Split(',');
                    if (string.IsNullOrEmpty(elements[0])
                        || elements[0].StartsWith("#")
                        || elements.Length < 6)
                    {
                        // comment, ignore
                        continue;
                    }
                    string patronId = elements[6];
                    if (!string.IsNullOrEmpty(patronId) && patronId != "0")
                    {
                        CallLogRecord currentCall;
                        if (calls.ContainsKey(patronId))
                        {
                            currentCall = calls[patronId];
                        }
                        else
                        {
                            currentCall = new CallLogRecord()
                            {
                                FirstSeen = DateTime.Parse($"{elements[0]} {elements[1]}"),
                                PatronId = patronId
                            };
                            calls.Add(patronId, currentCall);
                        }
                        currentCall.LastSeen = DateTime.Parse($"{elements[0]} {elements[1]}");
                        int secondToLastQuote = line.Substring(0, line.Length - 1).LastIndexOf('"');
                        string comment = line.Substring(secondToLastQuote + 1,
                            line.Length - (secondToLastQuote + 2));
                        if (comment.Contains("STARTING CALL"))
                        {
                            currentCall.IsStarted = true;
                        }
                        if (comment.Contains("Playing overdues message"))
                        {
                            currentCall.IsOverdue = true;
                        }
                        if (comment.Contains("Playing holds message"))
                        {
                            currentCall.IsHold = true;
                        }
                        if (comment.Contains("Connection time (seconds): "))
                        {
                            currentCall.ConnectionTime = comment.Substring(27);
                        }
                    }
                }
                reader.Close();
            }

            if (calls.Count == 0)
            {
                log.Info($"Zero calls found in {logfile}");
            }
            else
            {
                string fileNameOnly = System.IO.Path.GetFileName(logfile);
                string dateFromFilename = fileNameOnly
                    .Replace(FileBlob.Substring(0, FileBlob.IndexOf('*')), string.Empty)
                    .Replace(FileBlob.Substring(FileBlob.IndexOf('*') + 1), string.Empty);
                summary = new CallLogSummary()
                {
                    CallCount = calls.Count(),
                    Filename = fileNameOnly,
                    Date = DateTime.ParseExact(dateFromFilename, "yyMMdd", CultureInfo.InvariantCulture),
                    FirstEntry = calls.OrderBy(c => c.Value.FirstSeen).First().Value.FirstSeen,
                    LastEntry = calls.OrderByDescending(c => c.Value.LastSeen).Last().Value.LastSeen,
                    HoldCalls = calls.Count(c => c.Value.IsHold),
                    OverdueCalls = calls.Count(c => c.Value.IsOverdue),
                    StartedCalls = calls.Count(c => c.Value.IsStarted)
                };
                log.Info($"Identified {summary.CallCount} calls in '{summary.Filename}'");
                if (log.IsDebugEnabled)
                {
                    log.Debug($"Found {summary.CallCount} started calls; {summary.OverdueCalls} overdues; {summary.HoldCalls} holds");
                    log.Debug($"First call started {summary.FirstEntry}; last call ended {summary.LastEntry}");
                }

                if (insertInDatabase)
                {
                    using (var context = new PolarisCallLogContext())
                    {
                        var alreadySummary = context
                            .Summaries
                            .AsNoTracking()
                            .Where(s => s.Date == summary.Date)
                            .SingleOrDefault();
                        if (alreadySummary == null)
                        {
                            context.Summaries.Add(summary);
                            context.SaveChanges();
                        }
                        else
                        {
                            throw new Exception($"{summary.Date} is already in the CallLogSummaries table");
                        }
                    }
                }
            }
            return summary;
        }
    }
}
