﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Error = ResultPatternExample.Exceptions.Error;

namespace ResultPatternExample.Responses;

public class Response<T> : Response where T : class
{
    public Response() : base(new List<Error>(), StatusCodes.Status200OK)
    {
    }

    public Response(List<Error> errors, T result, int statusCode) : base(errors, StatusCodes.Status200OK)
    {
        if (!errors.Any())
        {
            Result = result ?? (T)Activator.CreateInstance(typeof(T));
        }
    }

    public T Result { get; set; }

    public static Response<T> Success(T result)
    {
        return new(new List<Error>(), result, StatusCodes.Status200OK);
    }

    public static Response<T> Success(T result, int? statusCode = null)
    {
        return new(new List<Error>(), result, statusCode ?? StatusCodes.Status200OK);
    }

    public static Response<T> Success(int? statusCode = null)
    {
        return new(new List<Error>(), default(T), statusCode ?? StatusCodes.Status200OK);
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
        Errors = errors;
        StatusCode = statusCode;
    }

    public List<Error> Errors { get; set; } = new List<Error>();

    public int StatusCode { get; set; } = StatusCodes.Status200OK;

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