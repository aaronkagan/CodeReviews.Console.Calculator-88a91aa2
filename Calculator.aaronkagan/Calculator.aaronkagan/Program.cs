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
                
                var calculationRequest =  ShowMainMenu(inputHandler, history);
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
            }

            return endApp;
        }

        private CalculationRequest ShowMainMenu(InputHandler inputHandler, CalculationHistory history)
        {
                double cleanNum1 = inputHandler.GetValidNumber(history);
                double cleanNum2 = inputHandler.GetValidNumber(history);
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
        public double GetValidNumber(CalculationHistory history)
        {
            bool getFromHistory = false;
            if (history.Count() > 0)
            {
                Console.WriteLine("Would you like to get the number from results history?  y for yes or any other key for no: ");
                Console.WriteLine();
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    getFromHistory = true;
                    Console.WriteLine();
                }
            }
            
            
            if (getFromHistory)
            {
                double historyResult = GetValidHistoryResult(history);
                return historyResult;
            }
            else
            {
                Console.WriteLine("Please enter a number: ");
                string? numInput = Console.ReadLine();

                double cleanNum;
                while (!double.TryParse(numInput, out cleanNum))
                {
                    Console.Write("This is not valid input. Please enter an integer value: ");
                    numInput = Console.ReadLine();
                }
            
                return cleanNum;
            }
        }
        private double GetValidHistoryResult(CalculationHistory history)
        {
            history.Print();
            Console.WriteLine("Which result from history would you like to use? Enter the line number then press enter: ");
            string? input = Console.ReadLine();
            int cleanNum;
            
            while (!int.TryParse(input, out cleanNum) ||  cleanNum > history.Count() || cleanNum < 0)
            {
                Console.Write("This is not valid input. Please enter a line number that exists: ");
                input = Console.ReadLine();

                int.TryParse(input, out cleanNum);
            }

            var index = cleanNum - 1;

            return history.GetResult(index);
        }

        public string GetValidOperation()
        {
            Console.WriteLine("Choose an operator from the following list:");
            Console.WriteLine("\ta - Add");
            Console.WriteLine("\ts - Subtract");
            Console.WriteLine("\tm - Multiply");
            Console.WriteLine("\td - Divide");
            Console.Write("Your option? ");
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
        private readonly List<Calculation> History = [];

        public void Print()
        {
            if (History.Count == 0)
            {
                Console.WriteLine("There is no history to show");
                Console.WriteLine("Press Enter to continue");
            }
            else
            {
                Console.WriteLine("HISTORY");
                Console.WriteLine("-----------------");
                foreach (var (index, calculation) in History.Index())
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
                Console.WriteLine("The calculator has been used " + History.Count + " " +
                                  (History.Count == 1 ? "time" : "times"));
            }

        }

        public void Add(Calculation calculation)
        {
            History.Add(calculation);
        }

        public void Delete()
        {
            History.Clear();
            Console.WriteLine("History deleted");
        }

        public int Count()
        {
            return History.Count;
        }

        public double GetResult(int index)
        {
            return History[index].Result;
        }
        
    }
}
  
