using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.DTOs.DetailMachine
{
    public class GetVidSubjectDto
    {
        public string Vid { get; set; }
        public string MachineName { get; set; }
        public string SubjectName { get; set; }
    }
}
