using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Services
{
    public class DonationHistoryService : IDonationHistoryService
    {
        private readonly IDonationHistoryRepository _repository;

        public DonationHistoryService(IDonationHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DonationHistory>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<DonationHistory?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<DonationHistory> AddAsync(DonationHistory history)
        {
            await _repository.AddAsync(history);
            return history;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.DeleteAsync(existing);
            return true;
        }
    }
}
