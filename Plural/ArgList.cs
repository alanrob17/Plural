// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgList.cs" company="Software Inc.">
//   A.Robson
// </copyright>
// <summary>
//   The argument list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Plural
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The argument list.
    /// </summary>
    public class ArgList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgList"/> class.
        /// </summary>
        /// <param name="fileName">The Pluralsight file name.</param>
        public ArgList(string fileName)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Gets or sets a value for the file name.
        /// </summary>
        public string FileName { get; set; }

    }
}
