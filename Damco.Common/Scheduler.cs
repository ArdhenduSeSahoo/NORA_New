 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Damco.Common
{
    public enum SchedulingStartType
    {
        Immediate = 1,
        AfterPeriod = 2,
        RandomWithinPeriod = 3
    }


    /// <summary>
    /// Generic scheduling utility to remove some of the complexity that usually entails.
    /// Wraps System.Threading.Timer 
    /// </summary>
    public class Scheduler: IDisposable
    {
        Timer _timer;
        List<Action> _work;
        bool _multipleSimultaneousRuns = false;
        bool _isRunning = false;

        public Scheduler(Action work, TimeSpan period, bool multipleSimultaneousRuns, SchedulingStartType startType)
            : this(new Action[] { work }, period, multipleSimultaneousRuns, startType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Scheduler class, using a timespan,
        /// flag to run multiple instances and Schedule startup type. 
        /// </summary>
        /// <param name="work"> List of work delegate representing a methods to be executed.</param>
        /// <param name="period">The time interval between invocations of work, converted to milliseconds.</param>
        /// <param name="multipleSimultaneousRuns">Flag to run multiple callback method simultaneously. </param>
        /// <param name="startType">Type of startup (Immediate,AfterPeriod,RandomWithinPeriod). </param>
        public Scheduler(IEnumerable<Action> work, TimeSpan period, bool multipleSimultaneousRuns, SchedulingStartType startType)
        {
            _work = work.ToList();
            _multipleSimultaneousRuns = multipleSimultaneousRuns;
            _timer = new Timer(DoSchedule, null,
                (startType == SchedulingStartType.Immediate ? 0 :
                startType == SchedulingStartType.AfterPeriod ? (int)period.TotalMilliseconds :
                startType == SchedulingStartType.RandomWithinPeriod ? (int)new Random().Next(0, (int)period.TotalMilliseconds) :
                0),
                (int)period.TotalMilliseconds);
        }

        private void DoSchedule(object state)
        {
            if ((_multipleSimultaneousRuns || !_isRunning) && !_disposeWhenDone)
            {
                try
                {
                    _isRunning = true;
                    foreach (Action work in _work)
                        work.Invoke();
                }
                finally
                {
                    _isRunning = false;
                }
            }
        }

        bool _disposeWhenDone;

        /// <summary>
        /// Release all resources used by the current instance of scheduler.
        /// </summary>
        public void DisposeWhenDone()
        {
            if(!_isRunning)
                this.Dispose();
            else
                _disposeWhenDone = true;
            while (_isRunning)
                System.Threading.Thread.Sleep(10);
            this.Dispose();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

    }
}
