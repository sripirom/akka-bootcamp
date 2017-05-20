using Akka.Actor;

namespace WinTail.Helpers
{
    public static class ActorPaths
    {
        public static readonly ActorMetaData FileValidationActor = new ActorMetaData("validationActor");

        public static readonly ActorMetaData TailCoordinatorActor = new ActorMetaData("tailCoordinatorActor");
        
    }
}
