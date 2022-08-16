using EmptyAPI.Data;
using EmptyAPI.Data.Entities;
using EmptyAPI.DTOs;
using EmptyAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;
        public ExcelDataController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        [HttpGet]
        public IActionResult GetData([FromQuery] Filter filter, [FromQuery] DataFilterDto dataFilter)
        {
            if (filter == 0 || dataFilter == null)
            {
                return BadRequest("Main demands still remain !");
            }   

            string type = Enum.GetName(typeof(Filter), filter);

            DateTime startDate = dataFilter.StartDate;

            DateTime endDate = dataFilter.EndDate;

            string email = dataFilter.AcceptorEmail;

            var query = _context.ExcelDatas.Where(d => d.Segment == type && d.Date > startDate && d.Date < endDate);

            var datas = query.ToList();

            string excelName = $"Datas-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            var path = @$"C:\Users\User\Desktop\Pyp\EmptyAPI\EmptyAPI\EmptyAPI\wwwroot\{excelName}";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            using (var package = new ExcelPackage())
            {

                var workSheet = package.Workbook.Worksheets.Add("Sheet1").Cells[1,1].LoadFromCollection(datas,true);
                package.SaveAs(path);

                EmailService emailService = new EmailService(_config.GetSection("ConfirmationParams:Email").Value, _config.GetSection("ConfirmationParams:Password").Value);
                emailService.SendEmail(email, "Datas Report", "something will be happened here", path, excelName);
                
            }

          
            return Ok("sended");
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
