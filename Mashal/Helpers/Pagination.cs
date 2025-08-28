namespace lawyer.Api.Helpers
{
    public class Pagination<T>
    {
        public Pagination(int pageSize, int pageIndex, IReadOnlyList<T> Data, int count)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            Count = count;
            data = Data;
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> data { get; set; }

    }
}
