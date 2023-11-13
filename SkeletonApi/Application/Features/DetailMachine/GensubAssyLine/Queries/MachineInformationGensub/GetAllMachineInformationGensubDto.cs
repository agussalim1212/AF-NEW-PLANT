﻿using SkeletonApi.Application.Common.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.DetailMachine.GensubAssyLine.Queries.MachineInformation
{
    public class GetAllMachineInformationGensubDto : IMapFrom<GetAllMachineInformationGensubDto>
    {
        [JsonPropertyName("machine_name")]
        public string MachineName { get; set; }
        [JsonPropertyName("subject_name")]
        public string SubjectName { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("value_running")]
        public decimal ValueRunning { get; set; }
        [JsonPropertyName("value_cycle_count")]
        public decimal CycleCount { get; set; }
    }
}
