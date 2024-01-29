﻿using CLup.Application.Users.Commands.UpdateUser;
using CLup.Domain.Shared.ValueObjects;
using CLup.Domain.Users.ValueObjects;

namespace CLup.API.Contracts.Users.UpdateUser;

public readonly record struct UpdateUserRequest(
    string Email,
    string Name,
    string Zip,
    string Street,
    string City,
    double Longitude,
    double Latitude)
{
    public UpdateUserCommand MapToCommand(UserId userId) =>
        new(userId,
            new UserData(Name, Email),
            new Address(Street, Zip, City),
            new Coords(Longitude, Latitude));
}
