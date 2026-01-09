namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IPaymentRecordRepository
    {
        PaymentRecord Create(PaymentRecord paymentRecord);
    }
}

