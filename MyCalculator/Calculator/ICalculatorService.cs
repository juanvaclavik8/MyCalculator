namespace MyCalculator.Calculator
{
    public interface ICalculatorService
    {
        enum OperationType
        {
            Add, Deduct, Divide, Multiply, NotDefined
        }

        string CalculatorText { get; }
        IList<string> Errors { get; }
        IList<string> Computations { get; }
        bool IsRoundedToInt { get; set; }

        void InsertDigit(int i);
        void InsertOperation(OperationType opType);
        void InsertDecimalPoint();
        void Cancel();
        void Compute();
    }
}
