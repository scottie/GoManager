﻿using PokemonGoGUI.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PokemonGoGUI.AccountScheduler
{
    public class Scheduler
    {
        public delegate void CheckHandler(object sender, SchedulerEventArgs e);
        public event CheckHandler OnSchedule;

        private const int Minute = 60000;

        public string Name { get; set; }
        public bool Enabled { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public double OffSet { get; set; }

        public SchedulerLimiter PokeStoplimiter { get; set; }
        public SchedulerLimiter PokemonLimiter { get; set; }
        public SchedulerOption MasterOption { get; set; }

        public Color NameColor { get; set; }

        private Timer _timer = new Timer(Minute * 5);

        public int CheckTime
        {
            get
            {
                return _checkTime / Minute;
            }
            set
            {
                _checkTime = value * Minute;

                _timer.Interval = _checkTime;
            }

        }

        public string PokemonSettings
        {
            get
            {
                if(PokemonLimiter == null)
                {
                    return "Unknown";
                }

                return String.Format("{0}-{1} {2}", PokemonLimiter.Min, PokemonLimiter.Max, PokemonLimiter.Option);
            }
        }

        public string PokestopSettings
        {
            get
            {
                if (PokeStoplimiter == null)
                {
                    return "Unknown";
                }

                return String.Format("{0}-{1} {2}", PokeStoplimiter.Min, PokeStoplimiter.Max, PokeStoplimiter.Option);
            }
        }

        private int _checkTime = Minute * 5;

        public Scheduler()
        {
            NameColor = Color.LightGray;

            Name = "Change Name";
            PokeStoplimiter = new SchedulerLimiter();
            PokemonLimiter = new SchedulerLimiter();
            MasterOption = SchedulerOption.StartStop;

            _timer = new Timer(_checkTime);
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;

            _timer.Start();
        }

        public void ForceCall()
        {
            CheckCaller();
        }

        public bool WithinTime()
        {
            int hour = DateTime.Now.Hour;
            int minutes = DateTime.Now.Minute;
            double hourPercent = (double)minutes / 60;

            double currentHour = hour + hourPercent;

            //Always run
            if (StartTime == EndTime)
            {
                return true;
            }

            //In the middle
            if (StartTime <= EndTime)
            {
                if (currentHour >= StartTime && currentHour <= EndTime)
                {
                    return true;
                }
            }
            else if (StartTime >= EndTime) //Running across day line
            {
                if(currentHour <= EndTime || currentHour >= StartTime)
                {
                    return true;
                }
            }

            return false;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CheckCaller();
        }

        private void CheckCaller()
        {
            CheckHandler caller = OnSchedule;

            if (caller != null)
            {
                caller(this, new SchedulerEventArgs(this));
            }
        }
    }

    public class SchedulerEventArgs : EventArgs
    {
        public Scheduler Scheduler { get; set; }

        public SchedulerEventArgs(Scheduler scheduler)
        {
            Scheduler = scheduler;
        }
    }
}
