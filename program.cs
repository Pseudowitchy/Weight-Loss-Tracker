using Microsoft.VisualBasic.FileIO;

while (true)
{
    if (File.Exists("weight_data.csv") && File.Exists("weight_goal.csv"))
    {
        Console.WriteLine($"Your starting weight was: {start():0.0} lbs. Your current goal weight is: {goal():0.0} lbs\n");
        Console.WriteLine("What would you like to do? ");
        Console.WriteLine("  1 - Record Weight");
        Console.WriteLine("  2 - Report Past 10 Entries");
        Console.WriteLine("  3 - Parse Overall Change");
        Console.WriteLine("  9 - Set/Change your Goal Weight");
        Console.WriteLine("  0 - Exit");
        int reply = Convert.ToInt32(Console.ReadLine());

        if (reply == 1) write();
        else if (reply == 2) read();
        else if (reply == 3) parse();
        else if (reply == 9) goalSet();
        else if (reply == 0) break;
        else Console.WriteLine("Bad entry, please try again.");
    }
    else { File.Create("weight_data.csv").Close(); File.Create("weight_goal.csv").Close(); }
}

double start()
{
    using TextFieldParser parser = new("weight_data.csv");
    parser.TextFieldType = FieldType.Delimited;
    parser.SetDelimiters(",");

    string[]? entries = parser.ReadFields();
    if (entries?[1] == null)
        return 0;
    else return Convert.ToDouble(entries![1]);
}

double goal()
{
    using StreamReader goal = new("weight_goal.csv");
    return Convert.ToInt32(goal.ReadLine());
}

void parse() // Show over all weightloss up til the most recent entry, show weight to go until goal, average loss per week/month and estimated time til goal at current pace.
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
    double weightToGoal = weightData[0].Weight - goal();

    Console.WriteLine($"You have recorded your weight {count} times over {duration.TotalDays} days. You began your tracking on {weightData[0].Date:MM/dd/yyyy} at {weightData[0].Weight:0.0} lbs.\r\n");
    if (weightChange > 0)
    {
        Console.WriteLine($"You have lost a total of {weightChange:0.0} lbs over that period. This equates to an average {weightChangeDay * 7:0.0} lbs per week, or {weightChangeDay:0.0} lbs lost per day! :)");
        Console.WriteLine($"Your goal weight is {goal():0.0} and you are currently {weightToGoal:0.0} lbs away from reaching it!");
    }
    else
    {
        Console.WriteLine($"So far your weight has gone up by a total of {weightChange:0.0} lbs over that period. This equates to an average {weightChangeDay * 7:0.0} lbs per week, or {weightChangeDay:0.0} lbs lost per day.");
    }

    Console.Write("\r\nPress any key to return to the main menu.");
    Console.ReadKey();
    Console.Clear();
}

void read()
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
        Console.WriteLine($"{count:00}: {historical.Date:MM/dd/yyyy} - {historical.Weight:0.0} lbs"); 
    }

    Console.Write("\r\nPress any key to return to the main menu.");
    Console.ReadKey();
    Console.Clear();
}

void write()
{
    Console.WriteLine("What is your current weight today?");
    double reply = Convert.ToDouble(Console.ReadLine());

    WeightData input = new(reply);

    Console.WriteLine($"Your weight is {input.Weight:0.0} lbs and today's date is: {input.Date:MM/dd/yyyy}.\r\n");

    using StreamWriter writer = new("weight_data.csv", append: true);
    writer.Write($"{input.Date:MM/dd/yyyy},{input.Weight:0.0},");
}

void goalSet()
{
    Console.Write("What would you like your goal weight to be set to? ");
    double goal = Convert.ToInt32(Console.ReadLine());

    using StreamWriter writer = new("weight_goal.csv");
    writer.WriteLine(goal);
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
