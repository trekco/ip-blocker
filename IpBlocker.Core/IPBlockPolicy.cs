using System;

namespace IpBlocker.Core
{
    public class IPBlockPolicy
    {
        private readonly bool block;
        private readonly DateTime unblockDate;

        private readonly string ruleName;

        public IPBlockPolicy(bool block, DateTime unblockDate, string ruleName)
        {
            this.block = block;
            this.unblockDate = unblockDate;
            this.ruleName = ruleName;
        }

        public bool ShouldBlock()
        {
            return block;
        }

        public DateTime GetUnblockDate()
        {
            return unblockDate;
        }

        public string GetRuleName()
        {
            return ruleName;
        }
    }
}