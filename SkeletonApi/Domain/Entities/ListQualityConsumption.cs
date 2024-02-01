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

    public class EnergyConsumptionDetail
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public decimal FirstValue { get; set; }
        public decimal LastValue { get; set; }
    }

    public class EnergyConsumptionTop
    {
        public DateTime DayBucket { get; set; }
        public string Id { get; set; }
        public decimal FirstValue { get; set; }
        public decimal LastValue { get; set; }
        public decimal Value { get; set; }
    }
    public class AirConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class ProductConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string LastValue { get; set; }
    }

    public class EnergyConsumption
    {
        public DateTime DayBucket { get; set; }
        public string Id { get; set; }
        public decimal ValueFirst { get; set; }
        public decimal ValueLast { get; set; }
        public decimal Value { get; set; }

    }

    public class AirConsumptionDetail
    {
        public DateTime DayBucket { get; set; }
        public string Id { get; set; }
        public decimal ValueFirst { get; set; }
        public decimal ValueLast { get; set; }
        public decimal Value { get; set; }
    }

    public class AirConsumptionTop
    {
        public DateTime DayBucket { get; set; }
        public string Id { get; set; }
        public decimal FirstValue { get; set; }
        public decimal LastValue { get; set; }
        public decimal Value { get; set; }
    }

    public class ElectricConsumptionDetail
    {
        public DateTime DayBucket { get; set; }
        public string Id { get; set; }
        public decimal ValueFirst { get; set; }
        public decimal ValueLast { get; set; }
        public decimal Value { get; set; }
    }

    public class FrqConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }

}
