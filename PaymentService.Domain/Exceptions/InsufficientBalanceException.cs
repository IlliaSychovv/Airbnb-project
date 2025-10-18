namespace PaymentService.Domain.Exceptions;

public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException()
        : base("Insufficient balance") { }
    
    public InsufficientBalanceException(string message)
        : base(message) { }
}