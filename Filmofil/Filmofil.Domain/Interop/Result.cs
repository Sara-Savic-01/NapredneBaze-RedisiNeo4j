using Filmofil.Domain.Errors;
using System.Collections.Generic;

namespace Filmofil.Domain.Interop
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public IList<Error> Errors { get; set; }
    }

    public class Result
    {
        public bool Success { get; set; }
        public IList<Error> Errors { get; set; }
    }
}
