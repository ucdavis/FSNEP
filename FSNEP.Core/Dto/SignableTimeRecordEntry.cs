using FSNEP.Core.Domain;
using System;

namespace FSNEP.Core.Dto
{
    /// <summary>
    /// Serializable class of time record entries for creating digital signatures
    /// </summary>
    [Serializable]
    public class SignableTimeRecordEntry
    {
        public SignableTimeRecordEntry(TimeRecordEntry entry)
        {
            Date = entry.Date;
            Hours = entry.Hours;
            AccountName = entry.Account.Name;
            ActivityName = entry.ActivityType.Name;
            FundTypeName = entry.FundType.Name;
            ProjectName = entry.Project.Name;

            AdjustmentDate = entry.AdjustmentDate;
            Comment = entry.Comment;
        }

        public DateTime? AdjustmentDate { get; set; }

        public string Comment { get; set; }

        public string ProjectName { get; set; }

        public string FundTypeName { get; set; }

        public string ActivityName { get; set; }

        public string AccountName { get; set; }

        public int Date { get; private set; }

        public double Hours { get; private set; }
    }
}