
class Registration
{
    public static void AddAll()
    {
        Core Core = Core.Instance;

        // Systems
        Core.Register<Logger>();
        Core.Register<Reflector>();
        Core.Register<Closer>();
        Core.Register<Version>();
        Core.Register<Credentials>();
        Core.Register<Vk>();
        Core.Register<Scenes>();

        // Processors
        Core.Register<Photos>();
        Core.Register<MessageReciever>();
        Core.Register<Commander>();
        Core.Register<MessageSender>();
        Core.Register<Sglypa>();

        // Commands
        Core.Register<Help>();
        Core.Register<Eblan>();
        Core.Register<TestImage>();
        Core.Register<Parnishka>();
        Core.Register<RimNotif>();
        Core.Register<RimAlert>();
    }
}
