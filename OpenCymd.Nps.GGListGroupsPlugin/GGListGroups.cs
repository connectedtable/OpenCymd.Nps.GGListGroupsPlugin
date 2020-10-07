using System;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Text;

using log4net;

using OpenCymd.Nps.Plugin;

namespace OpenCymd.Nps.GGListGroupsPlugin
{
    public class GGListGroupsPlugin : INpsPlugin
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GGListGroupsPlugin));

        /// <inheritdoc/>
        public void Initialize()
        {
            //Logger.Info("Initialize");
        }

        /// <inheritdoc/>
        public void RadiusExtensionProcess(IExtensionControl control)
        {
            Logger.Info("RadiusExtensionProcess");
            Logger.DebugFormat("RequestType: {0}", control.RequestType);
            Logger.DebugFormat("ResponseType: {0}", control.ResponseType);
            Logger.DebugFormat("ExtensionPoint: {0}", control.ExtensionPoint);

            if (control.ExtensionPoint != RadiusExtensionPoint.Authorization)
            {
                Logger.Debug("Returning due to not Authorization request");
                // We only want to handle authorization requests
                return;
            }

            // Establish domain context       
            PrincipalContext domain = new PrincipalContext(ContextType.Domain);

            UserPrincipal user = null;
            String UserName = null;

            foreach (var attrib in control.Request)
            {
                var attribName = Enum.IsDefined(typeof(RadiusAttributeType), attrib.AttributeId)
                                        ? ((RadiusAttributeType)attrib.AttributeId).ToString()
                                        : attrib.AttributeId.ToString(CultureInfo.InvariantCulture);                
                if (attribName.Equals("UserName"))
                {
                    UserName = Encoding.UTF8.GetString((byte[])attrib.Value).TrimEnd('\0');
                }
                else if (attribName.Equals("StrippedUserName"))
                {
                    UserName = Encoding.UTF8.GetString((byte[])attrib.Value).TrimEnd('\0');                   
                }              
            }

            if (UserName != null)
            {
                try
                {
                    user = UserPrincipal.FindByIdentity(domain, UserName);
                }
                catch (Exception e)
                {
                    Logger.Debug(e.Message);
                }

                if (user != null)
                {
                    PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                    foreach (Principal group in groups)
                    {
                        String DistinguishedName = group.DistinguishedName;

                        // Example OU
                        if (DistinguishedName != null &&
                            DistinguishedName.EndsWith(",OU=Access,OU=Applications,OU=Root,DC=example,DC=com"))
                        {
                            Logger.DebugFormat("Adding group {0}", group);
                            control.Response[RadiusCode.AccessAccept].Add(
                                // VENDOR Fortinet 12356
                                // ATTRIBUTE   Fortinet-Group-Name                           1 string
                                new RadiusAttribute(RadiusAttributeType.VendorSpecific, new VendorSpecificAttribute(12356, 1, Encoding.ASCII.GetBytes(group.ToString())))
                            );
                        }
                    }
                }
            }
        }
    }
}
