// This sample demonstrates the use of the WindowsIdentity class to impersonate a user.
// IMPORTANT NOTES: 
// This sample can be run only on Windows XP.  The default Windows 2000 security policy 
// prevents this sample from executing properly, and changing the policy to allow
// proper execution presents a security risk. 
// This sample requests the user to enter a password on the console screen.
// Because the console window does not support methods allowing the password to be masked, 
// it will be visible to anyone viewing the screen.
// The sample is intended to be executed in a .NET Framework 1.1 environment.  To execute
// this code in a 1.0 environment you will need to use a duplicate token in the call to the
// WindowsIdentity constructor. See KB article Q319615 for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;

[assembly: SecurityPermissionAttribute(SecurityAction.RequestMinimum, UnmanagedCode = true)]
[assembly: PermissionSetAttribute(SecurityAction.RequestMinimum, Name = "FullTrust")]
namespace MvcAppHyLinedoBrasil.Models
{   
    public class LoginAPIWindows
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        //[DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        //private unsafe static extern int FormatMessage(int dwFlags, ref IntPtr lpSource,
            //int dwMessageId, int dwLanguageId, ref String lpBuffer, int nSize, IntPtr* Arguments);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr ExistingTokenHandle,
            int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

        // Test harness.
        // If you incorporate this code into a DLL, be sure to demand FullTrust.
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public string LogonUserAPI(string domainName, string userName, string password)
        {
            IntPtr tokenHandle = new IntPtr(0);
            IntPtr dupeTokenHandle = new IntPtr(0);
            string retorno = "";
            try
            {
                // Get the user token for the specified user, domain, and password using the 
                // unmanaged LogonUser method.  
                // The local machine name can be used for the domain name to impersonate a user on this machine.
                const int LOGON32_PROVIDER_DEFAULT = 0;
                //This parameter causes LogonUser to create a primary token.
                const int LOGON32_LOGON_INTERACTIVE = 2;
                const int ERROR_PASSWORD_MUST_CHANGE = 1907;
                const int ERROR_PASSWORD_EXPIRED = 1330;
                const int LOGON_TYPE_NEW_CREDENTIALS = 9;

                tokenHandle = IntPtr.Zero;

                // Call LogonUser to obtain a handle to an access token.
                bool returnValue = LogonUser(userName, domainName, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    ref tokenHandle);

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    retorno = ret.ToString();
                    //retorno = "LogonUser failed with error code : " + ret.ToString();
                    //throw new System.ComponentModel.Win32Exception(ret);
                }

                //Console.WriteLine("Did LogonUser Succeed? " + (returnValue ? "Yes" : "No"));
                //Console.WriteLine("Value of Windows NT token: " + tokenHandle);

                // Check the identity.
                //Console.WriteLine("Before impersonation: "
                //    + WindowsIdentity.GetCurrent().Name);
                // Use the token handle returned by LogonUser.
                //WindowsIdentity newId = new WindowsIdentity(tokenHandle);
                //WindowsImpersonationContext impersonatedUser = newId.Impersonate();

                // Check the identity.
                //Console.WriteLine("After impersonation: "
                //    + WindowsIdentity.GetCurrent().Name);

                // Stop impersonating the user.
                //impersonatedUser.Undo();

                // Check the identity.
                //Console.WriteLine("After Undo: " + WindowsIdentity.GetCurrent().Name);

                // Free the tokens.
                //if (tokenHandle != IntPtr.Zero)
                //    CloseHandle(tokenHandle);

                return retorno;

            }
            catch (Exception ex)
            {
                retorno = ex.Message;
                return retorno;
            }

        }
    }
}