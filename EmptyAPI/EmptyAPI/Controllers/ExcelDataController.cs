using EmptyAPI.Data;
using EmptyAPI.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmptyAPI.Controllers
{
    public class ExcelDataController : BaseApiController
    {
        private readonly AppDbContext _context;

        public ExcelDataController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetData()
        {

            return Ok("grdin");
        }

        [HttpPost]
        public IActionResult AddData(IFormFile file)
        {
            if (file is null)
            {
                return BadRequest("File cant be null");
            }

            if (Path.GetExtension(file.FileName) != ".xls" && Path.GetExtension(file.FileName) != ".xlsx" )
            {
                return BadRequest("Wrong Format");
            }


            if (file.Length / Math.Pow(10,6) > 5)
            {
                return BadRequest("Oversize");
            }


            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                    var rowcount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowcount; row++)
                    {
                        ExcelData dataFromExcel = new ExcelData();

                        dataFromExcel.Segment = worksheet.Cells[row, 1].Value.ToString()?.Trim();
                        dataFromExcel.Country = worksheet.Cells[row, 2].Value.ToString()?.Trim();
                        dataFromExcel.Product = worksheet.Cells[row, 3].Value.ToString()?.Trim();
                        dataFromExcel.DiscountBand = worksheet.Cells[row, 4].Value.ToString().Trim();
                        dataFromExcel.UnitsSold = double.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
                        dataFromExcel.ManufacturingPrice = double.Parse(worksheet.Cells[row, 6].Value.ToString().Trim());
                        dataFromExcel.SellPrice = double.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());
                        dataFromExcel.GrossSales = double.Parse(worksheet.Cells[row, 8].Value.ToString().Trim());
                        dataFromExcel.Discount = double.Parse(worksheet.Cells[row, 9].Value.ToString().Trim());
                        dataFromExcel.Sale = double.Parse(worksheet.Cells[row, 10].Value.ToString().Trim());
                        dataFromExcel.COGS = double.Parse(worksheet.Cells[row, 11].Value.ToString().Trim());
                        dataFromExcel.Profit = double.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());
                        dataFromExcel.Date = DateTime.Parse(worksheet.Cells[row, 13].Value.ToString().Trim());

                        _context.ExcelDatas.Add(dataFromExcel);
                    }
                }
                _context.SaveChanges();
                return Ok("Mounting datas from excel to database succedeed !");
            }
        }
    }
}
