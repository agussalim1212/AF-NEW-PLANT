﻿using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.TotalProduction
{
    public class GetAllTotalProductionGensubDto : IMapFrom<GetAllTotalProductionGensubDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }
        [JsonPropertyName("value_ok_total")]
        public decimal ValueOkTotal { get; set; }
        [JsonPropertyName("value_ng_total")]
        public decimal ValueNgTotal { get; set; }
        [JsonPropertyName("value_ok_persentase")]
        public decimal ValueOKPresentase { get; set; }
        [JsonPropertyName("value_ng_persentase")]
        public decimal ValueNgPresentase { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

    }
}