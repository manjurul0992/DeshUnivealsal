using CrystalDecisions.CrystalReports.Engine;
using DeshUnivealsal.Models.DTOs;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DeshUnivealsal.Controllers
{
    public class CountriesController : Controller
    {
       
        private readonly CombineGiftEntities _context;

        public CountriesController()
        {
            _context = new CombineGiftEntities();
        }

        // GET: Countries
        public async Task<ActionResult> Index(string sortColumn = null,string sortOrder = null,string filterColumn = null,string filterQuery = null,int page = 1)
        {
            // Fetch data
            var query = _context.Countries.AsNoTracking().Select(c => new CountryDTO
            {
                Id = c.Id,
                Name = c.Name,
                ISO2 = c.ISO2,
                ISO3 = c.ISO3,
                TotCities = c.Cities.Count
            });

            // Filtering
            if (!string.IsNullOrEmpty(filterColumn) && !string.IsNullOrEmpty(filterQuery))
            {
                switch (filterColumn.ToLower())
                {
                    case "name":
                        query = query.Where(c => c.Name.Contains(filterQuery));
                        break;
                    case "iso2":
                        query = query.Where(c => c.ISO2.Contains(filterQuery));
                        break;
                    case "iso3":
                        query = query.Where(c => c.ISO3.Contains(filterQuery));
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
                    case "iso2":
                        query = sortOrder == "desc" ? query.OrderByDescending(c => c.ISO2) : query.OrderBy(c => c.ISO2);
                        break;
                    case "iso3":
                        query = sortOrder == "desc" ? query.OrderByDescending(c => c.ISO3) : query.OrderBy(c => c.ISO3);
                        break;
                    case "totcities":
                        query = sortOrder == "desc" ? query.OrderByDescending(c => c.TotCities) : query.OrderBy(c => c.TotCities);
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

            // Convert to list and apply pagination
            var countriesList = query.ToList();
            var pagedCountries = countriesList.ToPagedList(page, 8);

            return View(pagedCountries);
        }






        // GET: Countries/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch the country based on id
            var country = await _context.Countries
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CountryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ISO2 = c.ISO2,
                    ISO3 = c.ISO3,
                    TotCities = c.Cities.Count
                })
                .FirstOrDefaultAsync();

            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }


        // GET: Countries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,ISO2,ISO3")] CountryDTO countryDto)
        {
            if (ModelState.IsValid)
            {
                if (IsDupeField(0, "ISO2", countryDto.ISO2) || IsDupeField(0, "ISO3", countryDto.ISO3))
                {
                    ModelState.AddModelError("", "A country with the same ISO2 or ISO3 already exists.");
                    return View(countryDto);
                }

                var country = new Country
                {
                    Name = countryDto.Name,
                    ISO2 = countryDto.ISO2,
                    ISO3 = countryDto.ISO3
                };

                _context.Countries.Add(country);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(countryDto);
        }


        // GET: Countries/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch the country using the ID
            var country = await _context.Countries
                .Where(c => c.Id == id)
                .Select(c => new CountryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ISO2 = c.ISO2,
                    ISO3 = c.ISO3
                })
                .FirstOrDefaultAsync();

            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        // POST: Countries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CountryDTO countryDTO)
        {
            if (ModelState.IsValid)
            {
                var country = await _context.Countries.FindAsync(countryDTO.Id);
                if (country == null)
                {
                    return HttpNotFound();
                }

                if (IsDupeField(countryDTO.Id, "ISO2", countryDTO.ISO2) || IsDupeField(countryDTO.Id, "ISO3", countryDTO.ISO3))
                {
                    ModelState.AddModelError("", "A country with the same ISO2 or ISO3 already exists.");
                    return View(countryDTO);
                }

                country.Name = countryDTO.Name;
                country.ISO2 = countryDTO.ISO2;
                country.ISO3 = countryDTO.ISO3;

                _context.Entry(country).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(countryDTO);
        }


        // GET: Countries/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = await _context.Countries
                .Select(c => new CountryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    ISO2 = c.ISO2,
                    ISO3 = c.ISO3
                })
                .SingleOrDefaultAsync(c => c.Id == id);

            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }
        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return HttpNotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        [HttpPost]
        public bool IsDupeField(int countryId, string fieldName, string fieldValue)
        {
            // Ensure the property name exists in the Country entity
            var property = typeof(Country).GetProperty(fieldName);
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
            var parameter = Expression.Parameter(typeof(Country), "c");
            var member = Expression.Property(parameter, property.Name);
            var constant = Expression.Constant(convertedValue);
            var body = Expression.Equal(member, constant);

            // Add a condition to exclude the current country ID
            var idProperty = Expression.Property(parameter, "Id");
            var idConstant = Expression.Constant(countryId);
            var idCondition = Expression.NotEqual(idProperty, idConstant);

            // Combine conditions
            var combined = Expression.AndAlso(body, idCondition);
            var lambda = Expression.Lambda<Func<Country, bool>>(combined, parameter);

            // Execute the query
            return _context.Countries.Any(lambda);
        }



        public ActionResult Download_PDF()
        {
            CombineGiftEntities context = new CombineGiftEntities();
            var countries = context.Countries
        .Select(c => new CountryDTO
        {
            Id = c.Id,
            Name = c.Name,
            ISO2 = c.ISO2,
            ISO3 = c.ISO3,
            TotCities = c.Cities.Count()
        }).ToList();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "CrystalReport1.rpt"));
            rd.SetDataSource(countries);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "Countrycity.pdf");
        }

    }
}