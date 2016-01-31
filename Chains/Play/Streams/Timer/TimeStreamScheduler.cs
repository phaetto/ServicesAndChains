namespace Chains.Play.Streams.Timer
{
    using System.Collections.Generic;
    using System.Threading;

    public sealed class TimeStreamScheduler : Publisher
    {
        private const int StartNeverTimerValue = Timeout.Infinite;

        private const int PeriodicCallbackDisabled = Timeout.Infinite;

        private const int TimerIdle = int.MinValue;

        private const int MinimumIntervalToStartApplyingActionInMilliseconds = 2;

        private readonly Timer timer;

        private readonly List<TimerScheduledCall> scheduledActions = new List<TimerScheduledCall>();

        private readonly object scheduledActionsSyncObject = new object();

        private int lastRunIntervalInMilliseconds = TimerIdle;

        public bool IsIdle => lastRunIntervalInMilliseconds == TimerIdle;

        public TimeStreamScheduler()
        {
            this.timer = new Timer(
                TimerCallback,
                null,
                StartNeverTimerValue,
                PeriodicCallbackDisabled);
        }

        public void ScheduleActionCall(object action, int intervalInMilliseconds, TimerScheduledCallType timerScheduledCallType)
        {
            ScheduleActionCall(action, intervalInMilliseconds, timerScheduledCallType, CancellationToken.None);
        }

        public void ScheduleActionCall(object action, int intervalInMilliseconds, TimerScheduledCallType timerScheduledCallType, CancellationToken cancellationToken)
        {
            lock (scheduledActionsSyncObject)
            {
                scheduledActions.Add(new TimerScheduledCall(intervalInMilliseconds)
                    {
                        TimerScheduledCallType = timerScheduledCallType,
                        ActionToRepeat = action,
                        NextScheduledTimeToRunInMilliseconds = intervalInMilliseconds,
                        CancellationToken = cancellationToken,
                });
            }

            if (lastRunIntervalInMilliseconds == TimerIdle)
            {
                TimerStartLogic();
            }
        }

        private int FindNextScheduleTime()
        {
            lock (scheduledActionsSyncObject)
            {
                var minimumScheduledTimeToRun = int.MaxValue;
                foreach (var timerScheduledCall in scheduledActions)
                {
                    if (minimumScheduledTimeToRun > timerScheduledCall.NextScheduledTimeToRunInMilliseconds)
                    {
                        minimumScheduledTimeToRun = timerScheduledCall.NextScheduledTimeToRunInMilliseconds;
                    }
                }

                if (minimumScheduledTimeToRun == int.MaxValue)
                {
                    return TimerIdle;
                }

                return minimumScheduledTimeToRun;
            }
        }

        private void TimerCallback(object state)
        {
            var schedulesActivated = new List<TimerScheduledCall>();
            var schedulesCancelled = new List<TimerScheduledCall>();

            lock (scheduledActionsSyncObject)
            {
                foreach (var timerScheduledCall in scheduledActions)
                {
                    timerScheduledCall.NextScheduledTimeToRunInMilliseconds -= lastRunIntervalInMilliseconds;

                    if (timerScheduledCall.CancellationToken.IsCancellationRequested)
                    {
                        schedulesCancelled.Add(timerScheduledCall);
                    }
                    else if (timerScheduledCall.NextScheduledTimeToRunInMilliseconds <= MinimumIntervalToStartApplyingActionInMilliseconds)
                    {
                        schedulesActivated.Add(timerScheduledCall);
                    }
                }
            }

            lock (scheduledActionsSyncObject)
            {
                foreach (var timerScheduledCallCancelled in schedulesCancelled)
                {
                    scheduledActions.Remove(timerScheduledCallCancelled);
                }
            }

            foreach (var timerScheduledCallActivated in schedulesActivated)
            {
                if (timerScheduledCallActivated.CancellationToken.IsCancellationRequested)
                {
                    continue;
                }

                Publish(timerScheduledCallActivated.ActionToRepeat);

                if (timerScheduledCallActivated.TimerScheduledCallType == TimerScheduledCallType.Once)
                {
                    lock (scheduledActionsSyncObject)
                    {
                        scheduledActions.Remove(timerScheduledCallActivated);
                    }
                }
                else
                {
                    timerScheduledCallActivated.NextScheduledTimeToRunInMilliseconds = timerScheduledCallActivated.TimeToRunInMilliseconds;
                }
            }

            TimerStartLogic();
        }

        private void TimerStartLogic()
        {
            lastRunIntervalInMilliseconds = FindNextScheduleTime();

            if (lastRunIntervalInMilliseconds != TimerIdle)
            {
                timer.Change(lastRunIntervalInMilliseconds, PeriodicCallbackDisabled);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            timer.Dispose();
        }
    }
}
