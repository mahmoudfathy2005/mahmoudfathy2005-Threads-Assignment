# Phase 1 — Async Data Access Layer

## 1. Why are the sync and async times almost the same when orders are awaited one after another?

Because the orders are processed sequentially in both versions.

The async version uses `await`, but it still waits for each order to finish before starting the next one.

Async does not automatically make one operation faster. Its main advantage is that it does not block a thread while waiting for asynchronous operations to complete.

In this phase, both versions perform almost the same simulated database delays, so their execution times can be close. The async version also saves the validated orders, which adds extra work.

## 2. Is the thread ID before an await always the same as after it? Why?

No. The thread ID is not guaranteed to be the same before and after an await.

When an asynchronous operation is waiting, the current thread can return to the ThreadPool. When the operation completes, the continuation may run on another available thread.

This can be observed in the Log output by comparing thread IDs before and after awaited operations.

## 3. What does the thread do while Task.Delay() is waiting? And while Thread.Sleep() is waiting?

With Task.Delay(), the current thread is not blocked. It can return to the ThreadPool and execute other work.

With Thread.Sleep(), the current thread is blocked for the specified duration and cannot perform other work during that time.

## 4. Why should a repository method return Task<T> instead of T?

A repository method often performs I/O operations such as reading files or accessing a database.

Returning Task<T> allows the caller to await the operation without blocking the current thread while the I/O is pending.

It also makes it easier to compose asynchronous operations and scale applications that handle many requests.

## 5. What is the difference between File.ReadAllText and File.ReadAllTextAsync? Does the async version occupy a thread while the disk is reading?

File.ReadAllText is synchronous. It blocks the calling thread until the file has been read.

File.ReadAllTextAsync is asynchronous. It returns a Task that can be awaited, allowing the calling thread to do other work while the read operation is pending.

The async version does not require a dedicated thread to sit blocked for the entire disk I/O operation. The operating system and .NET runtime handle the asynchronous I/O completion.
