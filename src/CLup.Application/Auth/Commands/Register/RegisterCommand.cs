using CLup.Application.Shared;
using CLup.Domain.Shared.ValueObjects;
using CLup.Domain.Users;
using CLup.Domain.Users.Enums;
using CLup.Domain.Users.ValueObjects;

namespace CLup.Application.Auth.Commands.Register;

public sealed class RegisterCommand : IRequest<Result<string>>
{
    public UserData UserData { get; }

    public Address Address { get; }

    public RegisterCommand(UserData userData, Address address)
    {
        UserData = userData;
        Address = address;
    }

    public User MapToUser(string hashedPassword) =>
        new(new UserData(UserData.Name, UserData.Email, hashedPassword), Address, Role.User);
}
