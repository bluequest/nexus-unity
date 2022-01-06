using System;
using System.Runtime.Serialization;

namespace Nexus.Client
{
    /// <summary>
    /// A Nexus creator.
    /// </summary>
    [DataContract]
    [Serializable]
    public class NexusCreator
    {
        [DataMember] private string name = null;

        [DataMember] private string uuid = null;

        [DataMember] internal string nexusUrl = null;

        /// <summary>
        /// Creator's name.
        /// </summary>
        public string Name => this.name;

        /// <summary>
        /// Creator's Nexus URL. E.g. https://www.nexus.gg/player_unknown
        /// </summary>
        public string NexusURL { get; internal set; }

        /// <summary>
        /// Creator's Unique Id.
        /// </summary>
        public string UniqueId => this.uuid;

        public NexusCreator()
        {
        }

        /// <summary>
        /// For testing purposes.
        /// </summary>
        internal NexusCreator(string name, string uuid, string nexusUrl)
        {
            this.name = name;
            this.uuid = uuid;
            this.nexusUrl = nexusUrl;
        }
    }
}