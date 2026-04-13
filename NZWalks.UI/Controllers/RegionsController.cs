using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.DTO;
using System.Net.Http.Json;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = httpClientFactory.CreateClient("NZWalksApi");
            var regions = await client.GetFromJsonAsync<IEnumerable<RegionDto>>("api/regions");

            return View(regions?.ToList() ?? new List<RegionDto>());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddRegionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = httpClientFactory.CreateClient("NZWalksApi");
            var httpResponseMessage = await client.PostAsJsonAsync("api/regions", model);
            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (response is not null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = httpClientFactory.CreateClient("NZWalksApi");

            var response = await client.GetFromJsonAsync<RegionDto>($"api/regions/{id}");
            if (response is not null)
            {
                return View(response);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var client = httpClientFactory.CreateClient("NZWalksApi");
            var httpResponseMessage = await client.PutAsJsonAsync($"api/regions/{request.Id}", request);
            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (response is not null)
            {
                return RedirectToAction(nameof(Edit), new { id = response.Id });
            }

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto request)
        {
            var client = httpClientFactory.CreateClient("NZWalksApi");

            var httpResponseMessage = await client.DeleteAsync($"api/regions/{request.Id}");

            httpResponseMessage.EnsureSuccessStatusCode();

            return RedirectToAction(nameof(Index));
        }
    }
}
