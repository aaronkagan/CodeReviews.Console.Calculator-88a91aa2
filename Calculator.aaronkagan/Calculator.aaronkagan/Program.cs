using System.Text.RegularExpressions;
using CalculatorLibrary;

namespace CalculatorProgram
{

    class Program
    {
        static void Main()
        {
            bool endApp = false;
            Console.WriteLine("Console Calculator in C#\r");
            Console.WriteLine("------------------------\n");

            Calculator calculator = new Calculator();
            CalculationHistory history = new CalculationHistory();
            while (!endApp)
            {
                string? numInput1;
                string? numInput2;
                double result;

                Console.Write("Type a number, and then press Enter: ");
                numInput1 = Console.ReadLine();

                double cleanNum1;
                while (!double.TryParse(numInput1, out cleanNum1))
                {
                    Console.Write("This is not valid input. Please enter an integer value: ");
                    numInput1 = Console.ReadLine();
                }

                Console.Write("Type another number, and then press Enter: ");
                numInput2 = Console.ReadLine();

                double cleanNum2;
                while (!double.TryParse(numInput2, out cleanNum2))
                {
                    Console.Write("This is not valid input. Please enter an integer value: ");
                    numInput2 = Console.ReadLine();
                }

                Console.WriteLine("Choose an operator from the following list:");
                Console.WriteLine("\ta - Add");
                Console.WriteLine("\ts - Subtract");
                Console.WriteLine("\tm - Multiply");
                Console.WriteLine("\td - Divide");
                Console.Write("Your option? ");

                string? op = Console.ReadLine();

                if (op == null || ! Regex.IsMatch(op, "[a|s|m|d]"))
                {
                   Console.WriteLine("Error: Unrecognized input.");
                }
                else
                { 
                   try
                   {
                       result = calculator.DoOperation(cleanNum1, cleanNum2, op); 
                       if (double.IsNaN(result))
                       {
                           Console.WriteLine("This operation will result in a mathematical error.\n");
                       }
                       else
                       {
                           Console.WriteLine("Your result: {0:0.##}\n", result);
                           Calculation calculation = new Calculation(cleanNum1, cleanNum2, op, result);
                           history.Add(calculation);
                       }    
                           
                   }
                   catch (Exception e)
                   {
                       Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
                   }
                }
                Console.WriteLine("------------------------\n");

                Console.Write("Press 'n' to exit, 'h' to show the calculation history or press Enter to continue: ");

                var input = Console.ReadLine();
                Console.Clear();
                
                switch (input)
                {
                    case "n":
                        endApp = true;
                        break;
                    case "h":
                    {
                        history.Print();
                        Console.WriteLine("Press 'd' then Enter to delete the history or any other key and Enter to continue");
                        var answer = Console.ReadLine();
                        Console.Clear();
                        if (answer == "d")
                        {
                            history.Delete();
                            Console.WriteLine("History deleted");
                        }
                        break;
                    }
                    default:
                        continue;
                }

                Console.WriteLine("\n"); 
            }
            calculator.Finish();
        }
    }

    class Calculation
    {
        public readonly double _leftNumber;
        public readonly double _rightNumber;
        public readonly string _operation;
        public readonly double _result;

        public Calculation(double leftNumber, double rightNumber, string @operation, double result)
        {
            _leftNumber = leftNumber;
            _rightNumber = rightNumber;
            _operation = @operation;
            _result = result;
        }
    }

    class CalculationHistory
    {
        private List<Calculation> _history = [];
        public void Print()
        {
            if (_history.Count == 0)
            {
                Console.WriteLine("There is no history to show");
            }
            else
            {
                Console.WriteLine("HISTORY");
                Console.WriteLine("-----------------");

                foreach (var (index, calculation) in _history.Index())
                {
                    char operation = calculation._operation switch
                    {
                        "a" => '+',
                        "s" => '-',
                        "m" => '*',
                        "d" => '/',
                    };
                    Console.WriteLine($"{index + 1}) {calculation._leftNumber} {operation} {calculation._rightNumber} = {calculation._result}");
                }
                
                Console.WriteLine("-----------------");
                Console.WriteLine("The calculator has been used " + _history.Count + " " + (_history.Count == 1 ? "time" : "times"));
            }
            
        }
        public void Add(Calculation calculation)
        {
            _history.Add(calculation);
        }
        public void Delete()
        {
            _history = [];
        }
    }
}