using AppointMed.API.Data;
using AppointMed.API.Models.Medicine;
using AppointMed.API.Repository.Interface;
using AppointMed.API.Static;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointMed.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicinesController : ControllerBase
    {
        private readonly IMedicineRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<MedicinesController> logger;

        public MedicinesController(IMedicineRepository repository, IMapper mapper, ILogger<MedicinesController> logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Medicines - ALL USERS CAN VIEW
        [HttpGet]
        public async Task<ActionResult<List<MedicineDto>>> GetMedicines()
        {
            try
            {
                var medicines = await repository.GetAllMedicinesAsync();
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetMedicines)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Medicines/Available - ALL USERS CAN VIEW
        [HttpGet("Available")]
        public async Task<ActionResult<List<MedicineDto>>> GetAvailableMedicines()
        {
            try
            {
                var medicines = await repository.GetAvailableMedicinesAsync();
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetAvailableMedicines)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Medicines/5 - ALL USERS CAN VIEW
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicineDto>> GetMedicine(int id)
        {
            try
            {
                var medicine = await repository.GetMedicineDetailsAsync(id);
                if (medicine == null)
                    return NotFound();

                return Ok(medicine);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(GetMedicine)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // POST: api/Medicines - ADMINS ONLY
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<MedicineDto>> PostMedicine(AddMedicineDto medicineDto)
        {
            try
            {
                var medicine = mapper.Map<Medicine>(medicineDto);
                await repository.AddAsync(medicine);

                var createdMedicine = mapper.Map<MedicineDto>(medicine);
                return CreatedAtAction(nameof(GetMedicine), new { id = medicine.MedicineId }, createdMedicine);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(PostMedicine)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // PUT: api/Medicines/5 - ADMINS ONLY
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutMedicine(int id, UpdateMedicineDto medicineDto)
        {
            try
            {
                if (id != medicineDto.Id)
                    return BadRequest("Medicine ID mismatch");

                var medicine = await repository.GetAsync(id);
                if (medicine == null)
                    return NotFound();

                mapper.Map(medicineDto, medicine);

                try
                {
                    await repository.UpdateAsync(medicine);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await repository.Exists(id))
                        return NotFound();
                    throw;
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(PutMedicine)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // DELETE: api/Medicines/5 - ADMINS ONLY
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            try
            {
                if (!await repository.Exists(id))
                    return NotFound();

                await repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in {nameof(DeleteMedicine)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
    }
}