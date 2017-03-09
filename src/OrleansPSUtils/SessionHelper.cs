﻿using System;

namespace OrleansPSUtils
{
    using System.Management.Automation;

    using Orleans;

    internal static class SessionHelper
    {
        private const string ClusterClientVariableName = "ClusterClient";

        public static IClusterClient GetClient(this PSCmdlet cmdlet)
        {
            return cmdlet.SessionState.PSVariable.GetValue(ClusterClientVariableName) as IClusterClient;
        }

        public static void SetClient(this PSCmdlet cmdlet, IClusterClient client)
        {
            cmdlet.SessionState.PSVariable.Set(ClusterClientVariableName, client);
        }

        public static void CloseClient(this PSCmdlet cmdlet, IClusterClient client)
        {
            try
            {
                if (client == null) return;

                try
                {
                    client.Close().Wait();
                }
                catch (Exception exception)
                {
                    cmdlet.WriteError(
                        new ErrorRecord(
                            exception,
                            $"{nameof(IClusterClient)}{nameof(IClusterClient.Close)}Failed",
                            ErrorCategory.CloseError,
                            client));
                }

                client.Dispose();
            }
            finally
            {
                cmdlet.SetClient(null);
            }
        }
    }
}