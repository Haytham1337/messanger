using Application.Models.MessageDto;
using Application.Models.PhotoDto;
using Application.Models.UserDto;
using Application.Models.UserDto.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<GetUserDto> GetUserInfoAsync(GetUserInfoRequest request);

        Task UpdateUserAsync(UpdateUserDto model);

        Task<List<SearchUserDto>> SearchUserAsync(SearchUserDtoRequest request);

        Task BlockUserAsync(BlockUserRequest request);

        Task UnBlockUserAsync(BlockUserRequest request);

        Task<bool> CheckStatusAsync(AddMessageDto request);

        Task ChangePhotoAsync(AddPhotoDto model);
    }
}
