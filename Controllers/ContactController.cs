
using Microsoft.AspNetCore.Mvc;
using ContactApp.Models;
using ContactApp.Services;

namespace ContactApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAllContacts()
        {
            var contacts = await _contactService.GetAllContactsAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContactById(int id)
        {
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound();
            return Ok(contact);
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact(Contact contact)
        {
            var createdContact = await _contactService.CreateContactAsync(contact);
            return CreatedAtAction(nameof(GetContactById), new { id = createdContact.Id }, createdContact);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Contact>> UpdateContact(int id, Contact contact)
        {
            var updatedContact = await _contactService.UpdateContactAsync(id, contact);
            if (updatedContact == null)
                return NotFound();
            return Ok(updatedContact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var result = await _contactService.DeleteContactAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
