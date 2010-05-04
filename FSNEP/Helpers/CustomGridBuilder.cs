using Telerik.Web.Mvc.UI;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.Helpers
{
    public class CustomGridBuilder<T> : GridBuilder<T> where T : class
    {
        protected bool UseTransaction { get; set; }

        public CustomGridBuilder(Grid<T> component)
            : base(component)
        {
            UseTransaction = false;
        }

        public GridBuilder<T> Transactional()
        {
            UseTransaction = true;

            return this;
        }

        public override void Render()
        {
            if (UseTransaction)
            {
                using (var ts = new TransactionScope())
                {
                    base.Render();

                    ts.CommitTransaction();
                }
            }
            else
            {
                base.Render();
            }
        }
    }
}