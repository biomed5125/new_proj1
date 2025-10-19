using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HMS.Api.Pages.Lab.Requests;

public sealed class EditModel : PageModel
{
    public long Id { get; private set; }
    public void OnGet(long id) => Id = id;
}
