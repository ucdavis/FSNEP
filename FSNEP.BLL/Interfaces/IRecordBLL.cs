using System;
using System.Collections.Generic;
using System.Security.Principal;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Interfaces
{
    public interface IRecordBLL<T> where T : Record
    {
        /// <summary>
        /// True if the user has access to the record
        /// </summary>
        bool HasAccess(IPrincipal user, T record);

        /// <summary>
        /// True if the user can review this sheet.
        /// This is if they own the record, are an admin, or the user's supervisor
        /// </summary>
        bool HasReviewAccess(IPrincipal user, T record);

        /// <summary>
        /// Returns true if the sheet is in an editable state
        /// </summary>
        bool IsEditable(T record);

        /// <summary>
        /// Return the current time record if it has not been submitted.  If there is not current sheet, create one
        /// </summary>
        /// <param name="user">Name of user who's time record should be found</param>
        /// <returns></returns>
        T GetCurrent(IPrincipal user);

        /// <summary>
        /// Gets the current record. This should only be user internally and for testing
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        T GetCurrentRecord(IPrincipal user);

        DateTime GetCurrentSheetDate();
        
        void Submit(T record, IPrincipal user);

        /// <summary>
        /// Returns all of the reviewable & current records for the given supervisor's reviewees.
        /// </summary>
        /// <remarks>
        /// Criteria for a record being visible
        /// 1) Current user is the supervisor of the record's owner
        /// TODO: 2) Current user is a delegate for a supervisor who supervises the record's owner
        /// </remarks>
        /// <param name="user">The user who will review the record</param>
        IEnumerable<T> GetReviewableAndCurrentRecords(IPrincipal user);
    }
}