using System;
using FSNEP.Core.Domain;

namespace FSNEP.Core.Dto
{
    [Serializable]
    public class SignableCostShareEntry
    {
        public SignableCostShareEntry(CostShareEntry entry)
        {
            AccountName = entry.Account.Name;
            FundTypeName = entry.FundType.Name;
            ProjectName = entry.Project.Name;
            ExpenseTypeName = entry.ExpenseType.Name;

            EntryFileContent = entry.EntryFile.Content;
            
            Amount = entry.Amount;

            Description = entry.Description;
            Comment = entry.Comment;
        }

        public byte[] EntryFileContent { get; set; }

        public string ExpenseTypeName { get; set; }

        public double Amount { get; set; }

        public string Description { get; set; }

        public string Comment { get; set; }

        public string ProjectName { get; set; }

        public string FundTypeName { get; set; }

        public string AccountName { get; set; }
    }
}