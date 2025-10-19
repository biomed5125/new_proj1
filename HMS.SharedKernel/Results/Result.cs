
namespace HMS.SharedKernel.Results
{
    public class Result
    {
        public bool Succeeded { get; }
        public string[] Errors { get; }

        protected Result(bool ok, IEnumerable<string>? errors = null)
        {
            Succeeded = ok;
            Errors = errors?.ToArray() ?? Array.Empty<string>();
        }

        public static Result Success() => new(true);
        public static Result Fail(params string[] errors) => new(false, errors);
    }

    public sealed class Result<T> : Result
    {
        public T? Value { get; }
        private Result(bool ok, T? value, IEnumerable<string>? errors = null)
            : base(ok, errors) => Value = value;

        public static Result<T> Success(T value) => new(true, value);
        public static new Result<T> Fail(params string[] errors) => new(false, default, errors);
    }


}
