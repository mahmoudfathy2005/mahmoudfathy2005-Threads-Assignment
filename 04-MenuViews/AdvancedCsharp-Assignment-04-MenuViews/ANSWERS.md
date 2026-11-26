# Assignment 04 — Answers
 
Student name: Mahmoud Fathy
Group / ID: G3

## R1. Run the project and compare Expected and Actual.

When I ran the original project, I got the following result:

text
Expected: 400000
Actual:   205017


The expected result is 400,000 because there are 8 threads and each thread registers 50,000 views.

The actual result was lower than expected.

## R2. Explain why the numbers disagree.

The problem happens because Views++ is not an atomic operation.

It consists of three steps:

Reading the current value.
Increasing the value by one.
Writing the new value.

When multiple threads execute these steps at the same time, some updates can be lost.

This problem is called a Race Condition.

## R3. Fix RegisterView so the count stays correct. Choose the right tool for a single counter.

I used Interlocked.Increment() because it is suitable for safely incrementing a single shared counter.

Updated code:

csharp
using System.Threading;

namespace MenuViews;

public sealed class MenuViewCounter
{
    public int Views;

    public void RegisterView()
    {
        Interlocked.Increment(ref Views);
    }
}


Interlocked.Increment() makes the increment operation atomic, so multiple threads can update the counter safely without losing updates.

After fixing the code, the expected result should match the actual result:

text
Expected: 400000
Actual:   400000

R4. When would you protect this kind of update differently?

If I need to update multiple shared variables together or perform several operations that must be executed safely as one unit, I would use lock.

For a single counter, Interlocked.Increment() is simpler and more suitable.

If multiple application instances need to update shared data in a database, I would use database transactions or atomic database updates.
