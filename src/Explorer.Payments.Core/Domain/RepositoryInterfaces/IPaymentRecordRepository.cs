namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IPaymentRecordRepository
    {
        PaymentRecord Create(PaymentRecord paymentRecord);
        bool ExistsForBundle(int touristId, int bundleId);
    }
}

