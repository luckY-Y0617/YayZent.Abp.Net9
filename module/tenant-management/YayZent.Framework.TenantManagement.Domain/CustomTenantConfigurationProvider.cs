using Microsoft.Extensions.Localization;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.Localization;

namespace YayZent.Framework.TenantManagement.Domain;

/// <summary>
/// 自定义租户配置提供程序，负责解析并获取当前租户的配置。
/// </summary>
[Dependency(ReplaceServices = true)] // 注入替换服务
public class CustomTenantConfigurationProvider(ITenantResolver tenantResolver, ITenantStore tenantStore, 
    ITenantResolveResultAccessor tenantResolveResultAccessor, IStringLocalizer<AbpMultiTenancyResource> localizer): ITenantConfigurationProvider, ITransientDependency
{
        private readonly ITenantResolver _tenantResolver = tenantResolver;
        private readonly ITenantStore _tenantStore = tenantStore;
        private readonly ITenantResolveResultAccessor _tenantResolveResultAccessor = tenantResolveResultAccessor;
        private readonly IStringLocalizer<AbpMultiTenancyResource> _localizer = localizer;

        /// <summary>
        /// 获取当前租户的配置。 
        /// </summary>
        /// <param name="saveResolveResult">是否保存租户解析结果</param>
        /// <returns>返回租户配置</returns>
        public async Task<TenantConfiguration?> GetAsync(bool saveResolveResult = false)
        {
            var resolveResult = await _tenantResolver.ResolveTenantIdOrNameAsync();
            
            if (saveResolveResult)
            {
                _tenantResolveResultAccessor.Result = resolveResult;
            }

            if (string.IsNullOrEmpty(resolveResult.TenantIdOrName))
            {
                return null;
            }

            // 查找租户配置
            var tenant = await FindTenantAsync(resolveResult.TenantIdOrName);

            // 如果未找到租户，抛出异常
            if (tenant == null)
            {
                throw new BusinessException(
                    "Volo.AbpIo.MultiTenancy:010001",
                    _localizer["TenantNotFoundMessage"],
                    _localizer["TenantNotFoundDetails", resolveResult.TenantIdOrName]
                );
            }

            // 如果租户不可用（已禁用），抛出异常
            if (!tenant.IsActive)
            {
                throw new BusinessException(
                    "Volo.AbpIo.MultiTenancy:010002",
                    _localizer["TenantNotActiveMessage"],
                    _localizer["TenantNotActiveDetails", resolveResult.TenantIdOrName]
                );
            }

            // 返回找到的租户配置
            return tenant;
        }

        /// <summary>
        /// 根据租户ID或名称查找租户配置。
        /// </summary>
        /// <param name="tenantIdOrName">租户的ID或名称</param>
        /// <returns>租户配置或null</returns>
        private async Task<TenantConfiguration?> FindTenantAsync(string tenantIdOrName)
        {
            // 如果输入的租户ID或名称是GUID格式，则按ID查找
            if (Guid.TryParse(tenantIdOrName, out var tenantId))
            {
                return await _tenantStore.FindAsync(tenantId);
            }

            // 否则按名称查找
            return await _tenantStore.FindAsync(tenantIdOrName);
        }
    
}