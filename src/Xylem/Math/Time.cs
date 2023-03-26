
using System;

namespace Xylem.Vectors
{
    public class Stopwatch
    {
        private DateTime _lastTime;

        private double _elapsedTime;
        private double _limit;

        private bool _stopped = true;
        private bool _reversed = false;
        private bool _paused = false;

        public Stopwatch(double limit = double.MaxValue)
        {
            _limit = limit;
        }

        public void Start()
        {
            _lastTime = DateTime.Now;
            _stopped = false;
            _reversed = false;
        }

        public void Stop()
        {
            _stopped = true;
            _reversed = false;
            _elapsedTime = 0D;
        }

        public void SetPaused(bool paused)
        {
            _paused = paused;
            _lastTime = DateTime.Now;
        }

        public void Reverse()
        {
            _reversed = true;
        }

        public double GetTime()
        {
            if (_stopped)
                return 0D;

            if (_paused)
                return _elapsedTime;

            if (_reversed)
                _elapsedTime -= (DateTime.Now - _lastTime).TotalSeconds;
            else if (_elapsedTime < _limit)
                _elapsedTime += (DateTime.Now - _lastTime).TotalSeconds;

            if (_elapsedTime <= 0D)
            {
                Stop();
                return 0D;
            }

            _lastTime = DateTime.Now;

            return _elapsedTime;
        }

        public double GetProgress()
        {
            if (_limit == double.MaxValue)
                return GetTime();
            else 
                return System.Math.Min(GetTime() / _limit, 1);
        }
    }
}