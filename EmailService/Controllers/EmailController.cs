using EmailService.DTO;
using EmailService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/emails")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _sender;

        public EmailController(IEmailSender sender)
        {
            _sender = sender;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] EmailEvent request)
        {
            await _sender.SendAsync(request);
            return Ok();
        }
    }

}
