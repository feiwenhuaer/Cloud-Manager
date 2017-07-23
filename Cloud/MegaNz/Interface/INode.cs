namespace Cloud.MegaNz
{
    using System;

    public interface INodePublic
    {
        long Size { get; }

        string Name { get; }
    }

    public interface INode : INodePublic, IEquatable<INode>
    {
        string Id { get; }

        string ParentId { get; }

        string Owner { get; }

        RootType Type { get; }

        DateTime LastModificationDate { get; }
    }
    public enum RootType
    {
        File = 0,
        Directory,
        Root,
        Inbox,
        Trash
    }
}