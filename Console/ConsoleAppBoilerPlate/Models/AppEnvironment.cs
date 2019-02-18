using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppBoilerPlate.Models
{
    public enum AppEnvironment
    {
        /// <summary>
        /// Local
        /// </summary>
        Local,
        /// <summary>
        /// Development
        /// </summary>
        Dev,
        /// <summary>
        /// UAT
        /// </summary>
        Uat,
        /// <summary>
        /// SIT
        /// </summary>
        Sit,
        /// <summary>
        /// Production
        /// </summary>
        Prod
    }
}
