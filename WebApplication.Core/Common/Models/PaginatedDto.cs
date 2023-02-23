using System.Collections;

namespace WebApplication.Core.Common.Models
{
    public class PaginatedDto<T> where T : IEnumerable
    {
        public T Data { get; set; } = default!;
        public bool HasNextPage { get; set; }
    }
}
