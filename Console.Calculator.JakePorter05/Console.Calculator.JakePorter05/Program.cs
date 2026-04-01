IConfiguration config = new ConfigurationBuilder()
    .AddUserSecrets<Program>() 
    .Build();

bool endApp = false;

// Display title as the C# console calculator app.
Console.WriteLine("Console Calculator in C#\r");
Console.WriteLine("------------------------\n");

CalculatorService calculator = new CalculatorService();
InputHelper inputHelper = new InputHelper(calculator, config);

while (!endApp)
{
    //Ask if the user would like to use a previous result for the first number
    Console.WriteLine("C : Make Calculation");
    Console.WriteLine("E : Enable Speech if you have a microphone enabled.");
    Console.WriteLine("M : Use a previous result as first number.");
    Console.WriteLine("H : See previous Calculations");
    var selection = Console.ReadLine();

    if (selection == null || !Regex.IsMatch(selection, "[c|e|m|h]"))
    {
        Console.WriteLine("Error: Unrecognized input.");
    }
    else
    {
        switch (selection)
        {
            case "e":
                {
                    inputHelper.EnableSpeech();
                }
                break;
            case "m":
                {
                    inputHelper.ShowHistory();
                    inputHelper.GetHistoryInput();
                    inputHelper.GetOperatorInput();
                    if(inputHelper.SkipSecondNumber())
                    {
                        await inputHelper.GetSecondInput();
                    }
                    inputHelper.CalculateAndShowResult();
                }
                break;
            case "h":
                {
                    inputHelper.ShowHistory();
                    Console.WriteLine("Press c and Enter if you would like to clear the history.");
                    Console.WriteLine("Note: Clearing the history will also clear the current result.");
                    Console.WriteLine("Press any other key and Enter to return to the main menu.");
                    Console.ReadLine();
                    if (Console.ReadLine() == "c")
                    {
                        calculator.ClearCalculations();
                        calculator.ClearValues();
                        Console.WriteLine("History cleared.");
                        Console.WriteLine("Press any other key and Enter to return to the main menu.");
                        Console.ReadKey();
                    }
                }
                break;
            default:
                {
                    await inputHelper.GetFirstInput();
                    inputHelper.GetOperatorInput();
                    if (inputHelper.SkipSecondNumber())
                    {
                        await inputHelper.GetSecondInput();
                    }
                    inputHelper.CalculateAndShowResult();
                }
                break;
        }
    }
    Console.WriteLine("------------------------\n");
    calculator.ClearValues();

    // Wait for the user to respond before closing.
    Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");
    if (Console.ReadLine() == "n") endApp = true;

    Console.WriteLine("\n"); // Friendly linespacing.
}

calculator.EndArray();
Console.WriteLine("Total calculations performed: " + calculator.Calculations.Count);
calculator.AddCount();
calculator.Finish();