using Ganss.Xss;
using Markdig;

namespace YayZent.Framework.Core.Rendering.Markdown;

public class MarkdownRenderService: IMarkdownRenderService
{
    private readonly MarkdownPipeline _pipeline;
    private readonly HtmlSanitizer _sanitizer;

    public MarkdownRenderService()
    {
        // 配置 Markdown 支持扩展：如表格、任务列表、管道表、自动链接等
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UsePipeTables()
            .UseAutoLinks()
            .UseTaskLists()
            .Build();

        // 创建 HtmlSanitizer 实例，默认会去除危险标签和属性
        _sanitizer = new HtmlSanitizer();
    }

    /// <summary>
    /// 将 Markdown 转换为 HTML，并自动进行 XSS 安全清理
    /// </summary>
    /// <param name="markdown">Markdown 文本</param>
    /// <returns>安全的 HTML</returns>
    public string ToHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var html = Markdig.Markdown.ToHtml(markdown, _pipeline);
        return _sanitizer.Sanitize(html);
    }
}