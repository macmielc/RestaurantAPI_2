namespace RestaurantAPI_2.Models
{
    public class PageResult<T>
    {
        public PageResult(List<T> items, int totaItemslCount, int pageSize, int pageNumber)
        {
            Items = items;
            TotalItemsCount = totaItemslCount;

            ItemsFrom = pageSize * (pageNumber - 1) + 1;
            ItemsTo = ItemsFrom + pageSize - 1;
            TotalPages = (int)Math.Ceiling( totaItemslCount / (double)pageSize);
        }

        public List<T> Items { get; set; }

        public int TotalPages { get; set; }

        public int ItemsFrom { get; set; }

        public int ItemsTo { get; set; }

        public int TotalItemsCount { get; set; }
    }
}
