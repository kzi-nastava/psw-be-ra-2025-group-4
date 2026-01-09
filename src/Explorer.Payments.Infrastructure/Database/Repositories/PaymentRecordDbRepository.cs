using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class PaymentRecordDbRepository : IPaymentRecordRepository
    {
        private readonly PaymentsContext _dbContext;

        public PaymentRecordDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PaymentRecord Create(PaymentRecord paymentRecord)
        {
            _dbContext.PaymentRecords.Add(paymentRecord);
            _dbContext.SaveChanges();
            return paymentRecord;
        }
    }
}

