namespace TableBooking;

public sealed class FridayNightBooking
{
    private readonly Lock _lock = new();
    public int TablesLeft { get; private set; } = 5;
    public int Booked { get; private set; }

    public void Book(string customerName)
    {
        Thread.Sleep(20);
        lock (_lock)
        {
            if (TablesLeft <= 0)
                return;

           

            TablesLeft--;
            Booked++;
            Console.WriteLine($"{customerName} booked a table. Left={TablesLeft}");
        }
    }
}
