using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class CertificateRepository : GenericRepository<Certificate>, ICertificateRepository
    {
        public CertificateRepository(BloodDonationSupportContext context) : base(context) { }
    }
}
