using JetBrains.Annotations;

namespace GitPrompt
{
    internal static class ReSharperHelpers
    {
        [ContractAnnotation("false => halt")]
        // ReSharper disable once InconsistentNaming
        internal static void assume(bool expression)
        {
        }
    }
}
