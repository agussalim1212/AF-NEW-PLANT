namespace SkeletonApi.IotHub.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int LifeTimeDays { get; set; }
    }
}