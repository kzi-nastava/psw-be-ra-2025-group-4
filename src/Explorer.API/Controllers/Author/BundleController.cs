using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/bundles")]
[ApiController]
public class BundleController : ControllerBase
{
    private readonly IBundleService _bundleService;

    public BundleController(IBundleService bundleService)
    {
        _bundleService = bundleService;
    }

    private int GetAuthorId()
    {
        var id = User.FindFirst("id")?.Value;

        if (id != null) return int.Parse(id);

        var pid = User.FindFirst("personId")?.Value;

        return int.Parse(pid ?? throw new Exception("No user id found"));
    }

    [HttpGet]
    public ActionResult<PagedResult<BundleDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_bundleService.GetPagedByAuthor(GetAuthorId(), page, pageSize));
    }

    [HttpGet("{id:int}")]
    public ActionResult<BundleDto> GetById(int id)
    {
        var bundle = _bundleService.GetByIdForAuthor(GetAuthorId(), id);
        return Ok(bundle);
    }

    [HttpPost]
    public ActionResult<BundleDto> Create([FromBody] CreateBundleDto dto)
    {
        var created = _bundleService.Create(dto, GetAuthorId());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public ActionResult<BundleDto> Update(int id, [FromBody] UpdateBundleDto dto)
    {
        var updated = _bundleService.Update(id, dto, GetAuthorId());
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        _bundleService.Delete(id, GetAuthorId());
        return NoContent();
    }

    [HttpPut("{id:int}/publish")]
    public ActionResult Publish(int id)
    {
        _bundleService.Publish(id, GetAuthorId());
        return NoContent();
    }

    [HttpPut("{id:int}/archive")]
    public ActionResult Archive(int id)
    {
        _bundleService.Archive(id, GetAuthorId());
        return NoContent();
    }
}

