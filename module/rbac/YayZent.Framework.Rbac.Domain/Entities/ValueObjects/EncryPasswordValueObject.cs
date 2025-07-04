using Volo.Abp.Domain.Values;

namespace YayZent.Framework.Rbac.Domain.Entities.ValueObjects;

public class EncryPasswordValueObject : ValueObject
{
    public string PasswordHash { get; init; } 
    public string Salt { get; init; }
    
    //SqlSugar 在映射 [SugarColumn(IsOwnsOne = true)] 值对象时，需要通过 无参构造函数 实例化这个值对象。
    
    public EncryPasswordValueObject() {}

    public EncryPasswordValueObject(string passwordHash, string salt)
    {
        PasswordHash = passwordHash;
        Salt = salt;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return PasswordHash;
        yield return Salt;
    }
}
