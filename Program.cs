using Microsoft.VisualBasic.FileIO;

if (!File.Exists("settings.csv")) UnitSet();

while (true)
{
    if (File.Exists("weight_data.csv") && File.Exists("weight_goal.csv"))
    {
        Console.WriteLine($"Your starting weight was: {Start():0.0} {Unit()}. Your current goal weight is: {Goal():0.0} {Unit()}\n");
        Console.WriteLine("What would you like to do? ");
        Console.WriteLine("  1 - Record Weight");
        Console.WriteLine("  2 - Report Past 10 Entries");
        Console.WriteLine("  3 - Parse Overall Change");
        Console.WriteLine("  8 - Set/Change your displayed units");
        Console.WriteLine("  9 - Set/Change your Goal Weight");
        Console.WriteLine("  0 - Exit");
        int reply = Convert.ToInt32(Console.ReadLine());

        if (reply == 1) Write();
        else if (reply == 2) Read();
        else if (reply == 3) Parse();
        else if (reply == 8) UnitSet();
        else if (reply == 9) GoalSet();
        else if (reply == 0) break;
        else Console.WriteLine("Bad entry, please try again.");
    }
    else { File.Create("weight_data.csv").Close(); File.Create("weight_goal.csv").Close(); }
}

double Start()
{
    using TextFieldParser parser = new("weight_data.csv");
    parser.TextFieldType = FieldType.Delimited;
    parser.SetDelimiters(",");

    string[]? entries = parser.ReadFields();
    if (entries?[1] == null)
        return 0;
    else return Convert.ToDouble(entries![1]);
}

double Goal()
{
    using StreamReader goal = new("weight_goal.csv");
    return Convert.ToInt32(goal.ReadLine());
}

void Parse() // Show over all weightloss up til the most recent entry, show weight to go until goal, average loss per week/month and estimated time til goal at current pace.
{
    Console.Clear();
    List<WeightData> weightData = new();

    using TextFieldParser parser = new("weight_data.csv");
    parser.TextFieldType = FieldType.Delimited;
    parser.SetDelimiters(",");

    int count = 0;
    string[]? entries = parser.ReadFields();
    for (int i = 1; i > 0 && i < entries!.Length; i += 2)
    {
        count++;
        WeightData historical = new(Convert.ToDouble(entries![i]), Convert.ToDateTime(entries![i - 1]));
        weightData.Add(historical);        
    }

    TimeSpan duration = weightData[^1].Date - weightData[0].Date;
    double weightChange = weightData[0].Weight - weightData[^1].Weight;
    double weightChangeDay = weightChange / duration.TotalDays;
    double weightToGoal = weightData[0].Weight - Goal();

    Console.WriteLine($"You have recorded your weight {count} times over {duration.TotalDays} days. You began your tracking on {weightData[0].Date:MM/dd/yyyy} at {weightData[0].Weight:0.0} {Unit()}.\r\n");
    if (weightChange > 0)
    {
        Console.WriteLine($"You have lost a total of {weightChange:0.0} {Unit()} over that period. This equates to an average {weightChangeDay * 7:0.0} {Unit()} per week, or {weightChangeDay:0.0} {Unit()} lost per day! :)");
        Console.WriteLine($"Your goal weight is {Goal():0.0} and you are currently {weightToGoal:0.0} {Unit()} away from reaching it!");
    }
    else
    {
        Console.WriteLine($"So far your weight has gone up by a total of {weightChange:0.0} {Unit()} over that period. This equates to an average {weightChangeDay * 7:0.0} {Unit()} per week, or {weightChangeDay:0.0} {Unit()} lost per day.");
    }

    Console.Write("\r\nPress any key to return to the main menu.");
    Console.ReadKey();
    Console.Clear();
}

void Read()
{
    Console.Clear();
    Console.WriteLine("These are your 10 most recently saved weight entries.\n");

    using TextFieldParser parser = new("weight_data.csv");
    parser.TextFieldType = FieldType.Delimited;
    parser.SetDelimiters(",");

    int count = 0;
    string[]? entries = parser.ReadFields();
    for (int i = entries!.Length-2; i > 0 && i > entries!.Length-21; i -= 2)
    {
        count++;
        WeightData historical = new(Convert.ToDouble(entries![i]), Convert.ToDateTime(entries![i - 1]));
        Console.WriteLine($"{count:00}: {historical.Date:MM/dd/yyyy} - {historical.Weight:0.0} {Unit()}"); 
    }

    Console.Write("\r\nPress any key to return to the main menu.");
    Console.ReadKey();
    Console.Clear();
}

void Write()
{
    Console.WriteLine("What is your current weight today?");
    double reply = Convert.ToDouble(Console.ReadLine());

    WeightData input = new(reply);

    Console.WriteLine($"Your weight is {input.Weight:0.0} {Unit()} and today's date is: {input.Date:MM/dd/yyyy}.\r\n");

    using StreamWriter writer = new("weight_data.csv", append: true);
    writer.Write($"{input.Date:MM/dd/yyyy},{input.Weight:0.0},");
}

void GoalSet()
{
    Console.Write("What would you like your goal weight to be set to? ");
    double goal = Convert.ToDouble(Console.ReadLine());

    using StreamWriter writer = new("weight_goal.csv");
    writer.WriteLine(goal);
}

string Unit() { return File.ReadAllText("settings.csv"); }

void UnitSet()
{
    Console.Clear();
    Console.WriteLine("What unit type would you like to use?");
    string? unit = Console.ReadLine();
    using StreamWriter writer = new("settings.csv");
    writer.Write(unit);
    Console.Clear();
}

class WeightData
{
    public DateTime Date { get; set; }
    public double Weight { get; set; }

    public WeightData(double input)
    {
        Weight = input;
        Date = DateTime.Today;
    }

    public WeightData(double input, DateTime date) 
    { 
        Weight = input;
        Date = date;
    }

    public WeightData(DateTime date, double weight)
    {
        Date = date;
        Weight = weight;
    }
}