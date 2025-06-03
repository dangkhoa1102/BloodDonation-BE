using Blood_Donation_System.DTOs.Donor;
using Blood_Donation_System.Models;
using Blood_Donation_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blood_Donation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorsController : ControllerBase
    {
        private readonly IDonorService _donorService;
        private readonly IUserService _userService;

        public DonorsController(IDonorService donorService, IUserService userService)
        {
            _donorService = donorService;
            _userService = userService;
        }

        // GET: api/Donors
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DonorDTO>>> GetDonors()
        {
            var donors = await _donorService.GetAllDonorsAsync();
            var donorDtos = donors.Select(d => new DonorDTO
            {
                DonorId = d.DonorId,
                UserId = d.UserId,
                FullName = d.User?.FullName,
                BloodTypeId = d.BloodTypeId,
                BloodType = d.BloodType != null ? $"{d.BloodType.AboType}{d.BloodType.RhFactor}" : null,
                Weight = d.Weight,
                Height = d.Height,
                MedicalHistory = d.MedicalHistory,
                IsAvailable = d.IsAvailable,
                LastDonationDate = d.LastDonationDate,
                NextEligibleDate = d.NextEligibleDate,
                LocationId = d.LocationId,
                Address = d.Location?.Address,
                ClosestFacilityId = d.ClosestFacilityId,
                FacilityName = d.ClosestFacility?.FacilityName
            });
            
            return Ok(donorDtos);
        }

        // GET: api/Donors/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<DonorDTO>> GetDonor(Guid id)
        {
            var donor = await _donorService.GetDonorByIdAsync(id);

            if (donor == null)
            {
                return NotFound();
            }

            var donorDto = new DonorDTO
            {
                DonorId = donor.DonorId,
                UserId = donor.UserId,
                FullName = donor.User?.FullName,
                BloodTypeId = donor.BloodTypeId,
                BloodType = donor.BloodType != null ? $"{donor.BloodType.AboType}{donor.BloodType.RhFactor}" : null,
                Weight = donor.Weight,
                Height = donor.Height,
                MedicalHistory = donor.MedicalHistory,
                IsAvailable = donor.IsAvailable,
                LastDonationDate = donor.LastDonationDate,
                NextEligibleDate = donor.NextEligibleDate,
                LocationId = donor.LocationId,
                Address = donor.Location?.Address,
                ClosestFacilityId = donor.ClosestFacilityId,
                FacilityName = donor.ClosestFacility?.FacilityName
            };

            return Ok(donorDto);
        }

        // GET: api/Donors/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<DonorDTO>> GetDonorByUserId(Guid userId)
        {
            var donor = await _donorService.GetDonorByUserIdAsync(userId);

            if (donor == null)
            {
                return NotFound();
            }

            var donorDto = new DonorDTO
            {
                DonorId = donor.DonorId,
                UserId = donor.UserId,
                FullName = donor.User?.FullName,
                BloodTypeId = donor.BloodTypeId,
                BloodType = donor.BloodType != null ? $"{donor.BloodType.AboType}{donor.BloodType.RhFactor}" : null,
                Weight = donor.Weight,
                Height = donor.Height,
                MedicalHistory = donor.MedicalHistory,
                IsAvailable = donor.IsAvailable,
                LastDonationDate = donor.LastDonationDate,
                NextEligibleDate = donor.NextEligibleDate,
                LocationId = donor.LocationId,
                Address = donor.Location?.Address,
                ClosestFacilityId = donor.ClosestFacilityId,
                FacilityName = donor.ClosestFacility?.FacilityName
            };

            return Ok(donorDto);
        }

        // GET: api/Donors/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<DonorDTO>>> GetAvailableDonors()
        {
            var donors = await _donorService.GetAvailableDonorsAsync();
            var donorDtos = donors.Select(d => new DonorDTO
            {
                DonorId = d.DonorId,
                UserId = d.UserId,
                FullName = d.User?.FullName,
                BloodTypeId = d.BloodTypeId,
                BloodType = d.BloodType != null ? $"{d.BloodType.AboType}{d.BloodType.RhFactor}" : null,
                Weight = d.Weight,
                Height = d.Height,
                MedicalHistory = d.MedicalHistory,
                IsAvailable = d.IsAvailable,
                LastDonationDate = d.LastDonationDate,
                NextEligibleDate = d.NextEligibleDate,
                LocationId = d.LocationId,
                Address = d.Location?.Address,
                ClosestFacilityId = d.ClosestFacilityId,
                FacilityName = d.ClosestFacility?.FacilityName
            });
            
            return Ok(donorDtos);
        }

        // GET: api/Donors/bloodtype/5
        [HttpGet("bloodtype/{bloodTypeId}")]
        public async Task<ActionResult<IEnumerable<DonorDTO>>> GetDonorsByBloodType(Guid bloodTypeId)
        {
            var donors = await _donorService.GetDonorsByBloodTypeAsync(bloodTypeId);
            var donorDtos = donors.Select(d => new DonorDTO
            {
                DonorId = d.DonorId,
                UserId = d.UserId,
                FullName = d.User?.FullName,
                BloodTypeId = d.BloodTypeId,
                BloodType = d.BloodType != null ? $"{d.BloodType.AboType}{d.BloodType.RhFactor}" : null,
                Weight = d.Weight,
                Height = d.Height,
                MedicalHistory = d.MedicalHistory,
                IsAvailable = d.IsAvailable,
                LastDonationDate = d.LastDonationDate,
                NextEligibleDate = d.NextEligibleDate,
                LocationId = d.LocationId,
                Address = d.Location?.Address,
                ClosestFacilityId = d.ClosestFacilityId,
                FacilityName = d.ClosestFacility?.FacilityName
            });
            
            return Ok(donorDtos);
        }

        // POST: api/Donors
        [HttpPost]
        public async Task<ActionResult<DonorDTO>> CreateDonor(DonorCreateDTO donorCreateDto)
        {
            var user = await _userService.GetUserByIdAsync(donorCreateDto.UserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var donor = new Donor
            {
                UserId = donorCreateDto.UserId,
                BloodTypeId = donorCreateDto.BloodTypeId,
                Weight = donorCreateDto.Weight,
                Height = donorCreateDto.Height,
                MedicalHistory = donorCreateDto.MedicalHistory,
                IsAvailable = donorCreateDto.IsAvailable,
                LocationId = donorCreateDto.LocationId,
                ClosestFacilityId = donorCreateDto.ClosestFacilityId
            };

            var result = await _donorService.AddDonorAsync(donor);

            if (!result)
            {
                return BadRequest("Failed to create donor");
            }

            var donorDto = new DonorDTO
            {
                DonorId = donor.DonorId,
                UserId = donor.UserId,
                FullName = user.FullName,
                BloodTypeId = donor.BloodTypeId,
                Weight = donor.Weight,
                Height = donor.Height,
                MedicalHistory = donor.MedicalHistory,
                IsAvailable = donor.IsAvailable,
                LocationId = donor.LocationId,
                ClosestFacilityId = donor.ClosestFacilityId
            };

            return CreatedAtAction(nameof(GetDonor), new { id = donor.DonorId }, donorDto);
        }

        // PUT: api/Donors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonor(Guid id, DonorUpdateDTO donorUpdateDto)
        {
            var donor = await _donorService.GetDonorByIdAsync(id);

            if (donor == null)
            {
                return NotFound();
            }

            donor.BloodTypeId = donorUpdateDto.BloodTypeId ?? donor.BloodTypeId;
            donor.Weight = donorUpdateDto.Weight ?? donor.Weight;
            donor.Height = donorUpdateDto.Height ?? donor.Height;
            donor.MedicalHistory = donorUpdateDto.MedicalHistory ?? donor.MedicalHistory;
            donor.IsAvailable = donorUpdateDto.IsAvailable ?? donor.IsAvailable;
            donor.LocationId = donorUpdateDto.LocationId ?? donor.LocationId;
            donor.ClosestFacilityId = donorUpdateDto.ClosestFacilityId ?? donor.ClosestFacilityId;

            var result = await _donorService.UpdateDonorAsync(donor);

            if (!result)
            {
                return BadRequest("Failed to update donor");
            }

            return NoContent();
        }

        // DELETE: api/Donors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonor(Guid id)
        {
            var result = await _donorService.DeleteDonorAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
} 