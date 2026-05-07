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
                           history.Add(cleanNum1, cleanNum2, op);
                       }    
                           
                   }
                   catch (Exception e)
                   {
                       Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
                   }
                }
                Console.WriteLine("------------------------\n");

                Console.Write("Press 'n', 'h' to show the calculation history or press Enter to continue: ");

                var input = Console.ReadLine();
                
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

    class CalculationHistory
    {
        private List<string> _history = [];
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
                foreach (var calculation in _history)
                {
                    Console.WriteLine(calculation);
                }
                Console.WriteLine("-----------------");
            }
            
        }
        public void Add(double left, double right, string operation)
        {
            string calculationText = "";
            Calculator calculator = new Calculator();

            switch (operation)
            {
                case "a" :
                    calculationText = $"{left} + {right} = {calculator.DoOperation(left, right, operation)}";
                    break;
                case "s" :
                    calculationText = $"{left} - {right} = {calculator.DoOperation(left, right, operation)}";
                    break;
                case "m" :
                    calculationText = $"{left} * {right} = {calculator.DoOperation(left, right, operation)}";
                    break;
                case "d" :
                    calculationText = $"{left} / {right} = {calculator.DoOperation(left, right, operation)}";
                    break;
            }
            _history.Add(calculationText);
        }
        public void Delete()
        {
            _history = [];
        }
    }
}