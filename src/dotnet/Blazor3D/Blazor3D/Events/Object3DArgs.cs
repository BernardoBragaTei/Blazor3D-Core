﻿namespace HomagGroup.Blazor3D.Events
{
    internal class Object3DStaticArgs
    {
        public string ContainerId { get; set; } = null!;

        public Guid UUID { get; set; }
    }

    public class ContainerStaticArgs
    {
        public string ContainerId { get; set; } = null!;
    }

    /// <summary>
    /// <para>Arguments for <see cref="Viewers.Viewer"/> ObjectSelected and ObjectLoaded event handlers.</para>
    /// </summary>
    public class Object3DArgs
    {
        /// <summary>
        /// Selected or Loaded object unique identifier.
        /// </summary>
        public Guid UUID { get; set; }
    }
}
