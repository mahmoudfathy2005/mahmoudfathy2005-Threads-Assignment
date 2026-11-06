namespace MenuViews;

public sealed class MenuViewCounter
{
    public int Views;
    private readonly Lock _lock = new Lock();

    public void RegisterView()
    {
        Interlocked.Increment(ref Views);
        //lock (_lock)
        //{
        //    Views++;
        //}

    }
}
