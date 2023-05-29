
class AllSystems
{
    public static void Register()
    {
        Core Core = Core.Instance;

        Core.Register<Logger>();
        Core.Register<Closer>();
        Core.Register<Version>();
        Core.Register<Credentials>();
        Core.Register<Vk>();
        Core.Register<TestImage>(true);
        Core.Register<MessageInput>();
    }
}