using AutoMapper;
using BLL.Abstract;
using CORE.Helpers;
using CORE.Localization;
using DAL.EntityFramework.Abstract;
using DAL.EntityFramework.UnitOfWork;
using DAL.EntityFramework.Utility;
using DTO;
using DTO.Responses;
using DTO.User;
using ENTITIES.Entities;

namespace BLL.Concrete;

public class UserService(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    ITokenRepository tokenRepository) : IUserService
{
    public async Task<IResult> AddAsync(UserCreateRequestDto dto)
    {
        if (await userRepository.IsUserExistAsync(dto.Email, null))
        {
            return new ErrorResult(EMessages.UserIsExist.Translate());
        }

        var data = mapper.Map<User>(dto);

        data.Salt = SecurityHelper.GenerateSalt();
        data.Password = SecurityHelper.HashPassword(data.Password, data.Salt);

        await userRepository.AddAsync(data);

        await unitOfWork.CommitAsync();

        return new SuccessResult(EMessages.Success.Translate());
    }

    public async Task<IResult> SoftDeleteAsync(Guid id)
    {
        var data = await userRepository.GetAsync(id);

        userRepository.SoftDelete(data);
        await tokenRepository.RemoveUserActiveTokensAsync(id);

        await unitOfWork.CommitAsync();

        return new SuccessResult(EMessages.Success.Translate());
    }

    public async Task<IResult?> SoftDeleteMultipleAsync(IList<Guid> ids)
    {
        await userRepository.SoftDeleteMultipleAsync(ids);
        await tokenRepository.RemoveUsersActiveTokensAsync(ids);

        await unitOfWork.CommitAsync();

        return new SuccessResult(EMessages.Success.Translate());
    }

    public async Task<IResult> DuplicateAsync(Guid userId)
    {
        var data = await userRepository.SingleOrDefaultAsync(u => u.Id == userId);
        if (data == null) { return new ErrorResult(EMessages.DataNotFound.Translate()); }

        data.Id = Guid.Empty;
        data.CreatedAt = DateTimeOffset.UtcNow;

        await userRepository.AddAsync(data);

        await unitOfWork.CommitAsync();

        return new SuccessResult(EMessages.Success.Translate());
    }

    public async Task<IDataResult<IEnumerable<UserResponseDto>>> GetAsync()
    {
        var datas = await userRepository.GetListAsync();

        var mapped = mapper.Map<IEnumerable<UserResponseDto>>(datas);

        return new SuccessDataResult<IEnumerable<UserResponseDto>>(mapped, EMessages.Success.Translate());
    }

    public async Task<IDataResult<UserByIdResponseDto>> GetAsync(Guid id)
    {
        var data = mapper.Map<UserByIdResponseDto>(await userRepository.GetAsync(id));

        return new SuccessDataResult<UserByIdResponseDto>(data, EMessages.Success.Translate());
    }

    public async Task<IResult> UpdateAsync(Guid id, UserUpdateRequestDto dto)
    {
        if (await userRepository.IsUserExistAsync(dto.Email, id))
        {
            return new ErrorResult(EMessages.UserIsExist.Translate());
        }

        var data = mapper.Map<User>(dto);
        data.Id = id;

        userRepository.UpdateUser(data);

        await unitOfWork.CommitAsync();

        return new SuccessResult(EMessages.Success.Translate());
    }

    public async Task<IDataResult<PaginatedList<UserResponseDto>>> GetAsGenericListAsync(GenericRequestDto dto)
    {
        var datas = await userRepository.GetAsGenericListAsync(dto);

        var responseDto = new PaginatedList<UserResponseDto>(mapper.Map<List<UserResponseDto>>(datas.Datas),
                                                             datas.TotalRecordCount,
                                                             datas.PageIndex,
                                                             dto.Pagination?.PageSize ?? 0);

        return new SuccessDataResult<PaginatedList<UserResponseDto>>(responseDto, EMessages.Success.Translate());
    }

    public async Task<IResult> SetImageAsync(Guid id, string? image = null)
    {
        var user = await userRepository.GetAsync(id);
        user.Image = image;

        userRepository.Update(user);

        await unitOfWork.CommitAsync();

        return new SuccessResult();
    }

    public async Task<IDataResult<string>> GetImageAsync(Guid id)
    {
        var user = await userRepository.GetAsNoTrackingAsync(u => u.Id == id);
        return user?.Image == null
            ? new ErrorDataResult<string>(EMessages.DataNotFound.Translate())
            : new SuccessDataResult<string>(user.Image, EMessages.Success.Translate());
    }
}