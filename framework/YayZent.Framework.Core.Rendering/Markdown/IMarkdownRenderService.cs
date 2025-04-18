using Volo.Abp.DependencyInjection;

namespace YayZent.Framework.Core.Rendering.Markdown;

public interface IMarkdownRenderService: ITransientDependency
{
    string ToHtml(string markdown);
}