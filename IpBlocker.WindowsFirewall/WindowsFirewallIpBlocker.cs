using IpBlocker.Core;
using IpBlocker.Core.Objects;
using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using IpBlocker.Core.Interfaces;

namespace IpBlocker.WindowsFirewall
{
    public class WindowsFirewallIpBlocker : IFirewallIpBlocker
    {
        public bool Block(BlockedEntry blockEntry, IPBlockPolicy policy, out string ruleName)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            ruleName = $"{policy.GetRuleName()} [{blockEntry.Protocol}-{blockEntry.Port}]";

            INetFwRule rule = GetRule(firewallPolicy, ruleName);
            if (rule != null)
            {
                var ips = CleanIps(rule.RemoteAddresses);

                if (!ips.Contains(blockEntry.Ip))
                {
                    ips.Add(blockEntry.Ip);

                    rule.RemoteAddresses = String.Join(",", ips);
                    Console.WriteLine($"Blocked IP: {blockEntry.Ip}");
                }
                else
                {
                    Console.WriteLine($"IP AlreadyBlocked: {blockEntry.Ip}");
                }
            }
            else
            {
                rule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                rule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                rule.Description = "Blocked due to invalid dns requests";
                rule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                rule.Enabled = true;
                rule.RemoteAddresses = blockEntry.Ip;
                rule.InterfaceTypes = "All";
                rule.Protocol = (int)blockEntry.Protocol; //NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP / NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP
                rule.LocalPorts = blockEntry.Port.ToString();
                rule.Name = ruleName;
                firewallPolicy.Rules.Add(rule);

                Console.WriteLine($"Blocked IP: {blockEntry.Ip}");
            }

            return true;
        }

        private List<string> CleanIps(string remoteAddresses)
        {
            return remoteAddresses.Split(',').Select(ip => ip.Split('/')[0]).ToList();
        }

        private static INetFwRule GetRule(INetFwPolicy2 firewallPolicy, string ruleName)
        {
            INetFwRule rule;
            try
            {
                rule = firewallPolicy.Rules.Item(ruleName);
            }
            catch (Exception)
            {
                rule = null;
            }
            return rule;
        }
    }
}