using ContactMe.DataAccess.Models;

namespace ContactMe.DataAccess.Repositories
{
    /// <summary>
    /// Handles data access for the Contacts table.
    /// </summary>
    public interface IContactRepository
    {
        /// <summary>
        /// Inserts a contact to the database.
        /// </summary>
        /// <returns>A bool indicating success or failure.</returns>
        bool Insert(Contact contact);
    }
}
