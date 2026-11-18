namespace SE7.Invocation.Dispatch.Static.SourceGeneration
{
    internal sealed class Class
    {
        internal enum ClassModifiers
        {
            None,
            Sealed,
            Abstract
        }

        internal string Name { get; }
        internal ClassModifiers Modifiers { get; }

        internal Class(string name, ClassModifiers classModifiers)
        {

        }
    }
}
