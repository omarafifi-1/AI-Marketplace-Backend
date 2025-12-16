using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AI_Marketplace.Application.ChatSessions.DTOs;
using MediatR;
using AI_Marketplace.Application.ChatSessions.Queries.GetSession;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Application.ChatSessions.Queries.GetMessages;
using AI_Marketplace.Application.ChatSessions.Commands.AddMessages;
using AI_Marketplace.Application.ChatSessions.Commands.AddSession;
using System.Security.Claims;
using System.Threading.Tasks;
namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatSessionController : ControllerBase
    {
        private readonly ISender _send;
        public ChatSessionController(ISender send)
        {
            _send = send;
        }
        [HttpGet("GetSessionById")]
        public async Task<ActionResult> GetSessionById(GetSessionQuery Query)
        {
            GetSessionDTO sessionDTO = await _send.Send(Query);
            if (sessionDTO != null)
            {
                return Ok(sessionDTO);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("GetAllMessages")]
        public async Task<ActionResult> GetAllMessages(GetAllMessagesQuery Query)
        {
            ICollection<GetMessageDTO> messagesDTO = await _send.Send(Query);
            if (messagesDTO != null)
            {
                return Ok(messagesDTO);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost("AddMessage")]
        public async Task<IActionResult> AddMessage(AddMessageCommand Command)
        {
            if(Command==null)
            {
                return BadRequest("Empty Entry");
            }
            else
            {
                await _send.Send(Command);
                return Ok();
            }
        }
        [HttpPost("AddAllMessages")]
        public async Task<IActionResult> AddAllMesages(AddMessagesCommand command)
        {
            if (command.messagesDTO != null)
            {
                return Ok(await _send.Send(command.messagesDTO));
            }
            else
            {
                return BadRequest("Empty Entry");
            }
        }
        [HttpPost("AddSession")]
        public async Task<IActionResult> AddSession(AddSessionCommand command)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserIdString == null) 
            {
                return Unauthorized();
            }
            else
            {
                var UserIdInt = int.Parse(UserIdString);
                Console.WriteLine(UserIdInt);
                if (command.SessionDTO != null)
                {
                    command.SessionDTO.UserId = UserIdInt;

                    int sessionId = await _send.Send(command);
                    return Ok(sessionId);
                }
                else
                {
                    return BadRequest("Empty Entry");
                }
            }
        }
        [HttpGet("GetAllSessionsByUserId")]
        public async Task<IActionResult> GetAllSessionsByUserId()
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserIdString == null)
            {
                return Unauthorized("this user isn't authorized");
            }
            else
            {
                var UserIdInt = int.Parse(UserIdString);
                GetSessionsQuery query = new GetSessionsQuery { UserId=UserIdInt};
                return Ok(await _send.Send(query));
            }
        }
    }
}
