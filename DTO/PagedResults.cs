namespace NetflixClone.DTO;
using NetflixClone.Models;

public class PagedResults<T> {
    public List<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
