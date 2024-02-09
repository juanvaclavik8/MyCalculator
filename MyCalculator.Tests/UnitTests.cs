using Moq;
using MyCalculator.Calculator;
using MyCalculator.DAL;
using static MyCalculator.Calculator.ICalculatorService;

namespace MyCalculator.Tests
{
    public class UnitTests
    {
        [Fact]
        public void Compute_WhenAddition_ReturnsValue()
        {
            // Arrange
            string expected = "46";
            var mockDataAccessService = new Mock<IDataAccess>();
            mockDataAccessService.Setup(x => x.GetComputationsHistory()).Returns(new List<string>()
            {
                "1 + 1 = 2",
                "2 + 2 = 4"
            });
            mockDataAccessService.Setup(x => x.SaveComputation("1 + 1 = 2"));
            var calculatorService = new CalculatorService(mockDataAccessService.Object);

            // Act
            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(2);

            calculatorService.InsertOperation(OperationType.Add);

            calculatorService.InsertDigit(3);
            calculatorService.InsertDigit(4);

            calculatorService.IsRoundedToInt = false;

            calculatorService.Compute();

            // Assert
            Assert.Equal(expected, calculatorService.CalculatorText);
        }

        [Fact]
        public void Compute_WhenDeduction_ReturnsValue()
        {
            // Arrange
            string expected = "5";
            var mockDataAccessService = new Mock<IDataAccess>();
            mockDataAccessService.Setup(x => x.GetComputationsHistory()).Returns(new List<string>()
            {
                "1 + 1 = 2",
                "2 + 2 = 4"
            });
            mockDataAccessService.Setup(x => x.SaveComputation("1 + 1 = 2"));
            var calculatorService = new CalculatorService(mockDataAccessService.Object);

            // Act
            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(0);

            calculatorService.InsertOperation(OperationType.Deduct);

            calculatorService.InsertDigit(5);


            calculatorService.IsRoundedToInt = false;

            calculatorService.Compute();

            // Assert
            Assert.Equal(expected, calculatorService.CalculatorText);
        }

        [Fact]
        public void Compute_WhenMultiplication_ReturnsValue()
        {
            // Arrange
            string expected = "100";
            var mockDataAccessService = new Mock<IDataAccess>();
            mockDataAccessService.Setup(x => x.GetComputationsHistory()).Returns(new List<string>()
            {
                "1 + 1 = 2",
                "2 + 2 = 4"
            });
            mockDataAccessService.Setup(x => x.SaveComputation("1 + 1 = 2"));
            var calculatorService = new CalculatorService(mockDataAccessService.Object);

            // Act
            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(0);

            calculatorService.InsertOperation(OperationType.Multiply);

            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(0);


            calculatorService.IsRoundedToInt = false;

            calculatorService.Compute();

            // Assert
            Assert.Equal(expected, calculatorService.CalculatorText);
        }

        [Fact]
        public void Compute_WhenDivision_ReturnsValue()
        {
            // Arrange
            string expected = "1";
            var mockDataAccessService = new Mock<IDataAccess>();
            mockDataAccessService.Setup(x => x.GetComputationsHistory()).Returns(new List<string>()
            {
                "1 + 1 = 2",
                "2 + 2 = 4"
            });
            mockDataAccessService.Setup(x => x.SaveComputation("1 + 1 = 2"));
            var calculatorService = new CalculatorService(mockDataAccessService.Object);

            // Act
            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(0);

            calculatorService.InsertOperation(OperationType.Divide);

            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(0);


            calculatorService.IsRoundedToInt = false;

            calculatorService.Compute();

            // Assert
            Assert.Equal(expected, calculatorService.CalculatorText);
        }

        [Fact]
        public void Compute_WhenDivisionDecimal_ReturnsValue()
        {
            // Arrange
            string expected = "2.5";
            var mockDataAccessService = new Mock<IDataAccess>();
            mockDataAccessService.Setup(x => x.GetComputationsHistory()).Returns(new List<string>()
            {
                "1 + 1 = 2",
                "2 + 2 = 4"
            });
            mockDataAccessService.Setup(x => x.SaveComputation("1 + 1 = 2"));
            var calculatorService = new CalculatorService(mockDataAccessService.Object);

            // Act
            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(0);

            calculatorService.InsertOperation(OperationType.Divide);

            calculatorService.InsertDigit(4);

            calculatorService.IsRoundedToInt = false;

            calculatorService.Compute();

            // Assert
            Assert.Equal(expected, calculatorService.CalculatorText);
        }

        [Fact]
        public void Compute_WhenDivisionRounded_ReturnsValue()
        {
            // Arrange
            string expected = "3";
            var mockDataAccessService = new Mock<IDataAccess>();
            mockDataAccessService.Setup(x => x.GetComputationsHistory()).Returns(new List<string>()
            {
                "1 + 1 = 2",
                "2 + 2 = 4"
            });
            mockDataAccessService.Setup(x => x.SaveComputation("1 + 1 = 2"));
            var calculatorService = new CalculatorService(mockDataAccessService.Object);

            // Act
            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(0);

            calculatorService.InsertOperation(OperationType.Divide);

            calculatorService.InsertDigit(4);

            calculatorService.IsRoundedToInt = true;

            calculatorService.Compute();

            // Assert
            Assert.Equal(expected, calculatorService.CalculatorText);
        }

        [Fact]
        public void Compute_WhenDivisionTwoDecimals_ReturnsValue()
        {
            // Arrange
            string expected = "4.2";
            var mockDataAccessService = new Mock<IDataAccess>();
            mockDataAccessService.Setup(x => x.GetComputationsHistory()).Returns(new List<string>()
            {
                "1 + 1 = 2",
                "2 + 2 = 4"
            });
            mockDataAccessService.Setup(x => x.SaveComputation("1 + 1 = 2"));
            var calculatorService = new CalculatorService(mockDataAccessService.Object);

            // Act
            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(0);
            calculatorService.InsertDecimalPoint();
            calculatorService.InsertDigit(5);


            calculatorService.InsertOperation(OperationType.Divide);

            calculatorService.InsertDigit(2);
            calculatorService.InsertDecimalPoint();
            calculatorService.InsertDigit(5);

            calculatorService.IsRoundedToInt = false;

            calculatorService.Compute();

            // Assert
            Assert.Equal(expected, calculatorService.CalculatorText);
        }

        [Fact]
        public void Compute_WhenDivisionTwoDecimalsRounded_ReturnsValue()
        {
            // Arrange
            string expected = "4";
            var mockDataAccessService = new Mock<IDataAccess>();
            mockDataAccessService.Setup(x => x.GetComputationsHistory()).Returns(new List<string>()
            {
                "1 + 1 = 2",
                "2 + 2 = 4"
            });
            mockDataAccessService.Setup(x => x.SaveComputation("1 + 1 = 2"));
            var calculatorService = new CalculatorService(mockDataAccessService.Object);

            // Act
            calculatorService.InsertDigit(1);
            calculatorService.InsertDigit(0);
            calculatorService.InsertDecimalPoint();
            calculatorService.InsertDigit(5);


            calculatorService.InsertOperation(OperationType.Divide);

            calculatorService.InsertDigit(2);
            calculatorService.InsertDecimalPoint();
            calculatorService.InsertDigit(5);

            calculatorService.IsRoundedToInt = true;

            calculatorService.Compute();

            // Assert
            Assert.Equal(expected, calculatorService.CalculatorText);
        }
    }
}