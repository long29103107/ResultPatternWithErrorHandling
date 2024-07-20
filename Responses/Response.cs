using Microsoft.AspNetCore.Mvc;
using Error = ResultPatternExample.Exceptions.Error;

namespace ResultPatternExample.Responses;

public class Response<T> where T : class
{
    
    public Response()
    {
        AddTypeResponse(this);
    }

    public Response(List<Error> errors, T result, int statusCode)
    {
        Errors = errors;
        StatusCode = statusCode;
        AddTypeResponse(this);

        if (!errors.Any())
        {
            Result = result ?? (T)Activator.CreateInstance(typeof(T));
        }
    }

    private void AddTypeResponse(Response<T> res)
    {
        if (res.Result != null && res.Result.GetType().IsGenericType && res.Result.GetType().GetGenericTypeDefinition() == typeof(List<>))
        {
            res.TypeResponse = (int)TypeResponseEnum.List;
        }
        else
        {
            res.TypeResponse = (int)TypeResponseEnum.Detail;
        }
    }

    public int TypeResponse { get; set; }

    public T Result { get; set; }

    public List<Error> Errors { get; } = new List<Error>();

    public int StatusCode { get;  } = StatusCodes.Status200OK;

    public bool IsSuccess
    {
        get
        {
            return !this.Errors.Any();
        }
    }

    public static Response<T> Success(T result)
    {
        return new(new List<Error>(), result, StatusCodes.Status200OK);
    }

    public static Response<T> Success(T result, int? statusCode = null)
    {
        return new(new List<Error>(), result, statusCode ?? StatusCodes.Status200OK);
    }

    public static Response<T> Failure(List<Error> errors, int statusCode)
    {
        return new(errors, default(T), statusCode);
    }

    public static Response<T> Failure(Error error, int statusCode)
    {
        return new(new List<Error>() { error }, default(T), statusCode);
    }
}


public class Response
{
    public Response(List<Error> errors,  int statusCode)
    {
        TypeResponse = (int)TypeResponseEnum.NoContent;
        Errors = errors;
        StatusCode = statusCode;
    }

    public int TypeResponse { get; }

    public List<Error> Errors { get; } = new List<Error>();

    public int StatusCode { get; } = StatusCodes.Status200OK;

    public bool IsSuccess
    {
        get
        {
            return !this.Errors.Any();
        }
    }

    public static Response Success(int? statusCode = null)
    {
        return new (new List<Error>(), statusCode ?? StatusCodes.Status200OK);
    }

    public static Response Failure(List<Error> errors, int statusCode)
    {
        return new(errors, statusCode);
    }

    public static Response Failure(Error error, int statusCode)
    {
        return new(new List<Error>() { error }, statusCode);
    }
}