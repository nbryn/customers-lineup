﻿using CLup.Application.Auth.Commands.Register;
using CLup.Domain.Shared.ValueObjects;
using CLup.Domain.Users.ValueObjects;

namespace CLup.API.Contracts.Auth;

public readonly record struct RegisterRequest(
    string Email,
    string Password,
    string Name,
    string Zip,
    string Street,
    string City,
    double Longitude,
    double Latitude)
{
    public RegisterCommand MapToCommand() =>
        new(new UserData(Email, Password, Name),
            new Address(Street, Zip, City),
            new Coords(Longitude, Latitude));
}