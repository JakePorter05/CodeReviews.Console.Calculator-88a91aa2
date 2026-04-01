namespace CalculatorLibrary.Helpers;

public class InputHelper
{
    public CalculatorService Calculator { get; set; }
    public SpeechRecognizer? Recognizer { get; set; }
    internal bool speechEnabled { get; set; } = false;

    public InputHelper(CalculatorService calculator, IConfiguration config)
    {
        Calculator = calculator;
        
        var speechConfig = SpeechConfig.FromSubscription(config["SpeechService:Key"], config["SpeechService:Region"]);
        Recognizer = new SpeechRecognizer(speechConfig);
    }

    public void ShowHistory()
    {
        //Has Grids and Panels, but I am just using the table for now
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("[bold yellow]Operand 1[/]");
        table.AddColumn("[bold yellow]Operator[/]");
        table.AddColumn("[bold yellow]Operand 2[/]");
        table.AddColumn("[bold yellow]Result[/]");

        foreach (var calculation in Calculator.Calculations)
        {
            var num2 = double.IsNaN(calculation.Operand2) ? "" : calculation.Operand2.ToString();
            table.AddRow($"[blue]{calculation.Id}[/]", $"[yellow]{calculation.Operand1}[/]", $"[red]{ShowOperator(calculation.Operator)}[/]", $"[yellow]{num2}[/]", $"[blue]{calculation.Result}[/]");
        }
        AnsiConsole.Write(table);
    }

    string ShowOperator(string op)
    {
        switch (op)
        {
            case "a":
                return "+";
            case "s":
                return "-";
            case "m":
                return "*";
            case "r":
                return "Square Root";
            case "p":
                return "^";
            case "w":
                return "10x";
            case "t":
                return "Sin";
            default:
                return "/";
        }
    }

    public void CalculateAndShowResult()
    {
        try
        {
            var result = Calculator.DoOperation();
            if (double.IsNaN(result))
            {
                Console.WriteLine("This operation will result in a mathematical error.\n");
            }
            else
            {
                Console.WriteLine("Your result: {0:0.##}\n", result);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
        }
    }

    public void GetHistoryInput()
    {
        //Get the first number from the history of calculations.
        Console.Write("Enter the ID of the result you want to use: ");
        string? selectedId = Console.ReadLine();
        if (int.TryParse(selectedId, out int id) && id >= Calculator.Calculations.Min(x => x.Id) && id <= Calculator.Calculations.Max(x => x.Id))
        {
            var selectedCalculation = Calculator.Calculations.FirstOrDefault(c => c.Id == id);
            if (selectedCalculation != null)
            {
                Calculator.Calculation.Operand1 = selectedCalculation.Result;
                Console.WriteLine("Using previous result: " + Calculator.Calculation.Operand1);
            }
        }
        else
            Console.WriteLine("Invalid ID. Proceeding without using a previous result.");
    }

    public async Task GetFirstInput()
    {
        // Ask the user to type the first number.
        string? numInput1 = null;
        if (speechEnabled)
        {
            Console.WriteLine("You can say the first number if you have a microphone enabled.");
            if (Recognizer != null)
            {
                var result = await Recognizer.RecognizeOnceAsync();
                if(result.Reason == ResultReason.RecognizedSpeech)
                {
                    numInput1 = result.Text;
                    Console.WriteLine("You said: " + numInput1);
                }
                else
                {
                    Console.WriteLine("Speech not recognized. Please type the first number.");
                    Console.Write("Type a number, and then press Enter: ");
                    numInput1 = Console.ReadLine();
                }
            }
        }
        else
        {
            Console.Write("Type a number, and then press Enter: ");
            numInput1 = Console.ReadLine();
        }

        ValidateInput1(numInput1);
    }

    public bool SkipSecondNumber()
    {
        if (Calculator.Calculation.Operator != "r" &&
           Calculator.Calculation.Operator != "w" &&
           Calculator.Calculation.Operator != "t")
            return true;
        return false;
    }

    public void GetOperatorInput()
    {
        // Ask the user to choose an operator.
        Console.WriteLine("Choose an operator from the following list:");
        Console.WriteLine("\ta - Add");
        Console.WriteLine("\ts - Subtract");
        Console.WriteLine("\tm - Multiply");
        Console.WriteLine("\td - Divide");
        Console.WriteLine("\tr - Square Root");
        Console.WriteLine("\tp - Power");
        Console.WriteLine("\tw - 10x");
        Console.WriteLine("\tt - Sin(x)");
        Console.Write("Your option? ");

        string? operand = Console.ReadLine();

        // Validate input is not null, and matches the pattern
        while (string.IsNullOrEmpty(operand) || !Regex.IsMatch(operand, "[a|s|m|d|r|p|w|t]"))
        {
            Console.WriteLine("Error: Unrecognized input.");
            Console.Write("Your option? ");
            operand = Console.ReadLine();
        }

        Calculator.Calculation.Operator = operand;
    }

    public async Task GetSecondInput()
    {
        // Ask the user to type the second number.
        string? numInput2 = null;
        if (speechEnabled)
        {
            Console.WriteLine("You can say the second number if you have a microphone enabled.");
            if (Recognizer != null)
            {
                var result = await Recognizer.RecognizeOnceAsync();
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    numInput2 = result.Text;
                    Console.WriteLine("You said: " + numInput2);
                }
                else
                {
                    Console.WriteLine("Speech not recognized. Please type the second number.");
                    Console.Write("Type a number, and then press Enter: ");
                    numInput2 = Console.ReadLine();
                }
            }
        }
        else
        {
            Console.Write("Type a number, and then press Enter: ");
            numInput2 = Console.ReadLine();
        }

        ValidateInput2(numInput2);
    }

    void ValidateInput1(string? input)
    {
        while (string.IsNullOrEmpty(input) || !double.TryParse(input, out double output))
        {
            Console.Write("This is not valid input. Please enter an integer value: ");
            input = Console.ReadLine();
        }

        Calculator.Calculation.Operand1 = double.Parse(input);
    }

    void ValidateInput2(string? input)
    {
        while (string.IsNullOrEmpty(input) || !double.TryParse(input, out double output))
        {
            Console.Write("This is not valid input. Please enter an integer value: ");
            input = Console.ReadLine();
        }

        Calculator.Calculation.Operand2 = double.Parse(input);
    }

    public void EnableSpeech()
    {
        speechEnabled = true;
    }
}