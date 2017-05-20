using System;
using Akka.Actor;
using System.IO;

namespace WinTail
{
    /// <summary>
    /// Actor that validates user input and signals result to others.
    /// </summary>
    /// <seealso cref="Akka.Actor.UntypedActor" />
    public class FileValidationActor : UntypedActor
    {
        private readonly IActorRef _conosleWriterActor;

        public FileValidationActor(IActorRef consoleWriterActor)
        {
            _conosleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;

            if (String.IsNullOrEmpty(msg))
            {
                // signal that the user needs to supply an input
                _conosleWriterActor.Tell(new Messages.NullInputError("Input was blank Please try again.\n"));

                // tell sender to continue doing its thing (whatever that may be, this actor doesn't care
                Sender.Tell(new Messages.ConinueProcessing());
            }
            else {
                var valid = IsFileUri(msg);
                if (valid)
                {
                    // signal successful input
                    _conosleWriterActor.Tell(new Messages.InputSuccess(
                        String.Format("Starting processing for {0}", msg)));

                    // start coordinator
                    Context.ActorSelection(Helpers.ActorPaths.TailCoordinatorActor.Path).Tell(
                        new TailCoordinatorActor.StartTail(msg, _conosleWriterActor));
                }
                else {
                    // signal that input was bad
                    _conosleWriterActor.Tell(new Messages.ValidationError(
                        String.Format("{0} is not an existing URI on disk.", msg)));

                    // tell sender to continue doing its thing (whatever that 
                    // may be, this actor doesn't care)
                    Sender.Tell(new Messages.ConinueProcessing());
                }
            }

            // tell sender to continue doing its thing
            // (whatever that may be, this actor doesn't care)
            Sender.Tell(new Messages.ConinueProcessing());
        }

        private Boolean IsFileUri(string path)
        {
            return File.Exists(path);
        }
    }
}
