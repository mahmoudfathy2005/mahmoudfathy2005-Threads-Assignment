# Table Booking Assignment
R1 - Run the Project

I ran the original project several times.

The final results were not always correct because many customers tried to book at the same time.

R2 - Problems I Found
Multiple threads can access TablesLeft at the same time.
More than one customer can pass the if condition before the tables are updated.
This problem is called a Race Condition.
The application may book more tables than available.
R3 - Changes I Made

I used lock to protect the booking code.

I created a private lock object:

csharp
private readonly object _lock = new();


Then I placed the booking operations inside:

csharp
lock (_lock)
{
    if (TablesLeft <= 0)
        return;

    TablesLeft--;
    Booked++;
}


This allows only one thread at a time to check and update the booking numbers.

R4 - Results After Changes

I ran the project 5 times after fixing the code.

All runs gave the correct results.

Run	Booked	TablesLeft
1	5	0
2	5	0
3	5	0
4	5	0
5	5	0
Final Result

The application now books only 5 tables, even when 40 customers try to book at the same time.

The lock statement prevents multiple threads from changing the booking numbers at the same time.

The booking problem is fixed.