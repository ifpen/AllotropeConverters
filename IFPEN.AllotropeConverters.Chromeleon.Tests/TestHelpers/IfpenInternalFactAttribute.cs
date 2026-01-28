using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IFPEN.AllotropeConverters.Chromeleon.Tests.TestHelpers
{
    /// <summary>
    /// Xunit Fact attribute to ignore some tests outside IFPEN environment.
    /// Used to flag integration tests relying on IFPEN internal resources.
    /// </summary>
    internal class IfpenInternalFactAttribute: FactAttribute
    {
        public IfpenInternalFactAttribute()
        {
            bool isIfpenDomaine = Environment.UserDomainName.Equals("IFP1");
            if (!isIfpenDomaine)
            {
                Skip = $"Test skipped: requires IFPEN ressources";
            }
        }
    }
}
