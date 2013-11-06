using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp
{
    /// <summary>
    /// The collection of <see cref="RefSpec"/>s in a <see cref="Remote"/>
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class RefSpecCollection : IEnumerable<RefSpec>
    {
        private readonly Repository repository;
        private readonly Remote remote;

        /// <summary>
        /// Needed for mocking purposes.
        /// </summary>
        protected RefSpecCollection()
        { }

        internal RefSpecCollection(Remote remote)
        {
            this.repository = remote.repository;
            this.remote = remote;
        }

        /// <summary>
        /// Gets the <see cref="RefSpec"/> at the specified index
        /// </summary>
        /// <param name="index">The index of the refspec to retrieve.</param>
        /// <returns>The retrived <see cref="RefSpec"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">index is negative - or - index is larger than or equal <see cref="Count"/></exception>
        public virtual RefSpec this[int index]
        {
            get { return RefspecAtIndex(index); }
        }

        internal RefSpec RefspecAtIndex(int index)
        {
            EnsureValidIndex(index);

            using (RemoteSafeHandle remoteHandle = Proxy.git_remote_load(repository.Handle, remote.Name, true)) {
                using (GitRefSpecHandle handle = Proxy.git_remote_get_refspec(remoteHandle, index))
                {
                    return handle == null ? null : RefSpec.BuildFromPtr(handle);
                }
            }
        }

        /// <summary>
        /// Gets the count of refspecs defined for this remote
        /// </summary>
        public virtual int Count
        {
            get
            {
                using (RemoteSafeHandle remoteHandle = Proxy.git_remote_load(repository.Handle, remote.Name, true))
                {
                    return Proxy.git_remote_refspec_count(remoteHandle);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<RefSpec> GetEnumerator()
        {
            int count = Count;
            for (int i = 0; i < count; i++)
            {
                yield return RefspecAtIndex(i);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a fetch refspec.
        /// </summary>
        /// <param name="refSpec">The refspec to add.</param>
        public virtual void AddFetch(string refSpec)
        {
            Ensure.ArgumentNotNullOrEmptyString(refSpec, "refSpec");

            using (RemoteSafeHandle remoteHandle = Proxy.git_remote_load(repository.Handle, remote.Name, true))
            {
                Proxy.git_remote_add_fetch(remoteHandle, refSpec);
                Proxy.git_remote_save(remoteHandle);
            }
        }

        /// <summary>
        /// Adds a push refspec.
        /// </summary>
        /// <param name="refSpec">The refspec to add.</param>
        public virtual void AddPush(string refSpec)
        {
            Ensure.ArgumentNotNullOrEmptyString(refSpec, "refSpec");

            using (RemoteSafeHandle remoteHandle = Proxy.git_remote_load(repository.Handle, remote.Name, true))
            {
                Proxy.git_remote_add_push(remoteHandle, refSpec);
                Proxy.git_remote_save(remoteHandle);
            }
        }

        /// <summary>
        /// Deletes all refspecs.
        /// </summary>
        public virtual void Clear()
        {
            using (RemoteSafeHandle remoteHandle = Proxy.git_remote_load(repository.Handle, remote.Name, true))
            {
                Proxy.git_remote_clear_refspecsc(remoteHandle);
                Proxy.git_remote_save(remoteHandle);
            }
        }

        /// <summary>
        /// Deletes the refspec at a specific index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">index is negative - or - index is larger than or equal <see cref="Count"/></exception>
        public virtual void Remove(int index)
        {
            EnsureValidIndex(index);

            using (RemoteSafeHandle remoteHandle = Proxy.git_remote_load(repository.Handle, remote.Name, true))
            {
                Proxy.git_remote_remove_refspec(remoteHandle, index);
                Proxy.git_remote_save(remoteHandle);
            }
        }

        private void EnsureValidIndex(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "index must be a positive number");

            if (index > Count)
                throw new ArgumentOutOfRangeException("index", index, string.Format("index must be less than {0}", Count));
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "Count = {0}", this.Count);
            }
        }
    }
}
