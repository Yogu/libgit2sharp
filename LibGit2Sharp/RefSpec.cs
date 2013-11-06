using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGit2Sharp
{
    /// <summary>
    /// A push or fetch reference specification
    /// </summary>
    public class RefSpec
    {
        private RefSpec(string refSpec, RefSpecDirection direction, string source, string destination, bool forceUpdate)
        {
            Specification = refSpec;
            Direction = direction;
            Source = source;
            Destination = destination;
            ForceUpdate = forceUpdate;
        }

        /// <summary>
        /// Needed for mocking purposes.
        /// </summary>
        protected RefSpec()
        {

        }

        internal static RefSpec BuildFromPtr(GitRefSpecHandle handle)
        {
            return new RefSpec(Proxy.git_refspec_string(handle), Proxy.git_refspec_direction(handle),
                Proxy.git_refspec_src(handle), Proxy.git_refspec_dst(handle), Proxy.git_refspec_force(handle));
        }

        /// <summary>
        /// Gets the git refspec specification that is used in <see cref="RefSpecCollection.AddFetch"/> and 
        /// <see cref="RefSpecCollection.AddPush"/>
        /// </summary>
        public virtual string Specification{get; private set;}

        /// <summary>
        /// Indicates whether this is a Push or Pull refspec
        /// </summary>
        public virtual RefSpecDirection Direction { get; private set; }

        /// <summary>
        /// The source reference specifier
        /// </summary>
        public virtual string Source { get; private set; }

        /// <summary>
        /// The target reference specifier
        /// </summary>
        public virtual string Destination { get; private set; }

        /// <summary>
        /// Indicates whether the destination will be force-updated if fast-forwarding is not possible
        /// </summary>
        public virtual bool ForceUpdate { get; private set; }
    }
}
