using Application.Dtos;
using Core.Entities;
namespace Application.Mappings;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName
        };
    }
}