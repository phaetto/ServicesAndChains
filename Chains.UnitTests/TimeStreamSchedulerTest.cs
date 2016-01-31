namespace Chains.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Chains.Play.Streams.Timer;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    public class TimeStreamSchedulerTest
    {
        [TestMethod]
        public void TimeStreamScheduler_WhenSchedulingOneFutureCallback_ThenOneEventIsReturned()
        {
            var contextForTest = new ContextForTest();

            using (var timeStreamScheduler = new TimeStreamScheduler())
            {
                Assert.IsTrue(timeStreamScheduler.IsIdle);

                timeStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 100, TimerScheduledCallType.Once);

                var infiniteStream = contextForTest.Do(timeStreamScheduler.AsStream<ActionForTest>()).GetEnumerator();

                Assert.IsTrue(infiniteStream.MoveNext());

                Assert.AreEqual("timer hit", contextForTest.ContextVariable);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public void TimeStreamScheduler_WhenCancelling_ThenShouldThrow()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();

            using (var timeStreamScheduler = new TimeStreamScheduler())
            {
                var infiniteStream = contextForTest.Do(timeStreamScheduler.AsStream<ActionForTest>(cancellationTokenSource.Token)).GetEnumerator();

                Task.Delay(50).ContinueWith(x =>
                    {
                        cancellationTokenSource.Cancel();
                    });

                infiniteStream.MoveNext();
            }
        }

        [TestMethod]
        public void TimeStreamScheduler_WhenSchedulingAndCancelling_ThenNoEventHasRun()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            using (var timeStreamScheduler = new TimeStreamScheduler())
            {
                timeStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 100, TimerScheduledCallType.Once);

                Task.Delay(10).ContinueWith(x =>
                    {
                        var infiniteStream =
                            contextForTest.Do(timeStreamScheduler.AsStream<ActionForTest>(cancellationTokenSource.Token))
                                .GetEnumerator();

                        Assert.IsFalse(infiniteStream.MoveNext());
                    });

                cancellationTokenSource.Cancel();

                Assert.AreNotEqual("timer hit", contextForTest.ContextVariable);
            }
        }

        [TestMethod]
        public async Task TimeStreamScheduler_WhenSchedulingANumberOfEventsAndCancelling_ThenExactNumberOfEventsAreRunning()
        {
            var contextForTest = new ContextForTest();
            var contextForTest2 = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationTokenSource2 = new CancellationTokenSource();
            using (var timeStreamScheduler = new TimeStreamScheduler())
            {
#pragma warning disable 4014
                contextForTest.DoAsync(timeStreamScheduler.AsStream<ActionForTest>(cancellationTokenSource.Token));
                contextForTest2.DoAsync(timeStreamScheduler.AsStream<ActionForTest>(cancellationTokenSource2.Token));
#pragma warning restore 4014

                timeStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit 1"), 10, TimerScheduledCallType.Once);
                timeStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit 2"), 20, TimerScheduledCallType.Once);
                timeStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit 3"), 30, TimerScheduledCallType.Once);
                timeStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit 4"), 40, TimerScheduledCallType.Once);

                await Task.Delay(25).ContinueWith(x =>
                    {
                        // Cancel after the 2nd event
                        cancellationTokenSource.Cancel();
                    });

                Assert.AreEqual("timer hit 2", contextForTest.ContextVariable);

                await Task.Delay(50).ContinueWith(x =>
                {
                    // Cancel after everything has finished
                    cancellationTokenSource2.Cancel();
                });

                Assert.AreEqual("timer hit 4", contextForTest2.ContextVariable);
            }
        }

        [TestMethod]
        public async Task TimeStreamScheduler_WhenSchedulingRecurrentEventsAndCancellingSubscription_ThenExactNumberOfEventsAreRunning()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            using (var timeStreamScheduler = new TimeStreamScheduler())
            {
                timeStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 10, TimerScheduledCallType.Recurrent);

                await Task.Factory.StartNew(() =>
                    {
                        var infiniteStream =
                            contextForTest.Do(timeStreamScheduler.AsStream<ActionForTest>(cancellationTokenSource.Token))
                                .GetEnumerator();

                        Assert.IsTrue(infiniteStream.MoveNext());

                        Assert.IsTrue(infiniteStream.MoveNext());

                        cancellationTokenSource.Cancel();
                    });

                Assert.AreEqual("timer hit", contextForTest.ContextVariable);
            }
        }

        [TestMethod]
        public async Task TimeStreamScheduler_WhenSchedulingRecurrentEventsAndCancellingEvent_ThenEventIsCancelled()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            using (var timeStreamScheduler = new TimeStreamScheduler())
            {
                timeStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 10, TimerScheduledCallType.Recurrent, cancellationTokenSource.Token);

                await Task.Factory.StartNew(() =>
                {
                    var infiniteStream =
                        contextForTest.Do(timeStreamScheduler.AsStream<ActionForTest>())
                            .GetEnumerator();

                    Assert.IsTrue(infiniteStream.MoveNext());

                    Assert.IsTrue(infiniteStream.MoveNext());

                    cancellationTokenSource.Cancel();
                });

                Assert.AreEqual("timer hit", contextForTest.ContextVariable);
            }
        }

        [TestMethod]
        public async Task TimeStreamScheduler_WhenSchedulingDifferentEvents_ThenEventsAreFilteredCorrectly()
        {
            var contextForTest = new ContextForTest();
            var cancellationTokenSource = new CancellationTokenSource();
            using (var timeStreamScheduler = new TimeStreamScheduler())
            {
                timeStreamScheduler.ScheduleActionCall(new ActionForTest("timer hit"), 50, TimerScheduledCallType.Once, cancellationTokenSource.Token);
                timeStreamScheduler.ScheduleActionCall(new ActionForTestAsync("timer hit (async)"), 10, TimerScheduledCallType.Recurrent);

#pragma warning disable 4014
                Task.Factory.StartNew(() =>
#pragma warning restore 4014
                    {
                        var infiniteStream =
                            contextForTest.Do(timeStreamScheduler.AsStream<ActionForTest>())
                                .GetEnumerator();

                        Assert.IsTrue(infiniteStream.MoveNext());

                        // This should block - and get false
                        Test.Throws<OperationCanceledException>(() => infiniteStream.MoveNext());
                    });

                await Task.Delay(70).ContinueWith(x =>
                    {
                        // Cancel after everything has finished
                        cancellationTokenSource.Cancel();
                    });

                Assert.AreEqual("timer hit", contextForTest.ContextVariable);
            }
        }
    }
}
