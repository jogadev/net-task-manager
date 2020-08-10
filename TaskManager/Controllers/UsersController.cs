using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Services;
using TaskManager.Models;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

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
        public ActionResult<List<User>> Get() {
            Console.WriteLine("End of pipeline");
            return _service.Get(); 
        }

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
        public JsonResult Create(User user)
        {
            JsonResult result;
            Dictionary<string, string> jsonResponse;
            Dictionary<string, Object> createResult = _service.Create(user);
            if (createResult.ContainsKey("error")) {
                jsonResponse = new Dictionary<string, string> {
                    { "error", (string) createResult["error"] }
                };
                result = new JsonResult(jsonResponse) { StatusCode = 400 };
            }
            else
            {
                user = (User) createResult["user"];
                user.Tokens = new List<string>();
                var jwt = TokenManager.CreateTokenFor(user.Id);
                user.Tokens.Add(jwt);
                _service.Update(user.Id, user);
                jsonResponse = new Dictionary<string, string>
                {
                    { "token", jwt }
                };
                result = new JsonResult(jsonResponse) { StatusCode = 201 };
            }

            return result;
        }

        [HttpPatch("me")]
        public IActionResult Update(User user)
        {
            string id = (string)HttpContext.Items["_id"];
            var targetUser = _service.Get(id);
            // Update allowed values: age, name, email, password
            targetUser.Age = user.Age;
            if (user.Name != null)
                targetUser.Name = user.Name;
            if (user.Email != null)
                targetUser.Email = user.Email;
            if(user.Password != null)
                targetUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _service.Update(id, targetUser);
            return new OkResult();
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
