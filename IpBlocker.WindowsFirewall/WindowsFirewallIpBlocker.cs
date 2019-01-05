using IpBlocker.Core;
using IpBlocker.Core.Interfaces;
using IpBlocker.Core.Objects;
using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IpBlocker.WindowsFirewall
{
    public class WindowsFirewallIpBlocker : IFirewallIpBlocker
    {
        private const string RuleNameFormat = "{0}[{1}]";
        private readonly IIPBlockPolicyFactory policyFactory;
        private Dictionary<string, List<INetFwRule>> _rules;
        private const int MaxRules = 20;

        public WindowsFirewallIpBlocker(IIPBlockPolicyFactory policyFactory)
        {
            _rules = new Dictionary<string, List<INetFwRule>>();
            this.policyFactory = policyFactory;
        }

        public bool Block(BlockedEntry blockEntry, IPBlockPolicy policy, out string ruleName)
        {
            var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            ruleName = $"{policy.GetRuleName()} [{blockEntry.Protocol}-{String.Join(",", blockEntry.Ports)}]";

            var rules = GetRules(firewallPolicy, ruleName);

            var rule = rules.FirstOrDefault(r => CleanIps(r.RemoteAddresses).Count < 1000);

            if (rule != null)
            {
                var ips = CleanIps(rule.RemoteAddresses);

                if (!ips.Contains(blockEntry.Ip))
                {
                    ips.Add(blockEntry.Ip);

                    rule.RemoteAddresses = String.Join(",", ips);
                    ruleName = rule.Name;
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
                rule.Description = $"IPBlocker blocked IP's";
                rule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                rule.Enabled = true;
                rule.RemoteAddresses = blockEntry.Ip;
                rule.InterfaceTypes = "All";
                rule.Protocol = (int)blockEntry.Protocol; //NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP / NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP
                rule.LocalPorts = String.Join(",", blockEntry.Ports);
                rule.Name = GetNextRuleName(rules.Select(r => r.Name).ToList(), ruleName);

                firewallPolicy.Rules.Add(rule);
                rules.Add(rule);

                ruleName = rule.Name;
                Console.WriteLine($"Blocked IP: {blockEntry.Ip}");
            }

            return true;
        }

        public bool Unblock(BlockedEntry entry)
        {
            var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            var policy = policyFactory.GetPolicy(entry);

            var ruleName = $"{policy.GetRuleName()} [{entry.Protocol}-{String.Join(",", entry.Ports)}]";
            var rules = GetRules(firewallPolicy, ruleName);
            var toRemove = new List<INetFwRule>();

            foreach (var rule in rules.Where(r => CleanIps(r.RemoteAddresses).Contains(entry.Ip)))
            {
                var ips = CleanIps(rule.RemoteAddresses);

                ips.Remove(entry.Ip);

                if (ips.Count() == 0)
                {
                    toRemove.Add(rule);
                }
                else
                {
                    rule.RemoteAddresses = String.Join(",", ips);
                }
            }

            foreach (var rule in toRemove)
            {
                firewallPolicy.Rules.Remove(rule.Name);
                rules.Remove(rule);
            }

            return true;
        }

        private string GetNextRuleName(List<string> names, string ruleName)
        {
            var count = 1;
            var name = String.Format(RuleNameFormat, ruleName, count);

            while (names.Any(n => n == name))
            {
                count++;
                name = String.Format(RuleNameFormat, ruleName, count); ;
            }

            return name;
        }

        private List<string> CleanIps(string remoteAddresses)
        {
            return remoteAddresses.Split(',').Select(ip => ip.Split('/')[0]).ToList();
        }

        private List<INetFwRule> GetRules(INetFwPolicy2 firewallPolicy, string ruleName)
        {
            if (_rules.ContainsKey(ruleName))
            {
                return _rules[ruleName];
            }

            var count = 1;
            var firewallRules = new List<INetFwRule>();

            while (count < MaxRules)
            {
                try
                {
                    var name = String.Format(RuleNameFormat, ruleName, count);
                    var rule = firewallPolicy.Rules.Item(name);

                    if (rule != null)
                    {
                        firewallRules.Add(rule);
                    }
                }
                catch (Exception)
                {
                }

                count++;
            }

            _rules.Add(ruleName, firewallRules);

            return firewallRules;
        }
    }
}