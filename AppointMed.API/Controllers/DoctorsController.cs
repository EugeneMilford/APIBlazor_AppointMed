using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointMed.API.Models.Doctor;
using AppointMed.API.Static;
using AutoMapper;
using AppointMed.API.Repository.Interface;
using AppointMed.API.Data;

namespace AppointMed.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorsController> logger;

        public DoctorsController(IDoctorRepository repository, IMapper mapper, ILogger<DoctorsController> logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Doctors
        [HttpGet]
        public async Task<ActionResult<List<DoctorDto>>> GetDoctors()
        {
            try
            {
                return await repository.GetAllDoctorsAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetDoctors)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
        {
            try
            {
                var doctorDto = await repository.GetDoctorDetailsAsync(id);

                if (doctorDto == null)
                {
                    return NotFound();
                }

                return Ok(doctorDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing GET in {nameof(GetDoctor)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // PUT: api/Doctors/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutDoctor(int id, UpdateDoctorDto doctorDto)
        {
            try
            {
                if (id != doctorDto.Id)
                {
                    return BadRequest("Doctor ID mismatch");
                }

                var doctor = await repository.GetAsync(id);

                if (doctor == null)
                {
                    return NotFound();
                }

                mapper.Map(doctorDto, doctor);

                try
                {
                    await repository.UpdateAsync(doctor);
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
                logger.LogError(ex, $"Error Performing PUT in {nameof(PutDoctor)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // POST: api/Doctors
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<DoctorDto>> PostDoctor(AddDoctorDto doctorDto)
        {
            try
            {
                var doctor = mapper.Map<Doctor>(doctorDto);
                await repository.AddAsync(doctor);

                var createdDoctorDto = mapper.Map<DoctorDto>(doctor);
                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.DoctorId }, createdDoctorDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing POST in {nameof(PostDoctor)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // DELETE: api/Doctors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                if (!await repository.Exists(id))
                {
                    return NotFound();
                }

                await repository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteDoctor)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
    }
}
