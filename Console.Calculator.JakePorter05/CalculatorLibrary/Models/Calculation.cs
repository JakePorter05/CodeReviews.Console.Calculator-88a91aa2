namespace CalculatorLibrary.Models;

public class Calculation
{
    public int Id { get; set; }
    public double Operand1 { get; set; } = double.NaN;
    public double Operand2 { get; set; } = double.NaN;
    public string Operator { get; set; } = "";
    public double Result { get; set; } = double.NaN;
}
