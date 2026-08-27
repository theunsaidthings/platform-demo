using Microsoft.AspNetCore.Mvc;
using platform_demo.Models;
using platform_demo.Services;

namespace platform_demo.Controllers;

[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    private readonly IItemStore _store;

    public ItemsController(IItemStore store)
    {
        _store = store;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Item>> GetAll() => Ok(_store.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<Item> GetById(int id)
    {
        var item = _store.GetById(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public ActionResult<Item> Create(ItemDto dto)
    {
        var item = _store.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Item> Update(int id, ItemDto dto)
    {
        var item = _store.Update(id, dto);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) => _store.Delete(id) ? NoContent() : NotFound();
}