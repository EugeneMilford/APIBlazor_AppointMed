using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointMed.API.Models.Appointment;
using AppointMed.API.Static;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using AppointMed.API.Repository.Interface;
using AppointMed.API.Data;

namespace AppointMed.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentRepository repository;
        private readonly IAccountRepository accountRepository;
        private readonly IPrescriptionRepository prescriptionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<AppointmentsController> logger;

        public AppointmentsController(
            IAppointmentRepository repository,
            IAccountRepository accountRepository,
            IPrescriptionRepository prescriptionRepository,
            IMapper mapper,
            ILogger<AppointmentsController> logger)
        {
            this.repository = repository;
            this.accountRepository = accountRepository;
            this.prescriptionRepository = prescriptionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
        {
            try
            {
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                var isAdmin = User.IsInRole("Administrator");

                List<AppointmentDto> appointmentDtos;

                if (isAdmin)
                {
                    // Admin sees all appointments
                    appointmentDtos = await repository.GetAllAppointmentsAsync();
                }
                else
                {
                    // Regular users see only their own appointments
                    if (string.IsNullOrEmpty(userId))
                        return Unauthorized();

                    appointmentDtos = await repository.GetAppointmentsByUserIdAsync(userId);
                }

                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetAppointments)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            try
            {
                var appointmentDto = await repository.GetAppointmentAsync(id);

                if (appointmentDto == null)
                {
                    return NotFound();
                }

                // Check if user owns this appointment or is admin
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                var isAdmin = User.IsInRole("Administrator");

                if (!isAdmin && appointmentDto.UserId != userId)
                {
                    return Forbid();
                }

                return Ok(appointmentDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetAppointment)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Appointments/Doctor/5
        [HttpGet("Doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                var isAdmin = User.IsInRole("Administrator");

                var appointmentDtos = await repository.GetAppointmentsByDoctorAsync(doctorId);

                // Filter by user if not admin
                if (!isAdmin && !string.IsNullOrEmpty(userId))
                {
                    appointmentDtos = appointmentDtos.Where(a => a.UserId == userId).ToList();
                }

                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetAppointmentsByDoctor)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Appointments/Status/5
        [HttpGet("Status/{statusId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByStatus(int statusId)
        {
            try
            {
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                var isAdmin = User.IsInRole("Administrator");

                var appointmentDtos = await repository.GetAppointmentsByStatusAsync(statusId);

                // Filter by user if not admin
                if (!isAdmin && !string.IsNullOrEmpty(userId))
                {
                    appointmentDtos = appointmentDtos.Where(a => a.UserId == userId).ToList();
                }

                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetAppointmentsByStatus)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Appointments/ForUpdate/5
        [HttpGet("ForUpdate/{id}")]
        public async Task<ActionResult<UpdateAppointmentDto>> GetAppointmentForUpdate(int id)
        {
            try
            {
                var appointment = await repository.GetAsync(id);

                if (appointment == null)
                {
                    return NotFound();
                }

                // Check if user owns this appointment or is admin
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                var isAdmin = User.IsInRole("Administrator");

                if (!isAdmin && appointment.UserId != userId)
                {
                    return Forbid();
                }

                var updateDto = mapper.Map<UpdateAppointmentDto>(appointment);
                return Ok(updateDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetAppointmentForUpdate)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // PUT: api/Appointments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, UpdateAppointmentDto appointmentDto)
        {
            try
            {
                if (id != appointmentDto.Id)
                {
                    return BadRequest("Appointment ID mismatch");
                }

                var appointment = await repository.GetAsync(id);

                if (appointment == null)
                {
                    return NotFound();
                }

                // Check if user owns this appointment or is admin
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                var isAdmin = User.IsInRole("Administrator");

                if (!isAdmin && appointment.UserId != userId)
                {
                    return Forbid();
                }

                mapper.Map(appointmentDto, appointment);
                appointment.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await repository.UpdateAsync(appointment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await repository.Exists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing PUT in {nameof(PutAppointment)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> PostAppointment(AddAppointmentDto addAppointmentDto)
        {
            try
            {
                // Get current user ID from JWT token
                var userId = User.FindFirst(CustomClaimTypes.Uid)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token");
                }

                // Create the appointment entity
                var appointment = mapper.Map<Appointment>(addAppointmentDto);
                appointment.UserId = userId;
                appointment.CreatedAt = DateTime.UtcNow;

                await repository.AddAsync(appointment);

                var createdAppointmentDto = await repository.GetAppointmentAsync(appointment.AppointmentId);

                // Get or create user account
                var account = await accountRepository.GetOrCreateAccountAsync(userId);

                // Create transaction with the appointment ID
                await accountRepository.AddTransactionAsync(
                    accountId: account.AccountId,
                    transactionType: "Appointment",
                    amount: 200,
                    description: $"Appointment with {createdAppointmentDto.DoctorName} on {appointment.AppointmentDateTime:dd/MM/yyyy HH:mm}",
                    appointmentId: createdAppointmentDto.AppointmentId,
                    prescriptionId: null
                );

                return CreatedAtAction(
                    nameof(GetAppointment),
                    new { id = createdAppointmentDto.AppointmentId },
                    createdAppointmentDto
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing POST in {nameof(PostAppointment)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            try
            {
                var appointment = await repository.GetAsync(id);
                if (appointment == null)
                {
                    return NotFound();
                }

                // Get the user's account
                var account = await accountRepository.GetOrCreateAccountAsync(appointment.UserId);

                // STEP 1: Handle related prescriptions first
                var prescriptions = await prescriptionRepository.GetPrescriptionEntitiesByAppointmentIdAsync(id);

                foreach (var prescription in prescriptions)
                {
                    // Refund fulfilled prescriptions
                    if (prescription.IsFulfilled)
                    {
                        await accountRepository.RefundPrescriptionAsync(account.AccountId, prescription.PrescriptionId);
                    }

                    // Delete the prescription
                    await prescriptionRepository.DeleteAsync(prescription.PrescriptionId);
                }

                // STEP 2: Refund the appointment charge
                await accountRepository.RefundTransactionAsync(account.AccountId, id);

                // STEP 3: Now safe to delete the appointment
                await repository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error deleting appointment {id}: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the appointment.");
            }
        }
    }
}