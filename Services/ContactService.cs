using ContactApp.Models;

namespace ContactApp.Services
{
    public class ContactService : IContactService
    {
        private static List<Contact> _contacts = new List<Contact>
        {
            new Contact { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "1234567890", Address = "123 Main St" },
            new Contact { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", PhoneNumber = "0987654321", Address = "456 Oak Ave" }
        };

        private static int _nextId = 3;

        public Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            return Task.FromResult<IEnumerable<Contact>>(_contacts);
        }

        public Task<Contact> GetContactByIdAsync(int id)
        {
            var contact = _contacts.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(contact);
        }

        public Task<Contact> CreateContactAsync(Contact contact)
        {
            contact.Id = _nextId++;
            _contacts.Add(contact);
            return Task.FromResult(contact);
        }

        public Task<Contact> UpdateContactAsync(int id, Contact contact)
        {
            var existing = _contacts.FirstOrDefault(c => c.Id == id);
            if (existing != null)
            {
                existing.FirstName = contact.FirstName;
                existing.LastName = contact.LastName;
                existing.Email = contact.Email;
                existing.PhoneNumber = contact.PhoneNumber;
                existing.Address = contact.Address;
            }
            return Task.FromResult(existing);
        }

        public Task<bool> DeleteContactAsync(int id)
        {
            var contact = _contacts.FirstOrDefault(c => c.Id == id);
            if (contact != null)
            {
                _contacts.Remove(contact);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
