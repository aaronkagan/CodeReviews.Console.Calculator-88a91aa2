using System.Text.RegularExpressions;
using CalculatorLibrary;

namespace CalculatorProgram
{

    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Console Calculator in C#\r");
            Console.WriteLine("------------------------\n");

            Calculator calculator = new();
            CalculatorApp calculatorApp = new(calculator);
            calculatorApp.Start();
            calculator.Finish();
        }
    }

    class CalculatorApp
    {
        private readonly Calculator _calculator;

        public CalculatorApp(Calculator calculator)
        {
            _calculator = calculator;
        }
        

        public void Start()
        {
            CalculationHistory history = new();
            InputHandler inputHandler = new();
            bool endApp = false;
            while (!endApp)
            {
                
                var calculationRequest =  ShowMainMenu(inputHandler);
                RunCalculation(calculationRequest.LeftNumber, calculationRequest.RightNumber, calculationRequest.Op, history);
                Console.WriteLine("------------------------\n");
                Console.Write("Press 'n' to exit, 'h' to show the calculation history or press Enter to continue: ");
                
                
                var input = Console.ReadLine();
                Console.Clear();

                if (HandleUserChoice(input, endApp, history))
                {
                    endApp = true;
                }
                
                Console.WriteLine("\n");
            }
        }

        private bool HandleUserChoice(string? input, bool endApp, CalculationHistory history)
        {
            switch (input)
            {
                case "n":
                    endApp = true;
                    break;
                case "h":
                {
                    history.Print();
                    Console.WriteLine(history.Count() == 0
                        ? "Press Enter to continue"
                        : "Press 'd' then Enter to delete the history or Enter to continue");
                    var answer = Console.ReadLine();
                    Console.Clear();
                    if (answer == "d")
                    {
                        history.Delete();
                    }

                    break;
                }
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }

            return endApp;
        }

        private CalculationRequest ShowMainMenu(InputHandler inputHandler)
        {
            Console.Write("Type a number, and then press Enter: ");
            double cleanNum1 = inputHandler.GetValidNumber();
            Console.Write("Type another number, and then press Enter: ");
            double cleanNum2 = inputHandler.GetValidNumber();
            Console.WriteLine("Choose an operator from the following list:");
            Console.WriteLine("\ta - Add");
            Console.WriteLine("\ts - Subtract");
            Console.WriteLine("\tm - Multiply");
            Console.WriteLine("\td - Divide");
            Console.Write("Your option? ");
            string op = inputHandler.GetValidOperation();

            CalculationRequest calculationRequest = new(cleanNum1, cleanNum2, op);

            return calculationRequest;
        }
        
        private void RunCalculation(double cleanNum1, double cleanNum2, string op, CalculationHistory history)
        {
            try
            {
                double result = _calculator.DoOperation(cleanNum1, cleanNum2, op);
                if (double.IsNaN(result))
                {
                    Console.WriteLine("This operation will result in a mathematical error.\n");
                }
                else
                {
                    Console.WriteLine("Your result: {0:0.##}\n", result);
                    Calculation calculation = new(cleanNum1, cleanNum2, op, result);
                    history.Add(calculation);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
            }
            
        }
    }

    class CalculationRequest (double cleanNumber1, double cleanNumber2, string op)
    {
        public double LeftNumber { get; } = cleanNumber1;
        public double RightNumber { get; } = cleanNumber2;
        public string Op { get; } = op;

    }
    
    class InputHandler
    {
        public double GetValidNumber()
        {
            string? numInput = Console.ReadLine();

            double cleanNum;
            while (!double.TryParse(numInput, out cleanNum))
            {
                Console.Write("This is not valid input. Please enter an integer value: ");
                numInput = Console.ReadLine();
            }

            return cleanNum;
        }

        public string GetValidOperation()
        {
            string? op;
            do
            {
                op = Console.ReadLine();
                if (op == null || !Regex.IsMatch(op, "[a|s|m|d]"))
                {
                    Console.WriteLine("Invalid Operation. Please try again.");
                }
            } while (op == null || !Regex.IsMatch(op, "[a|s|m|d]"));

            return op;
        }
    }

    class Calculation(double leftNumber, double rightNumber, string @operation, double result)
    {
        public readonly double LeftNumber = leftNumber;
        public readonly double RightNumber = rightNumber;
        public readonly string Operation = @operation;
        public readonly double Result = result;
    }

    class CalculationHistory
    {
        private readonly List<Calculation> _history = [];

        public void Print()
        {
            if (_history.Count == 0)
            {
                Console.WriteLine("There is no history to show");
                Console.WriteLine("Press Enter to continue");
            }
            else
            {
                Console.WriteLine("HISTORY");
                Console.WriteLine("-----------------");
                foreach (var (index, calculation) in _history.Index())
                {
                    char operation = calculation.Operation switch
                    {
                        "a" => '+',
                        "s" => '-',
                        "m" => '*',
                        "d" => '/',
                        _ => throw new InvalidOperationException()
                    };
                    Console.WriteLine(
                        $"{index + 1}) {calculation.LeftNumber} {operation} {calculation.RightNumber} = {calculation.Result}");
                }

                Console.WriteLine("-----------------");
                Console.WriteLine("The calculator has been used " + _history.Count + " " +
                                  (_history.Count == 1 ? "time" : "times"));
            }

        }

        public void Add(Calculation calculation)
        {
            _history.Add(calculation);
        }

        public void Delete()
        {
            _history.Clear();
            Console.WriteLine("History deleted");
        }

        public int Count()
        {
            return _history.Count;
        }
    }
}
  
