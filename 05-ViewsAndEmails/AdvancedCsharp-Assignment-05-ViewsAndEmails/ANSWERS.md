# Assignment 05 — Answers

Student name: Mahmoud Fathy  
Group / ID: G3

## Failures you observed

- When I ran the original program, I got an `ArgumentException` because the same product key (`koshary-bowl`) was added to the Dictionary more than once.
- This happened because multiple threads were accessing the Dictionary at the same time.
- I also noticed that the email Queue was not thread-safe because multiple producers and workers were accessing it simultaneously.
- The view count could also be incorrect because two threads might read and update the value at the same time, causing lost updates.

## What you changed

- I added a `lock` object to protect the Views Dictionary.
- I placed the check, add, and increment operations inside the same lock to make sure that only one thread updates the view count at a time.
- I added another lock to protect the EmailJobs Queue.
- I used the same lock in both `PlaceOrder` and `EmailWorker` to make enqueue and dequeue operations safe.
- I also kept a separate lock for the `SentEmails` list to prevent multiple workers from modifying it at the same time.
- I made sure that all producer threads finish before setting `ProducersDone` to true.
- After the changes, the expected results are 50,000 views and 20 emails sent without exceptions.

## Collection safety vs business-rule safety

Making a collection thread-safe does not automatically make the whole business logic thread-safe.

For example, checking if a key exists and then adding it are two separate operations. Another thread could change the Dictionary between these operations.

The same problem can happen when checking the Queue count and then removing an item using `Dequeue()`.

Even if individual collection operations are thread-safe, multiple operations together may still cause race conditions.

That is why I used locks around the complete operations, not just individual statements, to make sure the program gives the correct results when multiple threads run at the same time.