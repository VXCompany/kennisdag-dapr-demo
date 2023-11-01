namespace TicketMasterApi.Models;

public class Cart
{
    public List<Item> Items { get; set; } = default!;
}

public class Item
{
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
}