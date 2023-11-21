using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Domain.Entities
{
    public class ListQualityConsumption
    { 
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class RobotConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class MainLineConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class CoolantFilingConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class OilBrakeConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class NumberingConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class PressConeRaceConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class WheelFrontConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
    public class WheelRearConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
