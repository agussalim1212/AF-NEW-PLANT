namespace SkeletonApi.Application.DTOs.Consumption
{
    public class Consumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class MachineInformationConsumption
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public DateTime DateTime { get; set; }
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
        public decimal Value { get; set; }
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
        public double FirstValue { get; set; }
        public double LastValue { get; set; }
        public double Value { get; set; }
    }

    public class AirConsumption
    {
        public string Id { get; set; }
        public DateTime Bucket { get; set; }
        public decimal ValueFirst { get; set; }
        public decimal ValueLast { get; set; }
    }

    public class ProductConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string LastValue { get; set; }
    }

    public class EnergyConsumption
    {
        public string Id { get; set; }
        public DateTime Bucket { get; set; }
        public decimal ValueFirst { get; set; }
        public decimal ValueLast { get; set; }
    }

    public class AirConsumptionDetail
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public decimal ValueFirst { get; set; }
        public decimal ValueLast { get; set; }
        public decimal Value { get; set; }
    }

    public class CurrentConsumptions
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public decimal LastValue { get; set; }
    }

    public class FrqConsumption
    {
        public DateTime Bucket { get; set; }
        public string Id { get; set; }
        public string Value { get; set; }
    }
}