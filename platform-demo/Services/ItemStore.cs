using platform_demo.Models;
using System.Collections.Concurrent;

namespace platform_demo.Services
{
    public class ItemStore : IItemStore
    {
        private readonly ConcurrentDictionary<int, Item> _items = new();
        private int _nextId;

        public IReadOnlyList<Item> GetAll() =>
            _items.Values.OrderBy(i => i.Id).ToList();

        public Item? GetById(int id) =>
            _items.TryGetValue(id, out var item) ? item : null;

        public Item Create(ItemDto dto)
        {
            var item = new Item
            {
                Id = Interlocked.Increment(ref _nextId),
                Name = dto.Name,
                Description = dto.Description
            };
            _items[item.Id] = item;
            return item;
        }

        public Item? Update(int id, ItemDto dto)
        {
            if (!_items.ContainsKey(id)) return null;

            var item = new Item
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description
            };
            _items[id] = item;
            return item;
        }

        public bool Delete(int id) => _items.TryRemove(id, out _);
    }
}
