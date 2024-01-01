using System.Diagnostics;

namespace HPImageViewer.Core.Miscs
{
    /// <summary>
    /// 限制流量
    /// </summary>
    public class DataTrafficLimiter
    {
        /// <summary>
        /// unit: ms
        /// </summary>
        double _controlCycleTime;
        int _acceptCountInCylceTime;
        public DataTrafficLimiter(double controlCycleTime, int acceptCountInCylceTime)
        {
            _acceptCountInCylceTime = acceptCountInCylceTime;
            _controlCycleTime = controlCycleTime;
        }
        Stopwatch _stopwatch = new Stopwatch();
        private object _syncLock = new object();
        private int _currentCount = 0;

        public bool TryAdd(int value)
        {
            lock (_syncLock)
            {
                if (_stopwatch.IsRunning == false || _stopwatch.ElapsedMilliseconds >= _controlCycleTime)
                {
                    _stopwatch.Restart();
                    _currentCount = 0;

                }

                if (_currentCount >= _acceptCountInCylceTime)
                {
                    //  Console.WriteLine("do###############################");
                    return false;

                }

                _currentCount += value;
                // Console.WriteLine("rest---------------------------");
                return true;
            }
        }


    }
}
