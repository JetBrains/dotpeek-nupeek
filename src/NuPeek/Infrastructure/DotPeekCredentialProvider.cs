using System;
using System.Net;
using System.Windows.Forms;
using NuGet;

namespace JetBrains.DotPeek.Plugins.NuPeek.Infrastructure
{
    public class DotPeekCredentialProvider 
        : ICredentialProvider
    {
        public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
        {
            if (credentialType == CredentialType.RequestCredentials)
            {
                var dialog = new CredentialsDialog(uri.ToString(), "Connecting to " + uri.Host + "...", "Please provide credentials to connect to " + uri);
                if (dialog.Show() == DialogResult.OK)
                {
                    return new NetworkCredential(dialog.Username, dialog.Password);
                }
            }
            return null;
        }
    }
}