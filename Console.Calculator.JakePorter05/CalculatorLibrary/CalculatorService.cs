namespace CalculatorLibrary;

public class CalculatorService
{
    JsonWriter writer;
    public Calculation Calculation { get; set; } = new();
    public List<Calculation> Calculations { get; set; } = [];

    public CalculatorService()
    {
        StreamWriter logFile = File.CreateText("calculatorlog.json");
        logFile.AutoFlush = true;
        writer = new JsonTextWriter(logFile);
        writer.Formatting = Formatting.Indented;
        writer.WriteStartObject();
        writer.WritePropertyName("Operations");
        writer.WriteStartArray();
    }

    public void ClearCalculations()
    {
        Calculations.Clear();
    }

    public void ClearValues()
    {
        Calculation = new();
    }   

    public double DoOperation()
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Operand1");
        writer.WriteValue(Calculation.Operand1);
        if (!double.IsNaN(Calculation.Operand2))
        {
            writer.WritePropertyName("Operand2");
            writer.WriteValue(Calculation.Operand2);
        }
        writer.WritePropertyName("Operation");

        // Use a switch statement to do the math.
        switch (Calculation.Operator)
        {
            case "a":
                Calculation.Result = Calculation.Operand1 + Calculation.Operand2;
                writer.WriteValue("Add");
                break;
            case "s":
                Calculation.Result = Calculation.Operand1 - Calculation.Operand2;
                writer.WriteValue("Subtract");
                break;
            case "m":
                Calculation.Result = Calculation.Operand1 * Calculation.Operand2;
                writer.WriteValue("Multiply");
                break;
            case "r":
                Calculation.Result = Math.Sqrt(Calculation.Operand1);
                writer.WriteValue("Square Root");
                break;
            case "p": 
                Calculation.Result = Math.Pow(Calculation.Operand1, Calculation.Operand2);
                writer.WriteValue("Power");
                break;
            case "w":
                Calculation.Result = Calculation.Operand1 * 10;
                writer.WriteValue("10x");
                break;
            case "t":
                Calculation.Result = Math.Sin(Calculation.Operand1);
                writer.WriteValue("Sin(x)");
                break;
            case "d":
                // Ask the user to enter a non-zero divisor.
                if (Calculation.Operand2 != 0)
                {
                    Calculation.Result = Calculation.Operand1 / Calculation.Operand2;
                }
                writer.WriteValue("Divide");
                break;
            // Return text for an incorrect option entry.
            default:
                break;
        }
        writer.WritePropertyName("Result");
        writer.WriteValue(Calculation.Result);
        writer.WriteEndObject();

        Calculation.Id = Calculations.Count + 1;
        Calculations.Add(Calculation);

        return Calculation.Result;
    }

    public void EndArray()
    {
        writer.WriteEndArray();
    }

    public void AddCount()
    {
        writer.WritePropertyName("CalculationCount");
        writer.WriteValue(Calculations.Count);
    }

    public void Finish()
    {
        writer.WriteEndObject();
        writer.Close();
    }
}