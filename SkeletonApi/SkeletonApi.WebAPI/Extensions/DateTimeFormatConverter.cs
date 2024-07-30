using SkeletonApi.Application.Common.Exceptions;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkeletonApi.WebAPI.Extensions
{
    public class DateTimeFormatConverter : JsonConverter<DateTime>
    {
        private readonly string _format;

        public DateTimeFormatConverter(string format)
        {
            _format = format;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return DateTime.ParseExact(
                    reader.GetString(),
                    _format,
                    CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new GeneralBadRequest(ex.Message);
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));

            writer.WriteStringValue(value
                .ToString(
                _format,
                CultureInfo.InvariantCulture));
        }
    }
}
