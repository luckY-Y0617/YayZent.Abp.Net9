namespace YayZent.Framework.Ddd.Application.Contracts.Dtos;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Code { get; set; } = 200;

    public static ApiResponse Ok(string message = "操作成功", int code = 200)
        => new ApiResponse { Success = true, Message = message, Code = code };

    public static ApiResponse Fail(string message = "操作失败", int code = 400)
        => new ApiResponse { Success = false, Message = message, Code = code };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "操作成功", int code = 200)
        => new ApiResponse<T> { Success = true, Message = message, Code = code, Data = data };

    public static ApiResponse<T> FailWithData(string message = "操作失败", int code = 400)
        => new ApiResponse<T> { Success = false, Message = message, Code = code, Data = default };

}