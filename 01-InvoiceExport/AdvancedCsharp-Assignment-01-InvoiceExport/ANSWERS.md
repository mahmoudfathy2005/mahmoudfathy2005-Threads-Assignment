# Assignment 01 — Answers

Student name:  Mahmoud Fathy 
Group / ID: 3


R1 - Run the Project
I ran the project before making any changes.

The application generated 5 invoice files successfully, but it took 5.9 seconds.

# Output Before Changes

text
Starting invoice export...
Wrote invoice-1001.txt
Wrote invoice-1002.txt
Wrote invoice-1003.txt
Wrote invoice-1004.txt
Wrote invoice-1005.txt
Export finished in 5.9 s

# R2 - Problems I Found
The code uses Thread.Sleep, which blocks the thread.
File reading and writing are synchronous.
Orders are processed one by one instead of at the same time.
The application becomes slower when there are many orders.

#R3 - Changes I Made
Replaced Thread.Sleep with Task.Delay.
Used async and await for database operations.
Used File.ReadAllTextAsync to read customer files.
Used File.WriteAllTextAsync to write invoice files.
Used Parallel.ForEachAsync to process multiple orders at the same time.
Added CancellationToken to support cancelling the export.
I kept the same business flow:

Load Order → Read Customer → Write Invoice.

I also kept the same invoice content.

# R4 - Results After Changes
I ran the project again after making the changes.

All 5 invoice files were generated successfully.

#Output After Changes

text
Starting invoice export...
Wrote invoice-1001.txt
Wrote invoice-1004.txt
Wrote invoice-1002.txt
Wrote invoice-1003.txt
Wrote invoice-1005.txt
Export finished in 1.8 s

# Before and After

Before	                        After
5.9 seconds	                    1.8 seconds
Sequential processing	        Parallel processing
Synchronous operations	        Asynchronous operations

The application is now faster, and all 5 invoices are still generated with the same content.
