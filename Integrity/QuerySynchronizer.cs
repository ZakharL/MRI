using System;
using System.Threading;

namespace MRI.Integrity
{
    class QuerySynchronizer
    {
        private static volatile QuerySynchronizer _instance;
        private static readonly object Sync = new Object();

        private volatile AutoResetEvent _blockEvent;
        private volatile AutoResetEvent _queryEvent;
                
        private bool RestrictQueueMode { get; set; }
        private int _concurrentQueryCounter;

        private readonly bool _isLogTurnOn;

        private QuerySynchronizer()
        {
            _blockEvent = new AutoResetEvent(true);
            _queryEvent = new AutoResetEvent(true);
            RestrictQueueMode = true;
            _isLogTurnOn = false;
        }

        public static QuerySynchronizer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                            _instance = new QuerySynchronizer();
                    }
                }
                return _instance;
            }
        }

        public void StartBlockingAction(int Id = 0)
        {
            _blockEvent.WaitOne();
            
            if (RestrictQueueMode)
            {
                _queryEvent.WaitOne();
                _queryEvent.Set();
            }

            if (_isLogTurnOn)
            {
                Console.WriteLine("Start " + Convert.ToString(Id) + " update");    
            }            
        }

        public void StopBlockingAction(int Id = 0)
        {
            if (_isLogTurnOn)
            {
                Console.WriteLine("Finish " + Convert.ToString(Id) + " update");    
            }
            
            _blockEvent.Set();
        }

        public void StartAction(int Id = 0)
        {
            _blockEvent.WaitOne();
            _blockEvent.Set();

            if (RestrictQueueMode && Interlocked.Increment(ref _concurrentQueryCounter) == 1)
                _queryEvent.WaitOne();

            if (_isLogTurnOn)
            {
                Console.WriteLine("Start " + Convert.ToString(Id) + " select");    
            }
        }

        public void StopAction(int Id = 0)
        {
            if (RestrictQueueMode && Interlocked.Decrement(ref _concurrentQueryCounter) == 0)
                _queryEvent.Set();

            if (_isLogTurnOn)
            {
                Console.WriteLine("Finish " + Convert.ToString(Id) + " select");    
            }
        }
    }
}
