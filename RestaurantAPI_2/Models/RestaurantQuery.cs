namespace RestaurantAPI_2.Models
{
    /// <summary>
    /// Klasa reprezentująca zapytanie do wyszukiwania restauracji, zawierająca parametry takie jak fraza wyszukiwania, numer strony, rozmiar strony, sortowanie i kierunek sortowania.
    /// </summary>
    public class RestaurantQuery
    {
        public string? SearchPhrase { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }

        public SortDirection sortDirection { get; set; } = SortDirection.ASC;
    }
}
