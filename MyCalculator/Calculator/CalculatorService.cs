using MyCalculator.DAL;
using System.Globalization;
using System.Text;
using static MyCalculator.Calculator.ICalculatorService;

namespace MyCalculator.Calculator
{
    public class CalculatorService : ICalculatorService
    {
        private NumberFormatInfo Nfi;
        private StringBuilder operandStringBuilder;
        private List<double> operands;
        private OperationType operationType;
        private StringBuilder _calculatorTextBuilder;
        private IDataAccess _dataAccessService;
        private List<string> _computations;
        private bool DidComputationHappened;

        public string CalculatorText { get { return _calculatorTextBuilder.ToString(); } }
        public IList<string> Errors { get; }
        public IList<string> Computations { get { return _computations; } }
        public bool IsRoundedToInt { get; set; }

        public CalculatorService(IDataAccess dataAccessService)
        {
            Nfi = new NumberFormatInfo();
            Nfi.NumberDecimalSeparator = ".";

            DidComputationHappened = false;
            IsRoundedToInt = false;
            operandStringBuilder = new StringBuilder();
            operands = new List<double>();
            operationType = OperationType.NotDefined;
            _calculatorTextBuilder = new StringBuilder();
            Errors = new List<string>();
            _computations = new List<string>();
            _dataAccessService = dataAccessService;

            // Load computations history
            _computations = new List<string>(_dataAccessService.GetComputationsHistory());
        }

        /// <summary>
        /// Set calculator to default
        /// </summary>
        public void Cancel()
        {
            operandStringBuilder.Clear();
            operands.Clear();
            operationType = OperationType.NotDefined;
            _calculatorTextBuilder.Clear();
            Errors.Clear();
            DidComputationHappened = false;
        }

        private void SendError(Exception ex)
        {
            // Show exception to UI
            Errors.Add(ex.Message);

            // Log exception to file
            StringBuilder sb = new StringBuilder();
            sb.Append(ex.Message);
            File.AppendAllText("ErrorLog.txt", DateTime.Now.ToString() + " -- " + sb.ToString() + Environment.NewLine);
            sb.Clear();
        }

        public void Compute()
        {
            try
            {
                // Create last operand (in this implmentation SECOND one)
                CreateOperand();

                string result = string.Empty;
                if (operands.Count == 2)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(_calculatorTextBuilder.ToString());

                    result = PerformComputation();

                    sb.Append(" = ");
                    sb.Append(result);

                    // Save computation to DAL
                    _dataAccessService.SaveComputation(sb.ToString());

                    // Reload computations history
                    _computations = new List<string>(_dataAccessService.GetComputationsHistory());
                }
                else
                {
                    _calculatorTextBuilder.Clear();
                    throw new Exception("Calculator only two operands. Either none, one or more than two operands inserted.");
                }

                // Set calculator to default, keep resulting value
                operandStringBuilder.Clear();
                operandStringBuilder.Append(result);
                operands.Clear();
                operationType = OperationType.NotDefined;
                Errors.Clear();
                DidComputationHappened = true;
            }
            catch (Exception ex)
            {
                SendError(ex);
            }
        }

        private string PerformComputation()
        {
            double result = 0;

            switch (operationType)
            {
                case OperationType.Add:
                    result = operands[0] + operands[1];
                    break;

                case OperationType.Deduct:
                    result = operands[0] - operands[1];
                    break;

                case OperationType.Divide:
                    if (operands[1] != 0)
                    {
                        result = (double)operands[0] / (double)operands[1];
                    }
                    else
                    {
                        throw new Exception("Division by zero is not defined.");
                    }
                    break;

                case OperationType.Multiply:
                    result = operands[0] * operands[1];
                    break;

                case OperationType.NotDefined:
                    _calculatorTextBuilder.Clear();
                    _calculatorTextBuilder.Append(" OperationNotDef ");
                    throw new Exception("Method PerformComputation() - undefined calculator operation.");
            }

            // Rounding if required
            if (IsRoundedToInt)
            {
                result = Math.Round(result, MidpointRounding.AwayFromZero);

            }

            _calculatorTextBuilder.Clear();

            _calculatorTextBuilder.Append(result.ToString(Nfi));

            return result.ToString(Nfi);
        }

        public void InsertDecimalPoint()
        {
            if (DidComputationHappened)
            {
                Cancel();
            }

            if (operandStringBuilder.Length > 0)
            {
                // Add "." only if there is no decimal point yet
                if (!operandStringBuilder.ToString().Contains('.'))
                {
                    operandStringBuilder.Append('.');
                    _calculatorTextBuilder.Append('.');
                }
            }
            else
            {
                // Add "0." if operand string builder is empty
                operandStringBuilder.Append("0.");
                _calculatorTextBuilder.Append("0.");
            }
        }

        public void InsertDigit(int digit)
        {
            try
            {
                if (DidComputationHappened)
                {
                    Cancel();
                }

                if (digit >= 0 && digit <= 9)
                {
                    operandStringBuilder.Append(digit.ToString());
                    _calculatorTextBuilder.Append(digit.ToString());
                }
                else
                {
                    throw new Exception($"Method InsertDigit() accepts only 0-9 digits. Inserted value is {digit}.");
                }
            }
            catch (Exception ex)
            {
                SendError(ex);
            }
        }

        public void InsertOperation(OperationType insertedOperationType)
        {
            if (DidComputationHappened)
            {
                operands.Clear();
                operationType = OperationType.NotDefined;
                _calculatorTextBuilder.Clear();
                _calculatorTextBuilder.Append(operandStringBuilder.ToString());
                Errors.Clear();
                DidComputationHappened = false;
            }

            try
            {
                if (operationType == OperationType.NotDefined)
                {
                    switch (insertedOperationType)
                    {
                        case OperationType.Add:
                            _calculatorTextBuilder.Append(" + ");
                            break;
                        case OperationType.Deduct:
                            _calculatorTextBuilder.Append(" - ");
                            break;
                        case OperationType.Divide:
                            _calculatorTextBuilder.Append(" / ");
                            break;
                        case OperationType.Multiply:
                            _calculatorTextBuilder.Append(" * ");
                            break;
                        case OperationType.NotDefined:
                            _calculatorTextBuilder.Append(" OperationNotDef ");
                            throw new Exception("Method InsertOperation() inserted undefined calculator operation.");
                    }

                    operationType = insertedOperationType;

                    // Inserting "operationType" means finishing building process of the preceding operand -> create that operand (in this implementation it means only the FIRST operand)
                    CreateOperand();
                }
                else
                {
                    throw new Exception("Operation already inserted.");
                }

            }
            catch (Exception ex)
            {
                SendError(ex);
            }
        }

        private void CreateOperand()
        {
            // Set limit for one operand to 15 chars
            if (operandStringBuilder.Length <= 15)
            {
                // Convert operand string builder to double
                double operand = ConvertOperandToDouble(operandStringBuilder.ToString());

                // Add operand to operand list
                operands.Add(operand);

                // Clear operand string builder to be ready for next number
                operandStringBuilder.Clear();
            }
            else
            {
                throw new Exception("Max length limit for one operand is 15.");
            }
        }

        private double ConvertOperandToDouble(string operandString)
        {
            double result = 0;

            if (!string.IsNullOrEmpty(operandString))
            {
                // Remove decimal point if operand ends with that
                if (operandString.EndsWith("."))
                {
                    operandString.Remove(operandString.Length - 1);
                }

                result = Convert.ToDouble(operandString, CultureInfo.InvariantCulture);
            }

            return result;
        }
    }
}
