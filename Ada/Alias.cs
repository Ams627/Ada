using System;
using System.Collections.Generic;

namespace Ada
{
    internal class Alias
    {
        private readonly List<string> bashAliases = new List<string>();
        private readonly List<string> bashDirAliases = new List<string>();
        private readonly List<string> tccAliases = new List<string>();
        private readonly List<string> tccDirAliases = new List<string>();
        private readonly List<string> cmdDirAliases = new List<string>();
        public Alias()
        {
        }

        internal int Add(AddOptions addOptions)
        {
            throw new NotImplementedException();
        }

        internal int List()
        {
            throw new NotImplementedException();
        }

        internal int Remove()
        {
            throw new NotImplementedException();
        }
    }
}