// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Item.cs" company="Software Inc.">
//   A.Robson
// </copyright>
// <summary>
//   Defines the Item type.
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
    /// The item.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Gets or sets the module id.
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        /// Gets or sets the module.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Gets or sets the item number.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        public string Name { get; set; }
    }
}
