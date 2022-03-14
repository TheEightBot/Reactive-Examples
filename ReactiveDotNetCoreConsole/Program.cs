using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Spectre.Console;

namespace ReactiveDotNetCoreConsole
{
    class Program
    {
        private readonly static object matlock = new object();

        static async Task Main (string[] args)
        {
            Console.WriteLine("Select an Example");
            Console.WriteLine("1a: Publishing With Refcount - Bad");
            Console.WriteLine("1b: Publishing With Refcount - Good");
            Console.WriteLine("1c: Publishing With Refcount - Good");
            Console.WriteLine("2a: Async with Ordering - Bad");
            Console.WriteLine("2b: Async with Ordering - Good");
            Console.WriteLine("3a: Limit Concurrency - Bad");
            Console.WriteLine("3b: Limit Concurrency - Good");
            Console.WriteLine("4a: Scheduler - Bad");
            Console.WriteLine("4b: Scheduler - Good");

            var selection = Console.ReadLine();

            switch (selection)
            {
                case "1a":
                    await PublishAndRefCountBadExample();
                    break;
                case "1b":
                    await PublishAndRefCountGoodExample();
                    break;
                case "1c":
                    await PublishAndRefCountAlternativeGoodExample();
                    break;
                case "2a":
                    await AsyncOrderingBadExample();
                    break;
                case "2b":
                    await AsyncOrderingGoodExample();
                    break;
                case "3a":
                    await LimitConcurrencyBadExample();
                    break;
                case "3b":
                    await LimitConcurrencyGoodExample();
                    break;
                case "4a":
                    await SchedulerExampleBad();
                    break;
                case "4b":
                    await SchedulerExampleGood();
                    break;
                default:
                    break;
            }


            Console.WriteLine();
            Console.WriteLine("Example Finished. Press Any key to continue...");

            Console.ReadLine();
        }

        public static async Task PublishAndRefCountBadExample ()
        {
            var intervalObs = 
                Observable
                    .Interval(TimeSpan.FromSeconds(2))
                    .Do(i => WriteToConsoleWithColor((ConsoleColor)i, $"Processing Interval {i}"));

            var firstListener =
                intervalObs
                    .Do(val => WriteToConsoleWithColor((ConsoleColor)val, $"First Listener Received: {val}"))
                    .Subscribe();

            var secondListener =
                intervalObs
                    .Do(val => WriteToConsoleWithColor((ConsoleColor)val, $"Second Listener Received: {val}"))
                    .Subscribe();

            await Task.Delay(TimeSpan.FromSeconds(5));

            firstListener.Dispose();
            secondListener.Dispose();
        }

        public static async Task PublishAndRefCountGoodExample ()
        {
            var intervalObs =
                Observable
                    .Interval(TimeSpan.FromSeconds(2))
                    .Do(i => WriteToConsoleWithColor((ConsoleColor)i, $"Processing Interval {i}"))
                    .Publish()
                    .RefCount();

            var firstListener =
                intervalObs
                    .Do(val => WriteToConsoleWithColor((ConsoleColor)val, $"First Listener Received: {val}"))
                    .Subscribe();

            var secondListener =
                intervalObs
                    .Do(val => WriteToConsoleWithColor((ConsoleColor)val, $"Second Listener Received: {val}"))
                    .Subscribe();

            await Task.Delay(TimeSpan.FromSeconds(5));

            firstListener.Dispose();
            secondListener.Dispose();
        }

        public static async Task PublishAndRefCountAlternativeGoodExample ()
        {
            var intervalObs =
                Observable
                    .Interval(TimeSpan.FromSeconds(2))
                    .Do(i => WriteToConsoleWithColor((ConsoleColor)i, $"Processing Interval {i}"))
                    .Publish();

            var firstListener =
                intervalObs
                    .Do(val => WriteToConsoleWithColor((ConsoleColor)val, $"First Listener Received: {val}"))
                    .Subscribe();

            var secondListener =
                intervalObs
                    .Do(val => WriteToConsoleWithColor((ConsoleColor)val, $"Second Listener Received: {val}"))
                    .Subscribe();

            intervalObs.Connect();

            await Task.Delay(TimeSpan.FromSeconds(5));

            firstListener.Dispose();
            secondListener.Dispose();
        }

        public static async Task AsyncOrderingBadExample ()
        {
            var rng = new Random();

            await Observable
                .Range(1, 10, TaskPoolScheduler.Default)
                .SelectMany(
                    async range =>
                    {
                        var delay = rng.Next(10, 300);
                        await Task.Delay(delay);

                        WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Finished after {delay}ms");

                        return range;
                    })
                .Do(
                    range =>
                    {
                        WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Received Notification at {DateTimeOffset.Now}");
                    });
        }

        public static async Task AsyncOrderingGoodExample()
        {
            var rng = new Random();

            await Observable
                .Range(1, 10, TaskPoolScheduler.Default)
                .Select(
                    async range =>
                    {
                        var delay = rng.Next(10, 300);
                        await Task.Delay(delay);

                        WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Finished after {delay}ms");

                        return range;
                    })
                .Concat()
                .Do(
                    range =>
                    {
                        WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Received Notification at {DateTimeOffset.Now}");
                    });
        }

        public static async Task LimitConcurrencyBadExample ()
        {
            var rng = new Random();

            await Observable
                .Range(1, 10, TaskPoolScheduler.Default)
                .SelectMany(
                    async range =>
                    {
                        WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Started at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                        var delay = rng.Next(10, 300);
                        await Task.Delay(delay);

                        WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Finished after {delay}ms");

                        return range;
                    })
                .Do(
                    range =>
                    {
                        WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Received Notification at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                    });
        }

        public static async Task LimitConcurrencyGoodExample ()
        {
            var rng = new Random();

            await Observable
                .Range(1, 10, TaskPoolScheduler.Default)
                .Select(
                    range =>
                        Observable
                            .DeferAsync(
                                async ct =>
                                {
                                    WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Started at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                                    var delay = rng.Next(10, 300);
                                    await Task.Delay(delay);

                                    WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Finished after {delay}ms");

                                    return Observable.Return(range);
                                }))
                .Merge(1)
                .Do(
                    range =>
                    {
                        WriteToConsoleWithColor((ConsoleColor)range, $"{range} - Received Notification at {DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
                    });
        }

        public static Task SchedulerExampleBad ()
        {
            var rng = new Random();

            var subscription = Observable
                .Range(1, 10)
                .Repeat()
                .Do(
                    range =>
                    {
                        WriteToConsoleWithColor(
                            (ConsoleColor)range,
                            $"{range} - Received Notification at {DateTimeOffset.Now}");
                    })
                .Subscribe();

            subscription.Dispose();

            return Task.CompletedTask;
        }

        public static Task SchedulerExampleGood ()
        {
            var rng = new Random();

            var subscription = Observable
                .Range(1, 10, TaskPoolScheduler.Default)
                .Repeat()
                .Do(
                    range =>
                    {
                        WriteToConsoleWithColor(
                            (ConsoleColor)range,
                            $"{range} - Received Notification at {DateTimeOffset.Now}");
                    })
                .Subscribe();

            subscription.Dispose();

            return Task.CompletedTask;
        }

        private static void WriteToConsoleWithColor(ConsoleColor color, string value)
        {
            lock(matlock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(value);
            }
        }
    }
}
