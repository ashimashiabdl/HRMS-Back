using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Data
{
    public class TreeNode
    {
        public long? Id { get; set; }
        public long? ParentId { get; set; }
        public TreeNode? Parent { get; set; }
        public string? Title { get; set; }
        public bool HasAccess { get; set; }
        public bool HasInheritedAccess { get; set; }
        /// <summary>
        /// True when at least one OrganisationPosition has RelatedNodeId equal to this node.
        /// </summary>
        public bool HasOrganisationPosition { get; set; }
        public List<TreeNode>? Children { get; set; }
    }
}
