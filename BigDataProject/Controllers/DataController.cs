using BigDataProject.Dtos;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BigDataProject.Controllers
{
	public class DataController : Controller
	{
		private readonly string connect = "Server = UNKNOWN\\SQLEXPRESS; initial Catalog = CARPLATES; integrated Security = True";
		public async Task<IActionResult> Index()
		{
            await using var conn = new SqlConnection(connect);

            //araba markaları 
            var brandMax = (await conn.QueryAsync<BrandDtos>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count DESC")).FirstOrDefault();
            var brandMin = (await conn.QueryAsync<BrandDtos>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count ASC")).FirstOrDefault();
            ViewData["brandMax"] = brandMax.BRAND;
            ViewData["countMaxBrand"] = brandMax.COUNT;
            ViewData["brandMin"] = brandMin.BRAND;
            ViewData["countMinBrand"] = brandMin.COUNT;
            //araba plakaları
            var plateMax = (await conn.QueryAsync<PlateDtos>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count DESC")).FirstOrDefault();
            var plateMin = (await conn.QueryAsync<PlateDtos>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count ASC")).FirstOrDefault();
            ViewData["plateMax"] = plateMax.PLATE;
            ViewData["countMaxPlate"] = plateMax.COUNT;
            ViewData["plateMin"] = plateMin.PLATE;
            ViewData["countMinPlate"] = plateMin.COUNT;
            //araba yakıt türleri
            var fuelType = (await conn.QueryAsync<FuelDtos>("SELECT TOP 1 FUEL, COUNT(*) AS count FROM PLATES GROUP BY FUEL ORDER BY count DESC")).FirstOrDefault();
            ViewData["fuelType"] = fuelType.FUEL;
            ViewData["fuelTypeCount"] = fuelType.COUNT;
            //araba renk çeşitlewri
            var colorType = (await conn.QueryAsync<ColorDtos>("SELECT TOP 1 COLOR, COUNT(*) AS count FROM PLATES GROUP BY COLOR ORDER BY count DESC")).FirstOrDefault();
            ViewData["colorType"] = colorType.COLOR;
            ViewData["colorTypeCount"] = colorType.COUNT;
            //arabaların yakıt türleri
            var shiftType = (await conn.QueryAsync<ShiftTypeDtos>("SELECT TOP 1 SHIFTTYPE, COUNT(*) AS count FROM PLATES GROUP BY SHIFTTYPE ORDER BY count DESC")).FirstOrDefault();
            ViewData["shiftType"] = shiftType.SHIFTTYPE;
            ViewData["shiftTypeCount"] = shiftType.COUNT;
            return View();
        }
        public async Task<IActionResult> Search(string keyword)
        {

            string query = @" SELECT TOP 100 BRAND, SUBSTRING(PLATE, 1, 2) AS COLOR, SHIFTTYPE, FUEL FROM PLATES WHERE BRAND LIKE '%' + @Keyword + '%' OR COLOR LIKE '%' + @Keyword + '%' OR SHIFTTYPE LIKE '%' + @Keyword + '%'   OR FUEL LIKE '%' + @Keyword + '%' ";
            await using var connection = new SqlConnection(connect);
            connection.Open();
            var searchResults = await connection.QueryAsync<SearchResult>(query, new { Keyword = keyword });
            return Json(searchResults);
        }
    }
}
