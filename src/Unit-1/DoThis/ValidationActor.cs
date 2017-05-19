using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor that validates user input and signals result to others.
    /// </summary>
    /// <seealso cref="Akka.Actor.UntypedActor" />
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _conosleWriterActor;


        public ValidationActor(IActorRef consoleWriterActor)
        {
            _conosleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;

            if (String.IsNullOrEmpty(msg))
            {
                // signal that the user needs to supply an input
                _conosleWriterActor.Tell(new Messages.NullInputError("No input received."));
            }
            else {
                var valid = IsValid(msg);
                if (valid)
                {
                    // send success to console writer
                    _conosleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));
                }
                else {
                    // signal that input was bad
                    _conosleWriterActor.Tell(new Messages.ValidationError("Invalid: input had odd number of charactors."));
                }
            }

            // tell sender to continue doing its thing
            // (whatever that may be, this actor doesn't care)
            Sender.Tell(new Messages.ConinueProcessing());
        }

        private Boolean IsValid(string msg)
        {
            var valid = msg.Length % 2 == 0;
            return valid;
        }
    }
}
