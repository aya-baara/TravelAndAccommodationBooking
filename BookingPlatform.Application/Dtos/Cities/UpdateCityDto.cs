﻿namespace BookingPlatform.Application.Dtos.Cities;

public class UpdateCityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string PostOffice { get; set; }
    public string Description { get; set; }
}

