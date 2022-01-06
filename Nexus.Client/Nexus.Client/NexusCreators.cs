using System;
using System.Runtime.Serialization;

namespace Nexus.Client
{
    /// <summary>
    /// Collection of <see cref="NexusCreators"/>.
    /// </summary>
    [DataContract]
    [Serializable]
    public class NexusCreators
    {
        [DataMember] private NexusCreator[] creators = new NexusCreator[0];

        /// <summary>
        /// List of creators from Nexus.
        /// </summary>
        public NexusCreator[] Creators => this.creators;

        public NexusCreators()
        {
        }

        /// <summary>
        /// For testing purposes.
        /// </summary>
        internal NexusCreators(NexusCreator[] creators)
        {
            this.creators = creators;
        }
    }
}