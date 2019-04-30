using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weather.Domain;

namespace Weather.Data.Mapping
{
    public class WeatherDataMap : IEntityTypeConfiguration<WeatherData>
    {
        public void Configure(EntityTypeBuilder<WeatherData> builder)
        {
            builder.ToTable(nameof(WeatherData));
            builder.HasKey(w => w.Id);

            builder.Property(w => w.City).HasMaxLength(100).IsRequired();
            builder.Property(w => w.Date).IsRequired();
            builder.Property(w => w.WeatherType).HasMaxLength(100).IsRequired();
            builder.Property(w => w.DayTemperature).HasMaxLength(100);
            builder.Property(w => w.NightTemperature).HasMaxLength(100).IsRequired();
            builder.Property(w => w.WindType).HasMaxLength(100).IsRequired();
            builder.Property(w => w.WindSpeed).HasMaxLength(100).IsRequired();

            //builder.Ignore(w => w.WeatherStatusType);

        }
    }
}
