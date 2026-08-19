using platform_demo.Models;

namespace platform_demo.Services
{
    public interface IItemStore
    {
        IReadOnlyList<Item> GetAll();
        Item? GetById(int id);
        Item Create(ItemDto dto);
        Item? Update(int id, ItemDto dto);
        bool Delete(int id);
    }
}
