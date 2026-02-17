using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointMed.API.Data;
using AppointMed.API.Models.Status;
using AppointMed.API.Static;
using AutoMapper;

namespace AppointMed.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly AppointMedDbContext _context;
        private readonly IMapper mapper;
        private readonly ILogger<StatusController> logger;

        public StatusController(AppointMedDbContext context, IMapper mapper, ILogger<StatusController> logger)
        {
            _context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Status
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses()
        {
            try
            {
                var statuses = await _context.Statuses
                    .OrderBy(s => s.DisplayOrder)
                    .ToListAsync();

                var statusDtos = mapper.Map<IEnumerable<StatusDto>>(statuses);
                return Ok(statusDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetStatuses)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Status/Active
        [HttpGet("Active")]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetActiveStatuses()
        {
            try
            {
                var statuses = await _context.Statuses
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.DisplayOrder)
                    .ToListAsync();

                var statusDtos = mapper.Map<IEnumerable<StatusDto>>(statuses);
                return Ok(statusDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetActiveStatuses)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Status/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusDto>> GetStatus(int id)
        {
            try
            {
                var status = await _context.Statuses.FindAsync(id);

                if (status == null)
                {
                    return NotFound();
                }

                var statusDto = mapper.Map<StatusDto>(status);
                return Ok(statusDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetStatus)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // PUT: api/Status/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStatus(int id, UpdateStatusDto statusDto)
        {
            try
            {
                if (id != statusDto.Id)
                {
                    return BadRequest("Status ID mismatch");
                }

                var status = await _context.Statuses.FindAsync(id);

                if (status == null)
                {
                    return NotFound();
                }

                mapper.Map(statusDto, status);
                _context.Entry(status).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusExists(id))
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
                logger.LogError(ex, $"Error Performing PUT in {nameof(PutStatus)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // POST: api/Status
        [HttpPost]
        public async Task<ActionResult<StatusDto>> PostStatus(AddStatusDto statusDto)
        {
            try
            {
                var status = mapper.Map<Status>(statusDto);
                _context.Statuses.Add(status);
                await _context.SaveChangesAsync();

                var createdStatusDto = mapper.Map<StatusDto>(status);
                return CreatedAtAction(nameof(GetStatus), new { id = status.StatusId }, createdStatusDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing POST in {nameof(PostStatus)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // DELETE: api/Status/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            try
            {
                var status = await _context.Statuses.FindAsync(id);
                if (status == null)
                {
                    return NotFound();
                }

                // Check if status is being used by any appointments
                var hasAppointments = await _context.Appointments
                    .AnyAsync(a => a.StatusId == id);

                if (hasAppointments)
                {
                    return BadRequest("Cannot delete status that is currently assigned to appointments. Please reassign appointments first.");
                }

                _context.Statuses.Remove(status);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteStatus)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        private bool StatusExists(int id)
        {
            return _context.Statuses.Any(e => e.StatusId == id);
        }
    }
}
