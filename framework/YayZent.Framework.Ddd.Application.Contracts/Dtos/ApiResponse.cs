namespace YayZent.Framework.Ddd.Application.Contracts.Dtos;

using System;
using System.Collections.Generic;

public class ApiResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int Code { get; set; } = 200;

    public static ApiResponse Ok(string message = "操作成功", int code = 200)
        => new ApiResponse
        {
            Success = true,
            Message = message,
            Code = code
        };

    public static ApiResponse Fail(string message = "操作失败", int code = 400)
        => new ApiResponse
        {
            Success = false,
            Message = message,
            Code = code
        };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    /// <summary>
    /// 扩展数据，用于返回额外字段
    /// </summary>
    public Dictionary<string, object>? Extras { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "操作成功", int code = 200)
        => new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Code = code,
            Data = data
        };

    public static ApiResponse<T> FailWithData(string message = "操作失败", int code = 400)
        => new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Code = code,
            Data = default
        };

    /// <summary>
    /// 添加一个额外字段
    /// </summary>
    public ApiResponse<T> WithExtra(string key, object value)
    {
        Extras ??= new Dictionary<string, object>();
        Extras[key] = value;
        return this;
    }

    /// <summary>
    /// 添加多个额外字段
    /// </summary>
    public ApiResponse<T> WithExtras(Dictionary<string, object> extraValues)
    {
        if (extraValues == null) return this;
        Extras ??= new Dictionary<string, object>();

        foreach (var kvp in extraValues)
        {
            Extras[kvp.Key] = kvp.Value;
        }

        return this;
    }
}
