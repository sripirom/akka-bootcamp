﻿using System;
using Akka.Actor;

namespace WinTail
{
    public class TailCoordinatorActor : UntypedActor
    {

        #region Message types

        public class StartTail
        {
            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }

            public String FilePath { get; private set; }
            public IActorRef ReporterActor { get; private set; }
        }

        public class StopTail
        {
            public StopTail(String filePath)
            {
                FilePath = filePath;
            }
            public String FilePath { get; private set; }
        }
        #endregion

        protected override void OnReceive(object message)
        {
            if (message is StartTail) {
                var msg = message as StartTail;
                // here we are creating our first parent/child relationship!
                // the TailActor instance created here is a child
                // of this instance of TailCoordinatorActor

                Context.ActorOf(Props.Create(
                    () => new TailActor(msg.ReporterActor, msg.FilePath)));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10, // maxNumberOfRetries
                TimeSpan.FromSeconds(30), // withinTimeRange
                x => // localOnlyDecider
                {
                    //Maybe we consider ArihmeticException to not be application critical
                    //so we just ignore the error and keep going
                    if (x is ArithmeticException) return Directive.Resume;

                    //Error that we cannot recover from, stop the failing actor
                    else if (x is NotSupportedException) return Directive.Stop;

                    //In all other cases, just restart the failing actor
                    else return Directive.Restart;
                });
        }
    }
}