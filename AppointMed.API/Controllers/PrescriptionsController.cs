using AppointMed.API.Data;
using AppointMed.API.Models.Prescription;
using AppointMed.API.Repository.Interface;
using AppointMed.API.Static;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointMed.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionRepository repository;
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;
        private readonly ILogger<PrescriptionsController> logger;

        public PrescriptionsController(
            IPrescriptionRepository repository,
            IAccountRepository accountRepository,
            IMapper mapper,
            ILogger<PrescriptionsController> logger)
        {
            this.repository = repository;
            this.accountRepository = accountRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Prescriptions - ADMINS ONLY
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<PrescriptionDto>>> GetPrescriptions()
        {
            try
            {
                var prescriptions = await repository.GetAllPrescriptionsAsync();
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetPrescriptions)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Prescriptions/MyPrescriptions - USER SPECIFIC
        [HttpGet("MyPrescriptions")]
        public async Task<ActionResult<List<PrescriptionDto>>> GetMyPrescriptions()
        {
            try
            {
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var prescriptions = await repository.GetPrescriptionsByUserIdAsync(userId);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetMyPrescriptions)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Prescriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionDto>> GetPrescription(int id)
        {
            try
            {
                var prescription = await repository.GetPrescriptionAsync(id);
                if (prescription == null)
                    return NotFound();

                // Check if user owns this prescription or is admin
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                var isAdmin = User.IsInRole("Administrator");

                if (!isAdmin && prescription.UserId != userId)
                    return Forbid();

                return Ok(prescription);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetPrescription)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Prescriptions/Appointment/5
        [HttpGet("Appointment/{appointmentId}")]
        public async Task<ActionResult<List<PrescriptionDto>>> GetPrescriptionsByAppointment(int appointmentId)
        {
            try
            {
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                var isAdmin = User.IsInRole("Administrator");

                var prescriptions = await repository.GetPrescriptionsByAppointmentIdAsync(appointmentId);

                // Filter by user if not admin
                if (!isAdmin && !string.IsNullOrEmpty(userId))
                {
                    prescriptions = prescriptions.Where(p => p.UserId == userId).ToList();
                }

                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetPrescriptionsByAppointment)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // POST: api/Prescriptions - ADMINS ONLY
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<PrescriptionDto>> PostPrescription(AddPrescriptionDto prescriptionDto)
        {
            try
            {
                var prescription = mapper.Map<Prescription>(prescriptionDto);
                prescription.PrescribedDate = DateTime.UtcNow;
                prescription.IsFulfilled = false;

                await repository.AddAsync(prescription);

                var createdPrescription = await repository.GetPrescriptionAsync(prescription.PrescriptionId);
                return CreatedAtAction(nameof(GetPrescription), new { id = prescription.PrescriptionId }, createdPrescription);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(PostPrescription)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // POST: api/Prescriptions/5/Fulfill
        [HttpPost("{id}/Fulfill")]
        public async Task<IActionResult> FulfillPrescription(int id)
        {
            try
            {
                var prescription = await repository.GetPrescriptionAsync(id);
                if (prescription == null)
                    return NotFound();

                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                var isAdmin = User.IsInRole("Administrator");

                // Only the prescription owner or admin can fulfill
                if (!isAdmin && prescription.UserId != userId)
                    return Forbid();

                if (prescription.IsFulfilled)
                    return BadRequest("Prescription already fulfilled");

                await repository.FulfillPrescriptionAsync(id);

                return Ok(new { message = "Prescription fulfilled successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(FulfillPrescription)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // DELETE: api/Prescriptions/5 - ADMINS ONLY
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            try
            {
                var prescription = await repository.GetAsync(id);
                if (prescription == null)
                    return NotFound();

                // Only refund if prescription was fulfilled
                if (prescription.IsFulfilled)
                {
                    // Get the user's account
                    var account = await accountRepository.GetOrCreateAccountAsync(prescription.UserId);

                    // Refund the prescription cost
                    await accountRepository.RefundPrescriptionAsync(account.AccountId, id);
                }

                // Delete the prescription
                await repository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(DeletePrescription)}: {ex.Message}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
    }
}