using DeshUnivealsal.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.Linq.Expressions;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;


namespace DeshUnivealsal.Controllers
{
    public class CitiesController : Controller
    {
        private readonly CombineGiftEntities _context;

        public CitiesController()
        {
            _context = new CombineGiftEntities();
        }

        // GET: Cities
        public ActionResult Index(string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null, int page = 1)
        {
            // Fetch data
            var query = _context.Cities.AsNoTracking().Select(c => new CityDTO
            {
                Id = c.Id,
                Name = c.Name,
                Lat = c.Lat,
                Lon = c.Lon,
                CountryName = c.Country.Name
            });

            // Filtering
            if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery))
            {
                switch (filterColumn.ToLower())
                {
                    case "name":
                        query = query.Where(c => c.Name.Contains(filterQuery));
                        break;
                    case "country":
                        query = query.Where(c => c.CountryName.Contains(filterQuery));
                        break;
                }
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                switch (sortColumn.ToLower())
                {
                    case "name":
                        query = sortOrder == "desc" ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
                        break;
                    case "country":
                        query = sortOrder == "desc" ? query.OrderByDescending(c => c.CountryName) : query.OrderBy(c => c.CountryName);
                        break;
                    default:
                        query = query.OrderBy(c => c.Name); // Default sorting
                        break;
                }
            }
            else
            {
                // Default sorting if no sort column is provided
                query = query.OrderBy(c => c.Name);
            }

            // Apply pagination at the query level
            var pagedCities = query.ToPagedList(page, 8);

            return View(pagedCities);
        }





        // GET: Cities/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var city = await _context.Cities
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CityDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Lat = c.Lat,
                    Lon = c.Lon,
                    CountryId = c.Country.Id,
                    CountryName = c.Country.Name
                })
                .FirstOrDefaultAsync();

            if (city == null)
            {
                return HttpNotFound();
            }

            return View(city);
        }


        // GET: Cities/Create
       
        public ActionResult Create()
        {
            // Load countries for the dropdown list
            ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name");
            return View();
        }


        // POST: Cities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Lat,Lon,CountryId")] CityDTO cityDto)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate city name
                if (IsDupeCity(0, "Name", cityDto.Name))
                {
                    ModelState.AddModelError("Name", "A city with this name already exists.");
                }
                else
                {
                    var city = new City
                    {
                        Name = cityDto.Name,
                        Lat = cityDto.Lat,
                        Lon = cityDto.Lon,
                        CountryId = cityDto.CountryId
                    };

                    _context.Cities.Add(city);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }

            // Reload countries in case of validation errors
            ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name", cityDto.CountryId);
            return View(cityDto);
        }





        // GET: Cities/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var city = await _context.Cities
                .Where(c => c.Id == id)
                .Select(c => new CityDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Lat = c.Lat,
                    Lon = c.Lon,
                    CountryId = c.Country.Id
                })
                .FirstOrDefaultAsync();

            if (city == null)
            {
                return HttpNotFound();
            }

            ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
            return View(city);
        }

        // POST: Cities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CityDTO cityDTO)
        {
            if (ModelState.IsValid)
            {
                var city = await _context.Cities.FindAsync(cityDTO.Id);
                if (city == null)
                {
                    return HttpNotFound();
                }

                // Check for duplicate city name, excluding the current city ID
                if (IsDupeCity(cityDTO.Id, "Name", cityDTO.Name))
                {
                    ModelState.AddModelError("Name", "A city with this name already exists.");
                }
                else
                {
                    city.Name = cityDTO.Name;
                    city.Lat = cityDTO.Lat;
                    city.Lon = cityDTO.Lon;
                    city.CountryId = cityDTO.CountryId;

                    _context.Entry(city).State = System.Data.Entity.EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }

            // Reload countries in case of validation errors
            ViewBag.Countries = new SelectList(_context.Countries, "Id", "Name", cityDTO.CountryId);
            return View(cityDTO);
        }



        // GET: Cities/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var city = await _context.Cities
                .Select(c => new CityDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Lat = c.Lat,
                    Lon = c.Lon,
                    CountryId = c.Country.Id,
                    CountryName = c.Country.Name
                })
                .SingleOrDefaultAsync(c => c.Id == id);

            if (city == null)
            {
                return HttpNotFound();
            }

            return View(city);
        }


        // POST: Cities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public bool IsDupeCity(int cityId, string fieldName, string fieldValue)
        {
            // Ensure the property name exists in the City entity
            var property = typeof(City).GetProperty(fieldName);
            if (property == null)
            {
                return false; // Invalid property name
            }

            // Convert fieldValue to the appropriate type
            object convertedValue;
            try
            {
                convertedValue = Convert.ChangeType(fieldValue, property.PropertyType);
            }
            catch
            {
                return false; // Conversion failed
            }

            // Use LINQ to dynamically build the query
            var parameter = Expression.Parameter(typeof(City), "c");
            var member = Expression.Property(parameter, property.Name);
            var constant = Expression.Constant(convertedValue);
            var body = Expression.Equal(member, constant);

            // Add a condition to exclude the current city ID
            var idProperty = Expression.Property(parameter, "Id");
            var idConstant = Expression.Constant(cityId);
            var idCondition = Expression.NotEqual(idProperty, idConstant);

            // Combine conditions
            var combined = Expression.AndAlso(body, idCondition);
            var lambda = Expression.Lambda<Func<City, bool>>(combined, parameter);

            // Execute the query
            return _context.Cities.Any(lambda);
        }

        public ActionResult Download_PDF()
        {
            CombineGiftEntities context = new CombineGiftEntities();
            var query = _context.Cities.AsNoTracking().Select(c => new CityDTO
            {
                Id = c.Id,
                Name = c.Name,
                Lat = c.Lat,
                Lon = c.Lon,
                CountryName = c.Country.Name
            });
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CrystalReport2.rpt"));
            rd.SetDataSource(query);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "CityCountry.pdf");
        }

    }

}