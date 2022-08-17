using AutoMapper;
using EmptyAPI.Data;
using EmptyAPI.Data.Entities;
using EmptyAPI.DTOs;
using EmptyAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmptyAPI.Controllers
{
    public class ExcelDataController : BaseApiController
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<ExcelDataController> _logger;
        private readonly IMapper _mapper;
        public ExcelDataController(AppDbContext context, IConfiguration config, ILogger<ExcelDataController> logger, IMapper mapper)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetData([FromQuery] DataFilterDto dataFilter)
        {
            if ( dataFilter == null)
            {
                _logger.LogError("Bad Request");
                return BadRequest("Please send all main information for getting result data");
            }

            var query = _context.ExcelDatas.Where(d => d.Date >= dataFilter.StartDate && d.Date <= dataFilter.EndDate);

            var mergedList = new List<DataReturnDto>();

            switch (Enum.GetName(typeof(Filter), dataFilter.Filter))
            {
                case "Segment":
                    DataReturnDto dataReturnDto = _mapper.Map<DataReturnDto>(query);               
                    mergedList = query.GroupBy(x => x.Segment)
                                         .Select(g => new DataReturnDto
                                         {
                                             Type = g.Key,
                                             Discount = g.Sum(x => x.Discount),
                                             Profit = g.Sum(x => x.Profit),
                                             Sale = g.Sum(x => x.Sale),
                                             TotalCount = g.Count()
                                         })
                                         .ToList();
                    _logger.LogWarning("Data for Segment pulled");
                    break;
                case "Country":
                    mergedList = query.GroupBy(x => x.Country)
                                       .Select(g => new DataReturnDto
                                       {
                                           Type = g.Key,
                                           Discount = g.Sum(x => x.Discount),
                                           Profit = g.Sum(x => x.Profit),
                                           Sale = g.Sum(x => x.Sale),
                                           TotalCount = g.Count()
                                       })
                                         .ToList();
                    _logger.LogWarning("Data for country pulled");
                    break;
                case "Product":
                    mergedList = query.GroupBy(x => x.Product)
                         .Select(g => new DataReturnDto
                         {
                             Type = g.Key,
                             Discount = g.Sum(x => x.Discount),
                             Profit = g.Sum(x => x.Profit),
                             Sale = g.Sum(x => x.Sale),
                             TotalCount = g.Count()
                         })
                                         .ToList();
                    _logger.LogWarning("Data for Product pulled");
                    break;
                case "Discount":
                    _logger.LogWarning("Data for Discounts pulled");
                    break;
                default:
                    
                    break;
            }


            string excelName = $"Datas-{DateTime.Now.ToString("dd.MMMM.yyyy HH:mm:ss")}.xlsx";

            var path = @$"C:\Users\User\Desktop\Pyp\EmptyAPI\EmptyAPI\EmptyAPI\wwwroot\{excelName}";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            using (var package = new ExcelPackage())
            {

                var workSheet = package.Workbook.Worksheets.Add("Sheet1").Cells[1, 1].LoadFromCollection(mergedList, true);
                package.SaveAs(path);

                MemoryStream ms = new MemoryStream();

                using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    file.Close();

                    EmailService emailService = new EmailService(_config.GetSection("ConfirmationParams:Email").Value, _config.GetSection("ConfirmationParams:Password").Value);
                    emailService.SendEmail(dataFilter.AcceptorEmail, "Datas Report", "something will be happened here", bytes, excelName);
                }

            }

            _logger.LogInformation("Data pull Succeded !");
            return Ok("sended");
        }



        [HttpPost]
        public IActionResult AddData(IFormFile file)
        {
            if (file is null)
            {
                _logger.LogError("File is null");
                return BadRequest("File cant be null");
            }

            if (Path.GetExtension(file.FileName) != ".xls" && Path.GetExtension(file.FileName) != ".xlsx")
            {
                _logger.LogError("Wrong Format");
                return BadRequest("Wrong Format");
            }


            if (file.Length / Math.Pow(10, 6) > 5)
            {
                _logger.LogError("Oversize");
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
                _logger.LogInformation("Mounting data from excel to sql success");
                _context.SaveChanges();
                return Ok("Mounting datas from excel to database succedeed !");
            }
        }
    }
}
