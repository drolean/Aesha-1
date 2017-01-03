using System.Security;
using System.Security.Principal;

namespace ObjectManager.Infrastucture
{
    internal class AdministrativeRights
    {
        public static void Ensure()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            var isAdmin =  principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!isAdmin)
                throw new SecurityException("ObjectManager requires administrative rights to function");
        }
    }
}
