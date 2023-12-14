﻿using AutoMapper;
using ClosedXML.Excel;
using MediatR;
using Microsoft.AspNetCore.Http;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.Create;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UploadExcel
{
    public class UploadExcelMaintPrevCommand : IRequest<Result<List<UploadExcelMaintPrevDto>>>
    {
        public IFormFile formFile { get; set; }

        public UploadExcelMaintPrevCommand(IFormFile file)
        {
            formFile = file;
        }
    }
    internal class UploadExcelMaintPrevCommandHandler : IRequestHandler<UploadExcelMaintPrevCommand, Result<List<UploadExcelMaintPrevDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UploadExcelMaintPrevCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<UploadExcelMaintPrevDto>>> Handle(UploadExcelMaintPrevCommand command, CancellationToken cancellationToken)
        {
            var excelData = new UploadExcelMaintPrevDto();
            var data = new List<UploadExcelMaintPrevDto>();
            using (var stream = new MemoryStream())
            {
                await command.formFile.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed();
                    Console.WriteLine(rows.Count());
                    if (rows.Count() < 2)
                    {
                        return Result<List<UploadExcelMaintPrevDto>>.Failure("Data Not Found");
                    }

                    foreach (var row in rows.Skip(1))
                    {
                        excelData = new UploadExcelMaintPrevDto()
                        {
                            Id = Guid.NewGuid(),
                            Name = row.Cell(1).Value?.ToString(),
                            Plan = row.Cell(2).Value?.ToString(),
                            StartDate = DateOnly.TryParse(row.Cell(3).Value?.ToString(), out DateOnly start_Date) ? start_Date : start_Date,
                            Actual = row.Cell(4).Value?.ToString(),
                            EndDate = DateOnly.TryParse(row.Cell(5).Value?.ToString(), out DateOnly end_Date) ? end_Date : null,
                        };
                        data.Add(excelData);
                    }
                    Console.WriteLine(data);
                }
            }
                return Result<List<UploadExcelMaintPrevDto>>.Success("");
        }
    }
}
