namespace YayZent.Framework.Core.Interfaces;

public interface ITreeModel<T>: IOrderNum
{
    Guid Id { get; }
    
    Guid? ParentId { get; }
    
    List<T>? Children { get; }
}