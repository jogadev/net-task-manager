using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Services;
using TaskManager.Models;
using Microsoft.Azure.Cosmos.Linq;

namespace TaskManager.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }


        [HttpGet]
        public ActionResult<List<User>> Get() => _service.Get();

        [HttpGet("/{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            User obtainedUser = _service.Get(id);
            if (obtainedUser == null)
                return NotFound();
            else
                return obtainedUser;
        }

        [HttpPost]
        public ActionResult<User> Create(User user)
        {
            _service.Create(user);
            return new CreatedAtRouteResult("GetUser", new { id = user.Id.ToString() }, user);
        }

        [HttpPut]
        public IActionResult Update(string id, User userIn)
        {
            var targetUser = _service.Get(id);
            if(targetUser == null)
                return NotFound();
            _service.Update(id, userIn);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var targetUser = _service.Get(id);
            if (targetUser == null)
                return NotFound();

            _service.Remove(targetUser.Id);
            return NoContent();
        }


    }
}
