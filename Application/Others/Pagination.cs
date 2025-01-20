namespace Application.Others
{
#pragma warning disable CS8618
    public class Pagination<T>
    {
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int TotalPages
        {
            get
            {
                var temp = TotalItems / PageSize;
                if (TotalItems % PageSize == 0)
                {
                    return temp;
                }
                return temp + 1;
            }
        }
        public int PageNumber { get; set; }

        /// <summary>
        /// page number start from 0
        /// </summary>
        public bool Next => PageNumber + 1 < TotalPages;
        public bool Previous => PageNumber > 0;
        public IEnumerable<T> Items { get; set; }
    }
}
