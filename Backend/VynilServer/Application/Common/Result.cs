namespace VynilServer.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T Value { get; set; }
    public string Error { get; set; }
    
    private  Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    
    public static Result<T> Success(T value)      => new(true,  value, string.Empty);
    public static Result<T> Failure(string error) => new(false, default, error);
}