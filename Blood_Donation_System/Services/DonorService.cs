using Blood_Donation_System.Models;
using Blood_Donation_System.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blood_Donation_System.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorRepository;

        public DonorService(IDonorRepository donorRepository)
        {
            _donorRepository = donorRepository;
        }

        public async Task<IEnumerable<Donor>> GetAllDonorsAsync()
        {
            return await _donorRepository.GetAllAsync();
        }

        public async Task<Donor> GetDonorByIdAsync(Guid id)
        {
            return await _donorRepository.GetByIdAsync(id);
        }

        public async Task<Donor> GetDonorByUserIdAsync(Guid userId)
        {
            return await _donorRepository.GetDonorByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Donor>> GetAvailableDonorsAsync()
        {
            return await _donorRepository.GetAvailableDonorsAsync();
        }

        public async Task<IEnumerable<Donor>> GetDonorsByBloodTypeAsync(Guid bloodTypeId)
        {
            return await _donorRepository.GetDonorsByBloodTypeAsync(bloodTypeId);
        }

        public async Task<bool> AddDonorAsync(Donor donor)
        {
            if (donor == null)
                throw new ArgumentNullException(nameof(donor));
            
            donor.DonorId = Guid.NewGuid();
            await _donorRepository.AddAsync(donor);
            return await _donorRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateDonorAsync(Donor donor)
        {
            if (donor == null)
                throw new ArgumentNullException(nameof(donor));
            
            _donorRepository.Update(donor);
            return await _donorRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteDonorAsync(Guid id)
        {
            var donor = await _donorRepository.GetByIdAsync(id);
            if (donor == null)
                return false;
            
            _donorRepository.Remove(donor);
            return await _donorRepository.SaveChangesAsync();
        }
    }
} 