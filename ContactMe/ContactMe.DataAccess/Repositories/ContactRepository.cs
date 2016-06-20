using ContactMe.DataAccess.Models;
using System;

namespace ContactMe.DataAccess.Repositories
{
    /// <summary>
    /// Handles data access for the Contacts table.
    /// </summary>
    public class ContactRepository : IContactRepository
    {
        /// <summary>
        /// Inserts a contact to the database.
        /// </summary>
        /// <returns>A bool indicating success or failure.</returns>
        public bool Insert(Contact contact)
        {
            try
            {
                // Generate a random guid for the id
                if (contact.ContactID == Guid.Empty)
                {
                    contact.ContactID = Guid.NewGuid();
                }

                // Add the new contact to the database
                // Since it's an insert, we expect exactly 1 row to change,
                // anything else indicates something went wrong.
                using (var db = new ContactMeEntities())
                {
                    db.Contacts.Add(contact);
                    return db.SaveChanges() == 1;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
