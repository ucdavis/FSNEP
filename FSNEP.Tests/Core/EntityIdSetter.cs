using System.Reflection;
using UCDArch.Core.Utils;

namespace FSNEP.Tests.Core
{
    /*
    /// <summary>
    /// For better data integrity, it is imperitive that the DomainObject<T,IdT>.id
    /// property is read-only and set only by the ORM.  With that said, some unit tests need 
    /// Id set to a particular value; therefore, this utility enables that ability.  This class should 
    /// never be used outside of the testing project.
    /// </summary>
    public static class EntityIdSetter
    {
        /// <summary>
        /// Uses reflection to set the Id of a DomainObject
        /// </summary>
        public static void SetIdOf<T,IdT>(DomainObject<T,IdT> entity, IdT id)
        {
            // Set the data property reflectively
            var idProperty = entity.GetType().GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
            
            Check.Ensure(idProperty != null, "idProperty could not be found");

            idProperty.SetValue(entity, id);
        }

        /// <summary>
        /// Extension method that uses reflection to set the Id of a DomainObject
        /// </summary>
        public static object SetIdTo<T,IdT>(this DomainObject<T,IdT> entity, IdT id)
        {
            SetIdOf(entity, id);
            return entity;
        }
    }
     */
}