using System.Security;
using System.Security.Principal;

namespace Aesha.Infrastructure
{
    internal static class AdministrativeRights
    {
        public static void Ensure()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            var isAdmin =  principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!isAdmin)
                throw new SecurityException("Aesha requires administrative rights to function");
        }
    }
}
