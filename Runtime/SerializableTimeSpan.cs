﻿using System;
using UnityEngine;

namespace DefaultNamespace
{
    [Serializable]
    public struct SerializableTimeSpan
    {
        private const int SecsInDay = 86400;
        private const int SecsInHour = 3600;
        private const int SecsInMinute = 60;
        
        [SerializeField] [Min(0)] private int days;
        [SerializeField] [Min(0)] private int hours;
        [SerializeField] [Min(0)] private int minutes;
        [SerializeField] [Min(0)] private int seconds;
        
        public int Days => days;

        public int Hours => hours;

        public int Minutes => minutes;

        public int Seconds => seconds;

        public int ToSeconds()
        {
            return days * SecsInDay + hours * SecsInHour + minutes * SecsInMinute + seconds;
        }

        public static SerializableTimeSpan FromSeconds(int seconds)
        {
            int days = seconds / SecsInDay;
            int hours = seconds % SecsInDay / SecsInHour;
            int minutes = seconds % SecsInDay % SecsInHour / SecsInMinute;
            int secs = seconds % SecsInDay % SecsInHour % SecsInMinute;

            return new SerializableTimeSpan
            {
                days = days,
                hours = hours,
                minutes = minutes,
                seconds = secs
            };
        }

        public static SerializableTimeSpan operator +(SerializableTimeSpan a, SerializableTimeSpan b)
        {
            return SerializableTimeSpan.FromSeconds(a.ToSeconds() + b.ToSeconds());
        }
        
        public static SerializableTimeSpan operator -(SerializableTimeSpan a, SerializableTimeSpan b)
        {
            return SerializableTimeSpan.FromSeconds(a.ToSeconds() - b.ToSeconds());
        }

        public override string ToString()
        {
            return $"{days:D3}:{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
    }
}