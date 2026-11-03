## Assignment 03 — Answers

Student name: Mahmoud Fathy
Group / ID: G3

## 1. What is System.Threading.Monitor used for?

System.Threading.Monitor is used to control access to shared resources when multiple threads are running at the same time. It makes sure that only one thread can access a critical section at a time, which helps prevent race conditions.

## 2. What does the C# lock statement compile to?

The C# lock statement is translated into Monitor.Enter() and Monitor.Exit() inside a try-finally block. This makes sure that the lock is released even if an exception happens while executing the code.

## 3. List at least two differences between using lock and calling Monitor APIs directly.
lock is easier to use and makes the code shorter and cleaner.
When using lock, the compiler automatically handles releasing the lock. With Monitor, we have to use try-finally manually.
Monitor gives more control because it has extra methods like TryEnter(), Wait(), and Pulse().
## 4. When might a developer use Monitor.TryEnter, Monitor.Pulse, or Monitor.Wait instead of a plain lock?
Monitor.TryEnter() is useful when we want to try getting a lock without waiting forever. We can also set a timeout.
Monitor.Wait() is useful when a thread needs to wait for something to happen. It releases the lock temporarily so another thread can work.
Monitor.Pulse() is used to notify a waiting thread that something has changed and it can try to continue.

These methods are useful in situations like producer-consumer applications.

## 5. Write the same critical section twice: once with lock, once with Monitor.Enter / Monitor.Exit.

Both examples below do the same thing: they increase a shared counter safely without allowing multiple threads to modify it at the same time.

Using lock:

csharp
private static readonly object lockObject = new object();
private static int counter = 0;

public static void Increment()
{
    lock (lockObject)
    {
        counter++;
    }
}


Using Monitor.Enter / Monitor.Exit:

csharp
private static readonly object lockObject = new object();
private static int counter = 0;

public static void Increment()
{
    bool lockTaken = false;

    try
    {
        Monitor.Enter(lockObject, ref lockTaken);
        counter++;
    }
    finally
    {
        if (lockTaken)
        {
            Monitor.Exit(lockObject);
        }
    }
}


Both ways protect the critical section and make sure that only one thread can increase the counter at a time.

Links:
https://learn.microsoft.com/en-us/dotnet/api/system.threading.monitor
https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock