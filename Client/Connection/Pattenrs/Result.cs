using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Connection.Pattenrs
{
    public class Result
    {
        public string? ErrorMessage;
        public bool IsSuccess => ErrorMessage == null;
        public bool IsFailed => ErrorMessage != null;
        protected Result()
        { 
        }
        protected Result(string error)
        {
            ErrorMessage = error;
        }
        public static Result Success() => new Result();
        public static Result Failure(string? error = null) => new Result(error is null ? string.Empty : error);
    }
    public class Result<T> : Result
    {
        public T? Value;
        protected Result(T value)
        {
            Value = value;
        }
        protected Result(string error)
        {
            ErrorMessage = error;
        }
        public static Result<T> Success(T value) => new Result<T>(value);
        public new static Result<T> Failure(string? error = null) => new Result<T>(error is null ? string.Empty : error);
    }
}
