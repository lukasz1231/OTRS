using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using otrs_backend.Requests;
using otrs_backend.Services;
using System.Security.Claims;

namespace otrs_backend.Controllers
{
    [Route("api/ticket")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _ticketService;

        public TicketController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika.");
            }

            try
            {
                var ticket = await _ticketService.CreateTicketAsync(request, currentUserId);
                return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticket);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika.");
            }

            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id, currentUserId);

                if (ticket == null)
                {
                    return NotFound(new { message = $"Nie znaleziono zgłoszenia o ID {id} lub nie masz do niego uprawnień." });
                }

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika.");
            }

            try
            {
                var tickets = await _ticketService.GetMyTicketsAsync(currentUserId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania zgłoszeń", error = ex.Message });
            }
        }

        [HttpPost("{id}/comment")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddComment(int id, [FromForm] string content, IFormFileCollection files)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Nie można zidentyfikować użytkownika.");
            }

            if (string.IsNullOrWhiteSpace(content) && (files == null || files.Count == 0))
            {
                return BadRequest("Komentarz musi zawierać treść lub przynajmniej jeden załącznik.");
            }

            try
            {
                await _ticketService.AddCommentAsync(id, currentUserId, content, files);
                return Ok(new { message = "Komentarz z załącznikami został dodany pomyślnie." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Helpdesk,Technik")]
        public async Task<IActionResult> ChangeTicketStatus(int id, [FromBody] ChangeStatusRequest request)
        {
            try
            {
                await _ticketService.UpdateStatusAsync(id, request.NewStatus);
                return Ok(new { message = "Status zaktualizowany pomyślnie." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("statuses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatuses()
        {
            try
            {
                var statuses = await _ticketService.GetAllStatusesAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd podczas pobierania statusów", error = ex.Message });
            }
        }

        public class ChangeStatusRequest
        {
            public string NewStatus { get; set; }
        }
    }
}